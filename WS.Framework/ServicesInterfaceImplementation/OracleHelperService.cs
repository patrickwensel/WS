using System.Collections.Generic;
using System.Linq;
using WS.Framework.Objects.Enums;
using WS.Framework.ServicesInterface;
using WS.Framework.WSJDEData;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class OracleHelperService : IOracleHelperService
    {
        public int GetNextSequenceValue(SequenceNumber sequenceNumber)
        {
            using (WSJDE context = new WSJDE())
            {
                IList<int> sequence;
                switch (sequenceNumber)
                {
                    case SequenceNumber.LeadToSFDC:
                        sequence =
                            context.Database.SqlQuery<int>("SELECT ws_lead_to_sfdc_lead_out.NEXTVAL FROM DUAL")
                                   .ToList();
                        return sequence[0];
                    case SequenceNumber.WorkOrder:
                        sequence =
                            context.Database.SqlQuery<int>("select pkg_jdeu.FN_NN ('48',1) from dual")
                                   .ToList();
                        return sequence[0];
                    case SequenceNumber.UniqueKeyPricing:
                        sequence =
                            context.Database.SqlQuery<int>("select pkg_jdeu.FN_NN ('17',5) from dual")
                                   .ToList();
                        return sequence[0];
                    case SequenceNumber.WorkOrderPart:
                        sequence =
                            context.Database.SqlQuery<int>("select pkg_jdeu.FN_NN_F00022('F3111') from dual")
                                   .ToList();
                        return sequence[0];
                    case SequenceNumber.Activity:
                                                sequence =
                            context.Database.SqlQuery<int>("select pkg_jdeu.FN_NN ('03B',8) from dual")
                                   .ToList();
                        return sequence[0];
                    default:
                        return 0;

                }
            }
        }

        public int RunBOARemittance(string filePath)
        {
            using (WSJDE context = new WSJDE())
            {
                IList<int> returnCode;

                returnCode = context.Database.SqlQuery<int>("select sisr.FN_PROCESS_BOA_REMITTANCE ('hi mom') from dual")
                           .ToList();
                return returnCode[0];

            }
        }
    }
}
