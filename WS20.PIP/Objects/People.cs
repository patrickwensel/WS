using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PIP.Objects
{
    [Serializable]
    public class People
    {
        public string FullName { get; set; }
        public string MultiFullName { get; set; }
        public decimal ObjectID { get; set; }

    }
}