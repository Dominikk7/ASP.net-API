using System;
using System.Collections.Generic;

namespace DataServerApi
{
    public class Verify
    {
        public static string checkLength(ref Dictionary<string, string> jsonMap, string key, int minLength, int maxLength)
        {
            string errorMessage = null;
            if (jsonMap.ContainsKey(key))
            {
                if (jsonMap.GetValueOrDefault(key).Length < minLength)
                {
                    errorMessage += key + " min length is: " + minLength + " characters";
                }
                else if (jsonMap.GetValueOrDefault(key).Length > maxLength)
                {
                    errorMessage += key + " max length is: " + maxLength + " characters";
                }
            }
            else
            {
                errorMessage += "missing data value in json. ";
            }


            return errorMessage;
        }
        public static string checkDateTime(ref Dictionary<string, string> jsonMap, string keyTime, string keyDate)
        {
            string errorMessage = null;

            //Split string mm/dd/yyyy
            string[] outputArrayDate = jsonMap.GetValueOrDefault(keyDate).Split("/");

            //Split string hh:mm
            string[] outputArrayTime = jsonMap.GetValueOrDefault(keyTime).Split(":");

            int month = 0;
            int day = 0;
            int year = 0;
            int hour = 0;
            int minute = 0;

            //Verify if can be converted to int

            //Month
            if (!Int32.TryParse(outputArrayDate[0], out month))
            {
                errorMessage += "Invalid characters in date. ";
            }
            //Day
            if (!Int32.TryParse(outputArrayDate[1], out day))
            {
                errorMessage += "Invalid characters in month. ";
            }
            //Year
            if (!Int32.TryParse(outputArrayDate[2], out year))
            {
                errorMessage += "Invalid characters in year. ";
            }
            //Hour
            if (!Int32.TryParse(outputArrayTime[0], out hour))
            {
                errorMessage += "Invalid characters in hour. ";
            }
            //Minute
            if (!Int32.TryParse(outputArrayTime[1], out minute))
            {
                errorMessage += "Invalid characters in minute. ";
            }

            //Verify validity
            try
            {
                DateTime tempTime = new DateTime(year, month, day, hour, minute, 0);
            }
            catch (Exception ex)
            {
                errorMessage += "Invalid date or time. ";
            }


            return errorMessage;
        }



    }
}
