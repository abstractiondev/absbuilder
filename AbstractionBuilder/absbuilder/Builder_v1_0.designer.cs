 
using System;

	namespace AbstractionBuilder {
	partial class Builder {
        public void Build()
        {
            Tuple<string, string>[] generatorFiles = null;
		        generatorFiles = ExecuteAssemblyGenerator("OperationToDocumentation", "TRANS", "Transformer");
        WriteGeneratorFiles(generatorFiles, "OperationToDocumentation", "TRANS");
			        generatorFiles = ExecuteAssemblyGenerator("Operation", "ABS", "CSharpCode_v1_0");
	        WriteGeneratorFiles(generatorFiles, "Operation", "ABS");
			        generatorFiles = ExecuteAssemblyGenerator("Documentation", "ABS", "DesignDocumentation_v1_0");
	        WriteGeneratorFiles(generatorFiles, "Documentation", "ABS");
		            //generatorFiles = ExecuteAssemblyGenerator("Documentation", "ABS", "DesignDocumentation_v1_0");
        }
	}
}
		