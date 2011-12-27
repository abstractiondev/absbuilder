using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
//using DocumentationABS.Documentation;
using Microsoft.VisualStudio.TextTemplating;
//using OperationABS.Operation;

namespace AbstractionBuilder
{
    class Program
    {
        static int Main(string[] args)
        {
            //CustomCmdLineHost host = new CustomCmdLineHost();
            //host.TemplateFileValue = @"C:\GitHub\kallex\private\Demos\CQRS_CustomerBankAccountDemo\Abstractions\OperationABS\Operation\CSharpCode_v1_0.tt";
            //OperationABS.Operation.CSharpCode_v1_0 generator = new CSharpCode_v1_0();
            //generator.Host = host;
            //string result = generator.TransformText();
            //TransformDocumentation();
            //GenerateDocumentation();
            if(args != null && args.Length > 0)
            {
                return ExecuteProgram(args);
            }
            Builder builder = new Builder();
            builder.Build();
            Console.WriteLine("Generations Done!");
            return 0;
        }

        private static int ExecuteProgram(string[] args)
        {
            Debugger.Launch();
            Console.WriteLine("Absbuilder Succeeded 0");
            Console.Error.WriteLine("Errori virhe!!!");
            return -1;
            //return 0;
        }

        private static void TransformDocumentation()
        {
        }

        /*
        private static void GenerateDocumentation()
        {
            CustomCmdLineHost host = new CustomCmdLineHost();
            host.TemplateFileValue = @"C:\GitHub\kallex\private\Demos\CQRS_CustomerBankAccountDemo\Abstractions\DocumentationABS\Documentation\DesignDocumentation_v1_0.tt";
            DocumentationABS.Documentation.DesignDocumentation_v1_0 generator = new DesignDocumentation_v1_0();
            generator.Host = host;
            var result = generator.GenerateDocuments();
            foreach(var item in result)
            {
                string fileName = @"c:\tmp\" + item.Name;
                File.WriteAllText(fileName, item.Content);
            }
        }
         * */
    }

    public class SchemaIncludeSupport
    {
        void GenerateXSDSerializer(string templateFile, string outputDirectory, bool generateNamespace, bool generateT4Tags,
            bool generateToTemplateOutput)
        {
            string xsdExeFileName = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) +
                                 @"\Microsoft SDKs\Windows\v7.0A\bin\xsd.exe";
            string xsdFile = GetXSDFile(templateFile);
            string outputClassFile = outputDirectory + "\\" + System.IO.Path.GetFileNameWithoutExtension(templateFile) + ".cs";
            string namespaceFileTag = generateNamespace ? "_namespace" : "";
            string outputttinclude = outputDirectory + "\\" + System.IO.Path.GetFileNameWithoutExtension(templateFile) + namespaceFileTag + ".ttinclude";
            string outputCsCode = outputDirectory + "\\" + System.IO.Path.GetFileNameWithoutExtension(templateFile) + namespaceFileTag + ".cs";
            string xsdParameters = "/c \"" + xsdFile + "\" /o:\"" + outputDirectory + "\"";
            string namespaceName = System.IO.Path.GetFileNameWithoutExtension(templateFile);
            if (generateNamespace == true && generateT4Tags == false)
            {
                xsdParameters = "/n:" + namespaceName + " " + xsdParameters;
            }
            System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo(xsdExeFileName, xsdParameters);
            WriteLog("Starting xsd.exe: " + xsdParameters);
            System.Diagnostics.Process proc = System.Diagnostics.Process.Start(processStartInfo);
            proc.WaitForExit();
            WriteLog("System.Diagnostics.Processing include tags...");
            string content = System.IO.File.ReadAllText(outputClassFile);
            System.IO.File.Delete(outputClassFile);
            string outputFile;
            if (generateT4Tags)
            {
                if (generateNamespace)
                {
                    content = "<" + "#+" +
                        System.Environment.NewLine + "public class " + namespaceName + " { " + System.Environment.NewLine +
                        content.Replace("using System.Xml.Serialization;", "") + "} " + "#" + ">";
                }
                else
                {
                    content = "<" + "#+" +
                        System.Environment.NewLine +
                        content.Replace("using System.Xml.Serialization;", "") + "#" + ">";
                }
                outputFile = outputttinclude;
            }
            else
                outputFile = outputCsCode;
            if (generateToTemplateOutput)
            {
                //this.GenerationEnvironment.Clear();
                //this.GenerationEnvironment.Append(content);
            }
            else
            {
                WriteLog("Writing to file: " + outputFile);
                System.IO.File.WriteAllText(outputFile, content);
            }
        }

        string GetXSDFile(string templateFile)
        {
            string directory = System.IO.Path.GetDirectoryName(templateFile);
            string xsdFile = directory + "\\" + System.IO.Path.GetFileNameWithoutExtension(templateFile) + ".xsd";
            return xsdFile;
        }

        void GenerateTTInclude(string templateFile, string outputDirectory, bool generateNamespace = false, bool generateT4Tags = true,
            bool generateToTemplateOutput = false)
        {
            GenerateXSDSerializer(templateFile, outputDirectory, generateNamespace, generateT4Tags, generateToTemplateOutput);
        }

        void GenerateTTInclude(string templateFile, bool generateNamespace = false)
        {
            GenerateTTInclude(templateFile, System.IO.Path.GetDirectoryName(templateFile), generateNamespace);
        }

        void WriteLog(string entry)
        {
        }

    }

}
