using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace DiscoveryLoader
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyLoad += (o, e) =>
            {
                if (e.LoadedAssembly.FullName.Contains("StrideWrapper"))
                {
                    // force Direct3D11 rendering mode
                    var cmdLineInstance = Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "UI Foundation.dll")).GetType("SpaceClaim.CommandLine").GetProperty("Instance", BindingFlags.Static | BindingFlags.Public).GetValue(null, null);
                    cmdLineInstance.GetType().GetProperty("Renderer").SetValue(cmdLineInstance, 3); // Direct3D11
                }
                else if (e.LoadedAssembly.FullName.Contains("Disco.CoreUI"))
                {
                    // disable startup compatibility error checks
                    var cmdLineArgs = Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Disco.Application.dll")).GetType("Ansys.Disco.CommandLine.CommandLineArgs").GetMethod("Parse", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[] { args });
                    cmdLineArgs.GetType().GetProperty("SkipHardwareCompatibilityChecks", BindingFlags.Instance | BindingFlags.Public).SetValue(cmdLineArgs, true);
                    cmdLineArgs.GetType().GetProperty("SkipPrerequisiteChecks", BindingFlags.Instance | BindingFlags.Public).SetValue(cmdLineArgs, true);
                    cmdLineArgs.GetType().GetProperty("Configuration", BindingFlags.Instance | BindingFlags.Public).SetValue(cmdLineArgs, 2); // Modeling only
                    
                    var cmdlineValue = Type.GetType("System.Windows.Application, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35").GetProperty("Current", BindingFlags.Static | BindingFlags.Public).GetValue(null, null);
                    cmdlineValue.GetType().BaseType.GetField("_commandLineArgs", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(cmdlineValue, cmdLineArgs);
                }
            };

            // use binding redirects from Discovery.exe.config
            var assemblyBinding = XDocument.Load(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Discovery.exe.config")).Root.Element("runtime").Elements(XName.Get("assemblyBinding", "urn:schemas-microsoft-com:asm.v1"));
            AppDomain.CurrentDomain.AssemblyResolve += (o, e) =>
            {
                var requestedAssembly = new AssemblyName(e.Name);
                var dependentAssemblies = assemblyBinding.Select(x => x.Descendants(XName.Get("dependentAssembly", "urn:schemas-microsoft-com:asm.v1"))).Where(x => x.Elements(XName.Get("assemblyIdentity", "urn:schemas-microsoft-com:asm.v1")).Attributes(XName.Get("name")).First().Value == requestedAssembly.Name).SingleOrDefault();
                if (dependentAssemblies == null)
                    return null;

                var forwardedVersion = dependentAssemblies.Elements(XName.Get("bindingRedirect", "urn:schemas-microsoft-com:asm.v1")).Attributes(XName.Get("newVersion")).First().Value;
                requestedAssembly.Version = new Version(forwardedVersion);
                return Assembly.Load(requestedAssembly);
            };

            // start the application
            Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Discovery.exe")).GetType("Ansys.Disco.Program").GetMethod("Main").Invoke(null, new object[] { args });
        }
    }
}
