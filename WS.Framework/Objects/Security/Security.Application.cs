using System.Collections.Generic;
using WS.Framework.WSJDEData;

namespace WS.Framework.Objects.Security
{
    public class SecurityApplication
    {
        public int SecurityID { get; set; }
        public int ApplicationID { get; set; }
        public string ApplicationName { get; set; }
        public string Password { get; set; }
        public int SecurityLevelID { get; set; }
        public List<SecurityLevel> SecurityLevels { get; set; }
    }
}
