using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TextTemplating;
using OperationABS.Operation;

namespace AbstractionBuilder
{
    class Program
    {
        static int Main(string[] args)
        {
            CustomCmdLineHost host = new CustomCmdLineHost();
            host.TemplateFileValue = @"C:\GitHub\kallex\private\Demos\CQRS_CustomerBankAccountDemo\Abstractions\OperationABS\Operation\CSharpCode_v1_0.tt";
            OperationABS.Operation.CSharpCode_v1_0 generator = new CSharpCode_v1_0();
            generator.Host = host;
            string result = generator.TransformText();
            Console.WriteLine("Generations Done!");
            return 0;
        }
    }

}
