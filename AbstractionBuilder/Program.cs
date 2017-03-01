using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
//using LibZ.Bootstrap;
//using DocumentationABS.Documentation;
using Microsoft.VisualStudio.TextTemplating;
//using OperationABS.Operation;

namespace AbstractionBuilder
{
    class Program
    {
        static int Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                var assemblyName = new AssemblyName(eventArgs.Name).Name;
                string resourceName = assemblyName + ".dll";
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                        return null;
                    var assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };

            //LibZResolver.Startup(() =>
            {

                // 
                string runningPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string directiveXml;
                string contentRootPath = null;

                if (args != null && args.Length > 0)
                {
                    directiveXml = args[0];
                    contentRootPath = args[1];
                }
                else
                {
                    var dirInfo = new DirectoryInfo(runningPath);
                    dirInfo = dirInfo.Parent.Parent.Parent.Parent;
                    //@TODO: This should be changed into a loop that many contents can be run
                    directiveXml = Path.Combine(dirInfo.FullName, 
                        "AbstractionContent", "absbuilder", "In", "Content_v1_0", "AbstractionBuilderContent_v1_0.xml");
                }
                Console.WriteLine("Using {0} as command file.", directiveXml);

                AbstractionEnvironment env = new AbstractionEnvironment(runningPath, directiveXml, contentRootPath);

                Runner runner = new Runner(env);
                runner.Execution();
            }
            //);
            return 0;
        }

        private static int ExecuteXSDIncludeGenerator(string xsdFileName)
        {
            FileInfo fileInfo = new FileInfo(xsdFileName);
            xsdFileName = fileInfo.FullName;
            DirectoryInfo dirInfo = new DirectoryInfo(Path.GetDirectoryName(xsdFileName));
            // Generate the include files to parent directory, not in the content directory
            dirInfo = dirInfo.Parent;
            // Generating the T4 generator usable no-namespace include with T4 tags
            SchemaIncludeSupport.GenerateTTInclude(xsdFileName, dirInfo.FullName);
            // Generating the Transformation usable namespace include without T4 tags 
            SchemaIncludeSupport.GenerateTTInclude(xsdFileName, dirInfo.FullName, generateNamespace: true, generateT4Tags: false);
            return 0;
        }

        private static void TransformDocumentation()
        {
        }

    }
}
