using System.IO;
using Hdc.Collections.Generic;
using Hdc.Reflection;
using Hdc.Serialization;

namespace Hdc.Mv.Inspection
{
    public static class InspectionSchemaExtensions
    {
        public static InspectionSchema LoadFromAssemblyDir(this string shortFileName)
        {
            var dir = typeof(InspectionSchemaExtensions).Assembly.GetAssemblyDirectoryPath();
            var fileName = Path.Combine(dir, shortFileName);
            if (!File.Exists(fileName))
            {
                var ds = InspectionSchemaFactory.CreateDefaultSchema();
                ds.SerializeToXamlFile(fileName);
            }
            var schema = fileName.DeserializeFromXamlFile<InspectionSchema>();
            return schema;
        }

        public static InspectionSchema LoadFromFile(this string fileName)
        {
            if (!File.Exists(fileName))
            {
                var ds = InspectionSchemaFactory.CreateDefaultSchema();
                ds.SerializeToXamlFile(fileName);
            }
            var schema = fileName.DeserializeFromXamlFile<InspectionSchema>();
            return schema;
        }
    }
}