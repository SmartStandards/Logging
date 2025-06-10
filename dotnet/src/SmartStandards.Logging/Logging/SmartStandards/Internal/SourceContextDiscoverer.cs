using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Logging.SmartStandards.Internal {

  internal class SourceContextDiscoverer { // v 1.0.0

    private static Dictionary<Assembly, String> _SourceContextNamesByAssembly = new Dictionary<Assembly, String>();

    /// <summary>
    ///   Searches for [assembly: AssemblyMetadata("SourceContext", "CustomSourceContextName")]
    /// </summary>
    /// <returns> S.th. like "CustomSourceContextName" </returns>
    internal static string InferSourceContext(Assembly candidateAssembly) {

      lock (_SourceContextNamesByAssembly) {

        string sourceContextName;

        if (_SourceContextNamesByAssembly.TryGetValue(candidateAssembly, out sourceContextName)) {
          return sourceContextName;
        }

        AssemblyMetadataAttribute foundAttribute = candidateAssembly.GetCustomAttributes<AssemblyMetadataAttribute>()?.Where(
          (AssemblyMetadataAttribute a) => a.Key.Equals("SourceContext", StringComparison.CurrentCultureIgnoreCase)
        ).FirstOrDefault();

        if (foundAttribute != null) {
          sourceContextName = foundAttribute.Value;
        } else {
          sourceContextName = candidateAssembly.GetName().Name;
        }

        _SourceContextNamesByAssembly[candidateAssembly] = sourceContextName;

        return sourceContextName;
      }

    }

  }

}
