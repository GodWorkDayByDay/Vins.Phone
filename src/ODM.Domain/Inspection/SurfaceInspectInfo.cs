namespace ODM.Domain.Inspection
{
    public class SurfaceInspectInfo
    {
        public int WorkpieceIndex { get; set; }

        public int SurfaceTypeIndex { get; set; }

        public ImageInfo ImageInfo { get; set; }

        public InspectInfo InspectInfo { get; set; }
    }
}