using System;
using System.IO;
using System.Reflection;
using System.Text;
using Utility.Models;

namespace Utility
{
    public class Logger
    {
        public static void Log(MethodBase menthodBase)
        {
            try
            {
                if (menthodBase != null)
                {
                    Log(string.Format("{0}.{1}", menthodBase.ReflectedType.FullName, menthodBase.Name));
                }
            }
            catch(Exception e)
            {
                Log(MethodBase.GetCurrentMethod(), e);
            }
        }

        public static void Log(MethodBase methodBase,Exception exception)
        {
            Log(new Error(methodBase, exception));
        }

        public static void Log(MethodBase methodBase, string message)
        {
            Log(new Error(methodBase, message));
        }

        public static void Log(Error error)
        {
            try
            {
                if (!string.IsNullOrEmpty(error.MethodName))
                {
                    Log(error.MethodName);
                }
                foreach(var msg in error.ErrorMessageList)
                {
                    Log(msg);
                }
            }
            catch
            {

            }
        }

        public static void Log(string message)
        {
            try
            {
                using(var sw=new StreamWriter(Define.LogFile, true, Encoding.UTF8))
                {
                    sw.WriteLine(DateTimeHelper.DateTimeToDateTimeStringWithSeperator(DateTime.Now) + " " + message);
                    sw.Close();
                }
            }
            catch
            {

            }
        }

        public static void Seperator()
        {
            try
            {
                using (StreamWriter sw=new StreamWriter(Define.LogFile, true, Encoding.UTF8))
                {
                    sw.WriteLine("=========================================================");
                    sw.Close();
                }
            }
            catch
            {

            }
        }

        public static void SubSeperator()
        {
            try
            {
                using (StreamWriter sw=new StreamWriter(Define.LogFile, true, Encoding.UTF8))
                {
                    sw.WriteLine("---------------------------------------------------------");
                    sw.Close();
                }
            }
            catch
            {

            }
        }

        public static void TimeSpan(DateTime beginTime,DateTime finisedTime)
        {
            try
            {
                var ts = finisedTime - beginTime;
                using(StreamWriter sw=new StreamWriter(Define.LogFile, true, Encoding.UTF8))
                {
                    sw.WriteLine("TimeSpan=>" + ts.Hours.ToString().PadLeft(2, '0') + ":" + ts.Minutes.ToString().PadLeft(2, '0') + ":" + ts.Seconds.ToString().PadLeft(2, '0'));
                    sw.Close();
                }
            }
            catch
            {

            }
        }
    }
}
