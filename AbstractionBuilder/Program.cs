using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using DocumentationABS.Documentation;
using Microsoft.VisualStudio.TextTemplating;
using OperationABS.Operation;

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
            Builder builder = new Builder();
            builder.Build();
            Console.WriteLine("Generations Done!");
            return 0;
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

}
