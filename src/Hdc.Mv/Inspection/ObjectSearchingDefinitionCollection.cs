using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hdc.Mv.Inspection
{
    public class ObjectSearchingDefinitionCollection : Collection<ObjectSearchingDefinition>
    {
        public ObjectSearchingDefinitionCollection()
        {
        }

        public ObjectSearchingDefinitionCollection(IList<ObjectSearchingDefinition> list)
            : base(list)
        {
        }
    }
}