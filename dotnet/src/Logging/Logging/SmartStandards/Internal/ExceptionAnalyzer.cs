using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace System.Logging.SmartStandards.Internal {

  internal class ExceptionAnalyzer {

    internal static int InferEventIdByException(Exception ex) {

      if (ex is TargetInvocationException && ex.InnerException != null) {
        return InferEventIdByException(ex.InnerException);
      }

      if (ex is Win32Exception) {
        return ((Win32Exception)ex).NativeErrorCode;
      }

      int hashTagIndex = ex.Message.LastIndexOf("#");

      if (hashTagIndex >= 0 && int.TryParse(ex.Message.Substring(hashTagIndex + 1), out int id)) {
        return id;
      } else {
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
