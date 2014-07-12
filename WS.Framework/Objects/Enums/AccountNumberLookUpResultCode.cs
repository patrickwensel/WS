namespace WS.Framework.Objects.Enums
{
    public enum AccountNumberLookUpResultCode
    {
        FoundRecords,
        DidNotFindRecords,
        DidNotFindRecordsWithinSetDays,
        MoreThanOneAccountNumber,
        WriteOff,
        NotValidDocumentType,
        NonUSorCA,
        GeneralError,
        ChildOfGroupBill
    }
}
