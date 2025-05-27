using Logging.SmartStandards.Filtering;
using Logging.SmartStandards.Sinks;
using Logging.SmartStandards.Textualization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Logging.SmartStandards.Centralized {

  public class LogEntryRemoteSink : ILogEntrySink {

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private HttpClient _HttpClient = new HttpClient {
      Timeout = TimeSpan.FromSeconds(10)
    };

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string _CachedAuthHeader = null;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private DateTime _AuthHeaderCacheTime = DateTime.MinValue;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string _CachedUrl = null;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private DateTime _UrlCacheTime = DateTime.MinValue;

    /// <summary>
    /// Creates a new instance of the RemoteSinkRedirector which can be used to redirect LogEvents to a remote sink.
    /// </summary>
    /// <param name="endpointUrl">
    ///  The url of that remote logging sink to be used as redirection target.
    /// </param>
    public LogEntryRemoteSink(string endpointUrl) {
      this.EndpointUrlGetter = () => endpointUrl;
      this.UrlCacheSec = -1;
    }

    /// <summary>
    /// Creates a new instance of the RemoteSinkRedirector which can be used to redirect LogEvents to a remote sink.
    /// </summary>
    /// <param name="endpointUrlGetter">
    ///  A method, that returns the url of the remote logging sink to be used as redirection target.
    /// </param>
    public LogEntryRemoteSink(Func<string> endpointUrlGetter) {
      this.EndpointUrlGetter = endpointUrlGetter;
      this.UrlCacheSec = 300;
    }

    #region " Configuration Properties "

    /// <summary>
    ///  A method, that returns the url of the remote logging sink to be used as redirection target.
    /// </summary>
    public Func<string> EndpointUrlGetter { get; set; } = null;

    /// <summary>
    ///  A method, that returns a string, which should be placed within the "authorization"-header of any http-request sended to the remote logging sink 
    /// </summary>
    public Func<string> AuthHeaderGetter { get; set; } = null;

    /// <summary>
    /// Configures the invalidation-timespan (in seconds) after which the 'EndpointUrlGetter' will be invoked again.
    /// </summary>
    public int UrlCacheSec { get; set; } = 0;

    /// <summary>
    /// Configures the invalidation-timespan (in seconds) after which the 'AuthHeaderGetter' will be invoked again.
    /// </summary>
    public int AuthHeaderCacheSec { get; set; } = 300;

    public TimeSpan HttpClientTimeout {
      get {
        return _HttpClient.Timeout;
      }
      set {
        _HttpClient.Timeout = value;
      }
    }

    #endregion

    public void ReceiveLogEntries(LogEntry[] logEntries) {

      if (this.AuthHeaderGetter != null) {
        if (_AuthHeaderCacheTime < DateTime.Now) {
          _CachedAuthHeader = this.AuthHeaderGetter.Invoke();
          _AuthHeaderCacheTime = DateTime.Now.AddSeconds(this.AuthHeaderCacheSec);
        }
      }

      if (this.EndpointUrlGetter != null) {
        if (_UrlCacheTime < DateTime.Now) {
          _CachedUrl = this.EndpointUrlGetter.Invoke();
          _UrlCacheTime = DateTime.Now.AddSeconds(this.UrlCacheSec);
        }
      }

      if (string.IsNullOrEmpty(_CachedUrl)) {
        return; //if there is no url, redirection will be turned off
      }

      var serilaizer = new JsonSerializer();
      var rawRequestBody = new StringBuilder(8000);
      var rawWtr = new StringWriter(rawRequestBody);
      var jsonWriter = new JsonTextWriter(rawWtr);

      //one entry should be a single line
      serilaizer.Formatting = Formatting.None;
      jsonWriter.Formatting = Formatting.None;

      jsonWriter.WriteStartArray();
      rawWtr.WriteLine();

      foreach (LogEntry logEntry in logEntries) {
        serilaizer.Serialize(rawWtr, logEntry);
        rawWtr.WriteLine(",");
      }
      jsonWriter.WriteEndArray();

      using (var request = new HttpRequestMessage(HttpMethod.Post, _CachedUrl)) {

        request.Content = new StringContent(rawRequestBody.ToString(), Encoding.UTF8, "application/json"); ;

        if (!string.IsNullOrWhiteSpace(_CachedAuthHeader)) {
          request.Headers.Add("Authorization", _CachedAuthHeader);
        }

        try {
          try {

            Task<HttpResponseMessage> requestTask = _HttpClient.SendAsync(request);
            requestTask.Wait();

            //Task<string> contentRetrivalTask = requestTask.Result.Content.ReadAsStringAsync();
            //contentRetrivalTask.Wait();
            //string responseContent = contentRetrivalTask.Result;
            //var responseHeaders = requestTask.Result.Headers;
            //string reasonPhrase = requestTask.Result.ReasonPhrase;

          }
          catch (AggregateException ex) {
            if (ex.InnerExceptions.Count == 1) {
              throw ex.InnerExceptions[0];
            }
            throw;
          }
        }
        catch (TaskCanceledException ex) {
          throw new TimeoutException($"The http call to '{_CachedUrl}' was canceled (configured timeout is {_HttpClient.Timeout.TotalSeconds}s)", ex);
        }
        catch (Exception ex) {
          throw new Exception($"The http call to '{_CachedUrl}' failed: {ex.Message}", ex);
        }

      }

    }

    #region " Filter-Rules "

    private List<LogEntryFilteringRule> _LocalFilteringRules = new List<LogEntryFilteringRule>();
    public IList<LogEntryFilteringRule> LocalFilteringRules {
      get {
        return new List<LogEntryFilteringRule>();
      }
    }

    LogEntryFilteringRule[] ILogEntrySink.GetLogLevelFilteringRules() {
      LogEntryFilteringRule[] receivedRules = this.RequestRemoteFilteringRules();
      lock (_LocalFilteringRules) {
        return _LocalFilteringRules.Union(receivedRules).ToArray();
      }
    }

    private LogEntryFilteringRule[] RequestRemoteFilteringRules() {

      if (this.AuthHeaderGetter != null) {
        if (_AuthHeaderCacheTime < DateTime.Now) {
          _CachedAuthHeader = this.AuthHeaderGetter.Invoke();
          _AuthHeaderCacheTime = DateTime.Now.AddSeconds(this.AuthHeaderCacheSec);
        }
      }

      if (this.EndpointUrlGetter != null) {
        if (_UrlCacheTime < DateTime.Now) {
          _CachedUrl = this.EndpointUrlGetter.Invoke();
          _UrlCacheTime = DateTime.Now.AddSeconds(this.UrlCacheSec);
        }
      }

      if (string.IsNullOrEmpty(_CachedUrl)) {
        return Array.Empty<LogEntryFilteringRule>(); //if there is no url, redirection will be turned off
      }

      using (var request = new HttpRequestMessage(HttpMethod.Post, _CachedUrl)) {

        request.Content = new StringContent("{\n}", Encoding.UTF8, "application/json"); ;

        if (!string.IsNullOrWhiteSpace(_CachedAuthHeader)) {
          request.Headers.Add("Authorization", _CachedAuthHeader);
        }

        try {
          try {

            Task<HttpResponseMessage> requestTask = _HttpClient.SendAsync(request);
            requestTask.Wait();

            Task<string> contentRetrivalTask = requestTask.Result.Content.ReadAsStringAsync();
            contentRetrivalTask.Wait();
            string responseContent = contentRetrivalTask.Result;

            LogEntryFilteringRule[] receivedRules = JsonConvert.DeserializeObject<LogEntryFilteringRule[]>(responseContent);

            return receivedRules;

            //var responseHeaders = requestTask.Result.Headers;
            //string reasonPhrase = requestTask.Result.ReasonPhrase;

          }
          catch (AggregateException ex) {
            if (ex.InnerExceptions.Count == 1) {
              throw ex.InnerExceptions[0];
            }
            throw;
          }
        }
        catch (TaskCanceledException ex) {
          throw new TimeoutException($"The http call to '{_CachedUrl}' was canceled (configured timeout is {_HttpClient.Timeout.TotalSeconds}s)", ex);
        }
        catch (Exception ex) {
          throw new Exception($"The http call to '{_CachedUrl}' failed: {ex.Message}", ex);
        }

      }

      #endregion

    }

  }

}
