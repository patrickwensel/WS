using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS.Framework.Objects.Emails;

namespace WS.Framework.ServicesInterface
{
    public interface IEmailService
    {
        void AttributeChanges(UpdateAttributesEmail updateAttributesEmail);
        bool BOATransaction(BOATransactionEmail boaTransactionEmail);
        void PaperlessRequest(PaperlessInvoiceEmail paperlessInvoiceEmail);
        void SafetyIncident(SafetyIncidentEmail safetyIncidentEmail);
        void HoursIT(HoursITEmail hoursItEmail);
    }
}
