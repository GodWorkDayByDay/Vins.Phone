using System.Collections.Generic;
using HalconDotNet;
using Hdc.Mv.Inspection;

namespace ODM.Domain.Inspection
{
    public class SimInspector : IInspector
    {
        private List<bool> _results = new List<bool>();

        private int _index;

        public void AddResult(bool result)
        {
            _results.Add(result);
        }

        public void AddResults(params bool[] result)
        {
            for (int i = 0; i < result.Length; i++)
            {
                _results.Add(result[i]);
            }
        }

        public void Dispose()
        {
        }

        public bool Init()
        {
            return true;
        }

        public bool LoadParameters()
        {
            return true;
        }

        public void FreeObject()
        {
        }

        public InspectionInfo Inspect(HImage imageInfo)
        {
            var inspectionInfo = new InspectionInfo();

            var offset = _index % _results.Count;
            var result = _results[offset];
            if (!result)
            {
                inspectionInfo.DefectInfos.Add(new Hdc.Mv.Inspection.DefectInfo());
            }

            _index++;

            return inspectionInfo;
        }
    }
}