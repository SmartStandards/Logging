using System;

namespace Logging.SmartStandards {

  public static class ExceptionExtensions {

    public static Exception Wrap(this Exception extendee, string message) {

      Exception wrappedException = new Exception(message, extendee);

      return wrappedException;
    }

    /// <summary>
    ///   Wraps an outer exception 
    /// </summary>
    /// <param name="extendee"> Will become the inner exception. </param>
    /// <param name="kindId"> Will be added as #-suffix to the message (SmartStandards compliant parsable). </param>
    /// <param name="message"> A custom message to add value to the wrapped exception.</param>
    /// <returns> A new (outer) exception.</returns>
    public static Exception Wrap(this Exception extendee, int kindId, string message) {

      WrappedException wrappedException = new WrappedException(message + " #" + kindId.ToString(), extendee);

      return wrappedException;
    }

    internal class WrappedException : Exception {

      public WrappedException(string message, Exception inner) : base (message, inner) {
      }

    }

  }

}
