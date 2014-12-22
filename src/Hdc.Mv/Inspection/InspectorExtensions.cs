using System.Threading.Tasks;

namespace Hdc.Mv.Inspection
{
    public static class InspectorExtensions
    {
        public static Task<InspectionInfo> InspectAsync(this IInspector inspector, ImageInfo imageInfo)
        {
            return Task.Run(() => inspector.Inspect(imageInfo));
        }
    }
}