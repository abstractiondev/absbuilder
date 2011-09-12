using System;
using System.Reflection;
using AbstractionConfig;

namespace AbstractionBuilder
{
    public partial class Builder
    {
        private readonly string LocationFormat;

        public Builder()
        {
            ContentSupport.ContentRootPath =
                @"C:\GitHub\kallex\private\Demos\CQRS_CustomerBankAccountDemo\Abstractions\AbstractionContent\";

            string currentAssemblyLocation = Assembly.GetExecutingAssembly().Location;
            LocationFormat = currentAssemblyLocation.Replace(@"\absbuilder\AbstractionBuilder\",
                                                             @"\{0}{1}\").Replace("AbstractionBuilder.exe",
                                                                               "{0}{1}.dll");
        }

        //public void Build()
        //{
        //    Tuple<string, string>[] generatorFiles;
        //    generatorFiles = ExecuteAssemblyGenerator("Documentation", "ABS", "DesignDocumentation_v1_0");
        //    WriteGeneratorFiles(generatorFiles);
        //}

        private void WriteGeneratorFiles(Tuple<string, string>[] generatorFiles, string abstractionName, string abstractionTypeString)
        {
        }

        private Tuple<string, string>[] ExecuteAssemblyGenerator(string abstractionName, string abstractionTypeString, string generatorClassName)
        {
            string assemblyLocation = String.Format(LocationFormat, abstractionName, abstractionTypeString);
            object result = DynaInvoke.InvokeMethod(assemblyLocation, generatorClassName, "GetGeneratorContent",
                                    new string[0]);
            Tuple<string, string>[] resultTupleArray = (Tuple<string, string>[]) result;
            return resultTupleArray;
        }

        private void FetchTransformationSources(string transformationName, string sourceAbstractionName)
        {
            ContentSupport.CleanupTransformationDirectories(transformationName);
            ContentSupport.CopyFromSourceToTrans(sourceAbstractionName, transformationName);
        }

        private void PushTransformationTargets(string transformationName, string targetAbstractionName)
        {
            ContentSupport.CopyFromTransToTarget(transformationName, targetAbstractionName);
        }
    }
}