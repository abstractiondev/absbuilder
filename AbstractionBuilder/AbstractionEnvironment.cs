using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
//using AbstractionConfig;

namespace AbstractionBuilder
{
    public class AbstractionEnvironment
    {
        // Builder action xml-filepath
        public string BuilderXmlFile { get; private set; }
        // Directory where absbuilder is running
        public string AbstractionBuilderDirectory { get; private set; }
        // Absbuilder XML (read and converted to C# class)
        public absbuilder.AbstractionBuilderType AbstractionXml { get; private set; }

        // Item name which is currently under use (Abstraction,transformation or transformation)
        public string ItemName { get; set; }

        // Private string to create assemblypath for abstractions
        private string LocationFormat { get; set; }
        // Private string for content root path
        private string ContentRootPath { get; set; }

        private const string AbstractionInputFolder = "In";
        private const string AbstractionOutputFolder = "Out";
        private const string AbstractionContentFolder = "AbstractionContent";

        // Hide normal constructor
        private AbstractionEnvironment() {}
        // Constructor
        public AbstractionEnvironment(string path, string xmlFile)
        {
            AbstractionBuilderDirectory = path;
            BuilderXmlFile = xmlFile;
            AbstractionXml = LoadXml<absbuilder.AbstractionBuilderType>(xmlFile);

            // Initialize ContentSupport-class
            var dirInfo = new DirectoryInfo(path);
            dirInfo = dirInfo.Parent.Parent.Parent.Parent;
            //ContentSupport.ContentRootPath = Path.Combine(dirInfo.FullName, AbstractionContentFolder);
            ContentRootPath = Path.Combine(dirInfo.FullName, AbstractionContentFolder);

            LocationFormat = path.Replace(@"\absbuilder\AbstractionBuilder\", @"\{0}\") + @"\{0}.dll";

        }

/*        public void SetAbstractionName(string abstractionName)
        {
            ItemName = abstractionName;
        }
 */
        // XML initializer
        private T LoadXml<T>(string xmlFileName)
        {
            using (FileStream fStream = File.OpenRead(xmlFileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                T result = (T)serializer.Deserialize(fStream);
                fStream.Close();
                return result;
            }
        }

        public string CurrentItemAssembly()
        {
            return Assembly(ItemName);
        }
        public string Assembly(string name)
        {
            return string.Format(LocationFormat, name);
        }

        // Since directive XML can contain many abstractions, it is needed
        // that user can get current abstraction path, which is running
        public string CurrentInDirectory()
        {
            return InDirectory(ItemName);
        }
        public string InDirectory(string abstractionName)
        {
            // Removing the legacy ABS extension on the content
            abstractionName = CleanupNameExtension(abstractionName);
            string path = Path.Combine(ContentRootPath, abstractionName, AbstractionInputFolder);
            return path;
        }

        private string CleanupNameExtension(string abstractionName)
        {
            if (abstractionName.EndsWith("ABS"))
                abstractionName = abstractionName.Substring(0, abstractionName.Length - 3);
            else if (abstractionName.EndsWith("TRANS"))
                abstractionName = abstractionName.Substring(0, abstractionName.Length - 5);
            return abstractionName;
        }

        // Since directive XML can contain many abstractions, it is needed
        // that user can get current abstraction path, which is running
        public string CurrentOutDirectory()
        {
            return OutDirectory(ItemName);
        }
        public string OutDirectory(string abstractionName)
        {
            // Removing the legacy ABS extension on the content
            abstractionName = CleanupNameExtension(abstractionName);
            string path = Path.Combine(ContentRootPath, abstractionName, AbstractionOutputFolder);
            return path;
        }

        public string[] GetCurrentInputContentFiles()
        {
            return GetInputContentFiles(ItemName);
        }
        public string[] GetInputContentFiles(string abstractionName)
        {
            string path = InDirectory(abstractionName);

            string[] fileNames = Directory.GetFiles(path, "*.xml", SearchOption.AllDirectories);
            return fileNames;
        }
        public string[] GetCurrentOutputContentFiles()
        {
            return GetOutputContentFiles(ItemName);
        }
        public string[] GetOutputContentFiles(string abstractionName)
        {
            string path = OutDirectory(abstractionName);

            string[] fileNames = Directory.GetFiles(path, "*.xml", SearchOption.AllDirectories);
            return fileNames;
        }
    }
}
