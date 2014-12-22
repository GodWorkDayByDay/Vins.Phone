using System;

namespace Hdc.Mv.Inspection.Halcon
{
    public class HalconInspectorException: Exception
    {
        public HalconInspectorException()
        {
        }

        public HalconInspectorException(string message) : base(message)
        {
        }

        public HalconInspectorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}