using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Framework.ServicesInterface
{
    public interface IHyperionService
    {
        string RunGLExtract(string month, string year, string ledgerType);
    }
}
