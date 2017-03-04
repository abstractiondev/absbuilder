using System;
using System.IO;
using absbuilder;

namespace AbstractionBuilder
{
    class Runner
    {
        private AbstractionEnvironment _env;

        private Runner() {}

        public Runner(AbstractionEnvironment env)
        {
            _env = env;
        }

        private void RunAbstraction(AbstractionItemType item)
        {
            _env.ItemName = item.nameRef; // Update given item name

            CleanUpAbstractionOutput();

            Console.WriteLine("Creating {0} abstractions", item.nameRef);
            foreach (var abs in _env.AbstractionXml.Builder.Abstractions)
            {
                if (abs.name.Equals(item.nameRef))
                {
                    foreach (var generator in abs.Generator)
                    {
                        var generatorFiles = ExecuteAssemblyGenerator(generator.name);
                        WriteGeneratorFiles(generatorFiles);
                    }
                }
            }
        }

        private void RunTransformation(TransformationItemType item)
        {
            _env.ItemName = item.nameRef; // Update given item name
            Console.WriteLine("Running transformation {0}", item.nameRef);

            foreach (var trans in _env.AbstractionXml.Builder.Transformations)
            {
                if (trans.name.Equals(item.nameRef))
                {
                    // #0 Clean up TRANS-in & -out directory
                    CleanUpTransformationIn();
                    CleanUpTransformationOutput();

                    // #1 Copy from ABS-from-in to TRANS-in
                    string transInDir = _env.CurrentInDirectory();
                    string[] files = _env.GetInputContentFiles(trans.sourceAbstraction);
                    foreach(var filename in files)
                    {
                        FileInfo file = new FileInfo(filename);
                        file.CopyTo(Path.Combine(transInDir, file.Name), true);
                    }
                    
                    // #2 Do transformation
                    string assemblyLocation = _env.CurrentItemAssembly();
                    string[] transFiles = _env.GetCurrentInputContentFiles();
                    object result;
                    try
                    {
                        result = DynaInvoke.InvokeMethod(assemblyLocation, trans.name, "GetGeneratorContent",
                                                         _env, transFiles);
                    } catch(Exception ex)
                    {
                        // #2 Legacy-structure support
                        result = DynaInvoke.InvokeMethod(assemblyLocation, "Transformer", "GetGeneratorContent",
                                                         transFiles);
                    }
                    Tuple<string, string>[] resultTupleArray = (Tuple<string, string>[]) result;
                    WriteGeneratorFiles(resultTupleArray);

                    // #3 copy data to from TRANS-OUT to ABS-to-in 
                    string absInDirectory = _env.InDirectory(trans.targetAbstraction);
                    if (!Directory.Exists(absInDirectory))
                        Directory.CreateDirectory(absInDirectory);
                    string[] files2 = _env.GetCurrentOutputContentFiles();
                    foreach (var filename in files2)
                    {
                        FileInfo file = new FileInfo(filename);
                        file.CopyTo(Path.Combine(absInDirectory, file.Name), true);
                    }
                }
            }
        }
        private void RunExecution(ExecutionItemType item)
        {
            _env.ItemName = item.nameRef; // Update given item name
            Console.WriteLine("Running custom command {0} {1} {2}", item.nameRef, item.className, item.methodName);

            string assemblyLocation = _env.CurrentItemAssembly();
            string[] xmlSourceFiles = _env.GetCurrentInputContentFiles();
            object result = DynaInvoke.InvokeMethod(assemblyLocation, item.className, item.methodName,
                _env, xmlSourceFiles);
        }

        public void Execution()
        {
            foreach (var x in _env.AbstractionXml.Builder.BuildExecution)
            {
                var absItem = x as AbstractionItemType;
                var transItem = x as TransformationItemType;
                var executionItem = x as ExecutionItemType;
                if (absItem != null)
                    RunAbstraction(absItem);
                else if (transItem != null)
                    RunTransformation(transItem);
                else if (executionItem != null)
                    RunExecution(executionItem);
                else
                    throw new NotSupportedException("Execute item type: " + x.GetType().Name);
            }
        }

        private Tuple<string, string>[] ExecuteAssemblyGenerator(string generatorClassName)
        {
            string[] xmlSourceFiles = _env.GetCurrentInputContentFiles();

            string assemblyLocation = _env.CurrentItemAssembly();
            object result;
            try
            {
                result = DynaInvoke.InvokeMethod(assemblyLocation, generatorClassName, "GetGeneratorContent",
                                                 _env, xmlSourceFiles);
            } catch(Exception)
            {
                // Legacy structure support without _env being passed
                result = DynaInvoke.InvokeMethod(assemblyLocation, generatorClassName, "GetGeneratorContent",
                                                 xmlSourceFiles);
            }
            Tuple<string, string>[] resultTupleArray = (Tuple<string, string>[])result;
            return resultTupleArray;
        }

        private void WriteGeneratorFiles(Tuple<string, string>[] generatorFiles)
        {
            string outputPath = _env.CurrentOutDirectory();
            foreach (var generatorFile in generatorFiles)
            {
                string fileName = Path.Combine(outputPath, generatorFile.Item1);
                string directoryName = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);
                File.WriteAllText(fileName, generatorFile.Item2);
            }
        }

        private void CleanUpAbstractionOutput()
        {
            CleanUpDirectory(_env.CurrentOutDirectory());
        }

        private void CleanUpTransformationIn()
        {
            CleanUpDirectory(_env.CurrentInDirectory());
        }
        private void CleanUpTransformationOutput()
        {
            CleanUpDirectory(_env.CurrentOutDirectory());
        }

        private void CleanUpDirectory(string directoryName)
        {
            var directoryInfo = new DirectoryInfo(directoryName);
            if (directoryInfo.Exists)
                directoryInfo.Delete(true);
            directoryInfo.Create();
        }
    }
}
