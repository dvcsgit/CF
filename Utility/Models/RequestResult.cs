using System;
using System.Reflection;

namespace Utility.Models
{
    public class RequestResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public Error Error { get; set; }

        public void Success()
        {
            IsSuccess = true;
            Message = string.Empty;
            Data = null;
            Error = null;
        }

        public void ReturnData(object data)
        {
            IsSuccess = true;
            Message = string.Empty;
            Data = data;
            Error = null;
        }

        public void ReturnData(object data,string message)
        {
            IsSuccess = true;
            Message = message;
            Data = data;
            Error = null;
        }

        public void ReturnSuccessMessage(string message)
        {
            IsSuccess = true;
            Message = message;
            Data = null;
            Error = null;
        }

        public void Failed()
        {
            IsSuccess = false;
            Message = string.Empty;
            Data = null;
            Error = null;
        }

        public void ReturnFailedMessage(string message)
        {
            IsSuccess = false;
            Message = message;
            Data = null;
            Error = null;
        }

        public void ReturnError(Error error)
        {
            IsSuccess = false;
            Message = error.ErrorMessage;
            Data = null;
            Error = error;
        }

        public void ReturnError(MethodBase methodBase,Exception exception)
        {
            var error = new Error(methodBase, exception);

            IsSuccess = false;
            Message = error.ErrorMessage;
            Data = null;
            Error = error;
        }
    }
}
