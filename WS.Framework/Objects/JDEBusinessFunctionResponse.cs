using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Framework.Objects
{
    public class JDEBusinessFunctionResponse
    {
        public int ReturnCode { get; set; }
        public string ReturnCodeDescription { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
}
