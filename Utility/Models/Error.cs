using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Utility.Models
{
    public class Error
    {
        public string OccurTime { get; private set; }

        private MethodBase MethodBase { get; set; }

        public string MethodName
        {
            get
            {
                if (MethodBase != null)
                {
                    return string.Format("{0}.{1}", MethodBase.ReflectedType.FullName, MethodBase.Name);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private Exception Exception { get; set; }

        public List<string> ErrorMessageList
        {
            get
            {
                var errorMessageList = new List<string>();
                var exception = Exception;
                while (exception != null && !string.IsNullOrEmpty(exception.Message))
                {
                    errorMessageList.Add(exception.Message);
                    exception = exception.InnerException;
                }
                return errorMessageList;
            }
        }

        public string ErrorMessage
        {
            get
            {
                if (ErrorMessageList != null && ErrorMessageList.Count > 0)
                {
                    var sb = new StringBuilder();
                    foreach (var errorMessage in ErrorMessageList)
                    {
                        sb.Append(errorMessage);
                        sb.Append("、");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    return sb.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public Error(MethodBase methodBase, Exception exception)
        {
            OccurTime = DateTimeHelper.DateTimeToDateTimeStringWithSeperator(DateTime.Now);
            MethodBase = methodBase;
            Exception = exception;
        }

        public Error(MethodBase methodBase,string errorMessage)
        {
            OccurTime = DateTimeHelper.DateTimeToDateTimeStringWithSeperator(DateTime.Now);
            MethodBase = methodBase;
            Exception = new Exception(errorMessage);
        }

    }
}