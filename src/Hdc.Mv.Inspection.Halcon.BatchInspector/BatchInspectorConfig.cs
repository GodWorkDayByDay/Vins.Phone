namespace Hdc.Mv.Inspection.Halcon.BatchInspector
{
    public class BatchInspectorConfig
    {
        public DirectoryViewModelCollection Directories { get; set; }

        public string InspectionSchemaFileName { get; set; }

        public BatchInspectorConfig()
        {
            Directories = new DirectoryViewModelCollection();
        }
    }
}