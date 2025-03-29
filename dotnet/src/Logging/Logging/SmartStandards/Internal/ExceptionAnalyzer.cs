using System;
using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Logging.SmartStandards.Internal {

  internal class ExceptionAnalyzer {

    internal static int InferEventIdByException(Exception ex) {

      if (ex is TargetInvocationException && ex.InnerException != null) {
        return InferEventIdByException(ex.InnerException);
      }

      // An einer Win32Exception hängt i.d.R. bereits eine EventId => diese verwenden

      if (ex is Win32Exception) {
        return ((Win32Exception)ex).NativeErrorCode;
      }

      // Falls der Absender die Konvention "MessageText #{EventId}" einhielt...

      int hashTagIndex = ex.Message.LastIndexOf("#");

      if (hashTagIndex >= 0 && int.TryParse(ex.Message.Substring(hashTagIndex + 1), out int id)) {
        return id;
      } else {

        // ...falls nicht: Wir leiten aus dem Exception-Typ eine EventId ab.

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
}
