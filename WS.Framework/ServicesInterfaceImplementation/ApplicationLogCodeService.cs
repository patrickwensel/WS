using System;
using NLog;
using WS.Framework.Objects.Enums;
using WS.Framework.ServicesInterface;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class ApplicationLogCodeService : IApplicationLogCodeService
    {
        //private static Logger logger = LogManager.GetCurrentClassLogger();

        //public string GetDisplayMessageAndAddLog(int codeID, LogType logType, string message)
        //{
        //    string displayMessage = "";

        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {
        //            ApplicationLogCode applicationLogCode = (from alc in context.ApplicationLogCodes
        //                                                     where alc.ID == codeID
        //                                                     select alc).FirstOrDefault();

        //            if (applicationLogCode != null)
        //            {
        //                ApplicationLog applicationLog = new ApplicationLog
        //                    {
        //                        ApplicationID = applicationLogCode.ApplicationID,
        //                        LogLevel = logType.ToString().ToUpper(),
        //                        Message = message,
        //                        LogCodeID = codeID
        //                    };

        //                context.ApplicationLogs.Add(applicationLog);
        //                context.SaveChanges();

        //                displayMessage = applicationLogCode.DisplayMessage;

        //                if (logType == LogType.Error)
        //                {
        //                    logger.Error("Application Log: " + applicationLog.ID);
        //                }
        //                if (logType == LogType.Fatal)
        //                {
        //                    logger.Fatal("Application Log: " + applicationLog.ID);
        //                }

        //            }
        //            else
        //            {
        //                ApplicationLog applicationLog = new ApplicationLog
        //                {
        //                    LogCodeID = 2,
        //                    LogLevel = "ERROR"
        //                };

        //                context.ApplicationLogs.Add(applicationLog);
        //                context.SaveChanges();

        //                ApplicationLogCode noLogCode = (from alc in context.ApplicationLogCodes
        //                                                where alc.ID == 2
        //                                                select alc).FirstOrDefault();

        //                displayMessage = noLogCode.DisplayMessage;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //    }

        //    return displayMessage;
        //}


    }
}
