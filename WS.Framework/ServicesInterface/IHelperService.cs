using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Web.Mvc;

namespace WS.Framework.ServicesInterface
{
    public interface IHelperService
    {
        DateTime JDEDateToDateTime(int jdeDate);
        int DateTimeToJDEDate(DateTime dateTime);
        int DateTimeToJDETime(DateTime dateTime);
        void LogDbEntityValidationException(DbEntityValidationException dbEx);
        List<SelectListItem> GetSelectListStates();
        string FormatUnitNumber(string unitNumber);
    }
}
