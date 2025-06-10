using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Logging.SmartStandards.Internal {

  internal class SourceContextDiscoverer {

    private static Dictionary<Assembly, String> _SourceContextNamesByAssembly = new Dictionary<Assembly, String>();

    internal static string InferSourceContext(Assembly representativeAssembly) {

      lock (_SourceContextNamesByAssembly) {
        string sourceContextName;

        if (_SourceContextNamesByAssembly.TryGetValue(representativeAssembly, out sourceContextName)) {
          return sourceContextName;
        }

        AssemblyMetadataAttribute attrib = representativeAssembly.GetCustomAttributes<AssemblyMetadataAttribute>()?.Where(
          (a) => a.Key.Equals("SourceContext", StringComparison.CurrentCultureIgnoreCase)
        ).FirstOrDefault();

        if (attrib != null) {
          sourceContextName = attrib.Value;
        } else {
          sourceContextName = representativeAssembly.GetName().Name;
        }

        _SourceContextNamesByAssembly[representativeAssembly] = sourceContextName;

        return sourceContextName;
      }

    }

  }

}
