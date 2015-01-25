namespace Hdc.Mv.Inspection
{
    public static class InspectorFactory
    {
        public static ICircleInspector CreateCircleInspector(string typeName)
        {
            switch (typeName)
            {
                case "Hal":
                    return new CircleInspector();
                case "Sim":
                    return new SimCircleInspector();
            }
            return null;
        }

        public static IEdgeInspector CreateEdgeInspector(string typeName)
        {
            switch (typeName)
            {
                case "Hal":
                    return new EdgeInspector();
                case "Sim":
                    return new SimEdgeInspector();
            }
            return null;
        }
    }
}