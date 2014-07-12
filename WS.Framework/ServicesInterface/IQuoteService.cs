using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS.Framework.Objects;

namespace WS.Framework.ServicesInterface
{
    public interface IQuoteService
    {
        void AddNewQuote(RequestQuote requestQuote);

    }
}
