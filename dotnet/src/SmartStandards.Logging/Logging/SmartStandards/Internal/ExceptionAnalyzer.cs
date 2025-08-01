﻿using System;
using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Logging.SmartStandards.Internal {

  internal class ExceptionAnalyzer { // v 1.0.0

    internal static int InferEventKindIdByException(Exception ex) {

      // 'Zwiebel' durch Aufrufe via Reflection (InnerException ist mehr repräsentativ)

      if (ex is TargetInvocationException && ex.InnerException != null) {
        return InferEventKindIdByException(ex.InnerException);
      }

      // 'Zwiebel' durch Task.Run (InnerException ist mehr repräsentativ)

      if (ex is AggregateException) {
        AggregateException castedAggregateException = (AggregateException)ex;
        if (
          castedAggregateException.InnerExceptions != null &&
          castedAggregateException.InnerExceptions.Count == 1 //falls nur 1 enthalten (macht MS gern)
        ) {
          return InferEventKindIdByException(castedAggregateException.InnerExceptions[0]);
        }
      }

      // An einer Win32Exception hängt i.d.R. bereits eine eventKindId => diese verwenden

      if (ex is Win32Exception) {
        return ((Win32Exception)ex).NativeErrorCode;
      }

      // Falls der Absender die Konvention "MessageText #{eventKindId}" einhielt...

      int hashTagIndex = ex.Message.LastIndexOf('#');

      if (hashTagIndex >= 0 && int.TryParse(ex.Message.Substring(hashTagIndex + 1), out int id)) {
        return id;
      }

      // 'Zwiebel' durch Exception.Wrap (InnerException ist mehr repräsentativ)

      if (ex is ConcretizedException) {
        return InferEventKindIdByException(ex.InnerException);
      }

      // Fallback zuletzt: Wir leiten aus dem Exception-Typ eine eventKindId ab.

      using (var md5 = MD5.Create()) {
        int hash = BitConverter.ToInt32(md5.ComputeHash(Encoding.UTF8.GetBytes(ex.GetType().Name)), 0);
        if (hash < 0) {
          return hash * -1;
        }
        return hash;
      }

    }

  }
}
