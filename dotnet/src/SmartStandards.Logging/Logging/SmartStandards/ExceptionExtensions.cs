using System;

namespace Logging.SmartStandards {

  public static class ExceptionExtensions { // v 1.0.0

    /// <summary>
    ///   Puts your envelope exception around an exception coming from elsewhere.
    /// </summary>
    /// <param name="extendee"> The 3rd party exception (becoming the inner exception). </param>
    /// <param name="message"> Your message adding value to the 3rd party exception. </param>
    /// <returns> A new WrappedException instance containing the extendee as inner exception. </returns>
    /// <remarks>
    ///   Purpose: The 3rd party exception might be generic (like null reference, etc.) and thus not very helpful.
    ///   Add specific information (like IDs etc.) in to the envelope's message.
    /// </remarks>
    public static Exception Wrap(this Exception extendee, string message) { // REQ #395397931

      ConcretizedException wrappedException = new ConcretizedException(message, extendee);

      return wrappedException;
    }

    /// <summary>
    ///   Puts your envelope exception around an exception coming from elsewhere.
    /// </summary>
    /// <param name="extendee"> The 3rd party exception (becoming the inner exception). </param>
    /// <param name="eventKindId"> Will be added as #-suffix to the message (SmartStandards compliant parsable). </param>
    /// <param name="message"> Your message adding value to the 3rd party exception. </param>
    /// <returns> A new WrappedException instance containing the extendee as inner exception. </returns>
    /// <remarks>
    ///   Purpose: The 3rd party exception might be generic (like null reference, etc.) and thus not very helpful.
    ///   Add specific information (like IDs etc.) in to the envelope's message.
    /// </remarks>
    public static Exception Wrap(this Exception extendee, int eventKindId, string message) { // REQ #395397931

      ConcretizedException wrappedException = new ConcretizedException(message + " #" + eventKindId.ToString(), extendee);

      return wrappedException;
    }

  }

  internal class ConcretizedException : Exception {

    public ConcretizedException(string message, Exception inner) : base(message, inner) {
    }

    public override string StackTrace {
      get {
        // if our StackTrace is empty (because usually this Wrapper wasnt thrown, either logged directly),
        // return the InnerException's StackTrace.
        if (string.IsNullOrWhiteSpace(base.StackTrace)) {
          return InnerException.StackTrace;
        }
        return base.StackTrace;
      }
    }
    
  }

}
