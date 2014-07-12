using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Framework.Objects.SalesForce
{
    public class SFLead
    {
        public string LeadID { get; set; }
        public Lead Lead { get; set; }
        public List<Lead_Q_A__c> QuestionAnswers { get; set; }
    }
}
