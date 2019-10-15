using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class DateTimeHelper
    {
        public static string DateStringToDateStirngWithSeparator(string input)
        {
            string output = string.Empty;
            try
            {
                output = DateStringToDateTime(input).Value.ToString(Define.DateTimeFormat_DateStringWithSeperator);
            }
            catch
            {
                output = string.Empty;
            }
            return output;
        }

        public static DateTime? DateStringToDateTime(string input)
        {
            DateTime? output = null;
            try
            {
                output = new DateTime(int.Parse(input.Substring(0, 4)), int.Parse(input.Substring(4, 2)), int.Parse(input.Substring(6, 2)));
            }
            catch
            {
                output = null;
            }
            return output;
        }

        public static string DateTimeToDateTimeStringWithSeperator(DateTime? input)
        {
            string output = string.Empty;
            try
            {
                output = string.Format("{0} {1}", DateTimeToDateStringWithSeperator(input), DateTimeToTimeStringWithSeperator(input));
            }
            catch
            {
                output = string.Empty;
            }
            return output;
        }

        public static string DateTimeToDateString(DateTime? dateTime)
        {
            string result = string.Empty;
            try
            {
                result = dateTime.Value.ToString(Define.DateTimeFormat_DateString);
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }

        public static string DateTimeToDateStringWithSeperator(DateTime? Input)
        {
            string output = string.Empty;

            try
            {
                output = Input.Value.ToString(Define.DateTimeFormat_DateStringWithSeperator);
            }
            catch
            {
                output = string.Empty;
            }

            return output;
        }

        public static string DateTimeToTimeStringWithSeperator(DateTime? Input)
        {
            string output = string.Empty;

            try
            {
                output = Input.Value.ToString(Define.DateTimeFormat_TimeStringWithSeperator);
            }
            catch
            {
                output = string.Empty;
            }

            return output;
        }
    }
}
