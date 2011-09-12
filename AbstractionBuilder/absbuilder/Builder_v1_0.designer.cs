 
using System;

	namespace AbstractionBuilder {
	partial class Builder {
        public void Build()
        {
			CleanUp();
            Tuple<string, string>[] generatorFiles = null;
				FetchTransformationSources("OperationToDocumentation", "Operation");
        generatorFiles = ExecuteAssemblyGenerator("OperationToDocumentation", "TRANS", "Transformer");
        WriteGeneratorFiles(generatorFiles, "OperationToDocumentation", "TRANS");
		PushTransformationTargets("OperationToDocumentation", "Documentation");
			        generatorFiles = ExecuteAssemblyGenerator("Documentation", "ABS", "DesignDocumentation_v1_0");
	        WriteGeneratorFiles(generatorFiles, "Documentation", "ABS");
		            //generatorFiles = ExecuteAssemblyGenerator("Documentation", "ABS", "DesignDocumentation_v1_0");
        }
		
		private void CleanUp()
		{
		            CleanUpTransformationInputAndOutput("OperationToDocumentation", "Documentation");
				            CleanUpAbstractionOutput("Documentation");
						}
	}
}
		