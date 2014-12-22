namespace ODM.Domain.Inspection
{
    public enum InspectState
    {
        Default,
        Ready,
        Grabbing,
        Grabbed,
        Inspecting,
//        Inspected,
        InspectedWithAccepted,
        InspectedWithRejected,
    }
}