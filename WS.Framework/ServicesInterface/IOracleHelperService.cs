using WS.Framework.Objects.Enums;

namespace WS.Framework.ServicesInterface
{
    public interface IOracleHelperService
    {
        int GetNextSequenceValue(SequenceNumber sequenceNumber);
    }
}
