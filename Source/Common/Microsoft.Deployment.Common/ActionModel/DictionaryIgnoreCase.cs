using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Common.ActionModel
{
    public class DictionaryIgnoreCase<TV> : Dictionary<string, TV>
    {
        public DictionaryIgnoreCase() : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }

}
