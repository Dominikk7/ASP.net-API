using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DataServerApi
{
    public class Utils
    {

        private static string getTimeStamp(string date, string time)
        {
            //Split string mm/dd/yyyy
            string[] outputArrayDate = date.Split("/");

            //Split string hh:mm
            string[] outputArrayTime = time.Split(":");

            int month = Int32.Parse(outputArrayDate[0]);
            int day = Int32.Parse(outputArrayDate[1]);
            int year = Int32.Parse(outputArrayDate[2]);
            int hour = Int32.Parse(outputArrayTime[0]);
            int minute = Int32.Parse(outputArrayTime[1]);

            DateTime tempTime = new DateTime(year, month, day, hour, minute, 0);
            //tempTime.AddMinutes((key-1)*15);
            string timestamp = ((DateTimeOffset)tempTime).ToUnixTimeSeconds().ToString();

            return timestamp;
        }
        public static string verifyData(ref Dictionary<string, string> jsonMap)
        {

            string errorMessage = null;

            //Verify Lengths
            errorMessage += Verify.checkLength(ref jsonMap, "eventName", 1, 100);
            errorMessage += Verify.checkLength(ref jsonMap, "date", 10, 10);
            errorMessage += Verify.checkLength(ref jsonMap, "startTime", 5, 5);
            errorMessage += Verify.checkLength(ref jsonMap, "endTime", 5, 5);
            errorMessage += Verify.checkLength(ref jsonMap, "description", 0, 200);

            //Verify Validity
            if (errorMessage.Length == 0)
            {
                errorMessage += Verify.checkDateTime(ref jsonMap, "startTime", "date");
                errorMessage += Verify.checkDateTime(ref jsonMap, "endTime", "date");

            }

            //Type

            //coordinates


            Console.WriteLine(errorMessage);
            return errorMessage;

        }
        public static MapEvent parseMapEvent(ref Dictionary<string, string> jsonMap)
        {
            //Get current timestamp
            DateTime currentTime = new DateTime();
            currentTime = DateTime.Now;

            //Parse map to MapEvent
            int id = 0;
            string eventName = jsonMap.GetValueOrDefault("eventName");
            string eventType = "not_set";
            string coordinates = "not_set";
            string startDate = getTimeStamp(jsonMap.GetValueOrDefault("date"), jsonMap.GetValueOrDefault("startTime"));
            string endDate = getTimeStamp(jsonMap.GetValueOrDefault("date"), jsonMap.GetValueOrDefault("endTime"));
            string description = jsonMap.GetValueOrDefault("description");
            string timestamp = ((DateTimeOffset)currentTime).ToUnixTimeSeconds().ToString();

            MapEvent tempEvent = new MapEvent(id, eventName, eventType, coordinates, startDate, endDate, description, timestamp);

            return tempEvent;
        }
        public static void mapEventToDb(ref MapEvent currentEvent)
        {

            string accessCommand = "INSERT INTO `test_schema`.`data_table` (`index`, `eventName`, `eventType`, `coordinates`, `startDate`, `endDate`, `description`, `timestamp`) VALUES ('"
                + currentEvent.id + "', '" + currentEvent.eventName + "', '" + currentEvent.eventType + "', '" + currentEvent.coordinates + "', '" + currentEvent.startDate + "', '" +
                currentEvent.endDate + "', '" + currentEvent.description + "', '" + currentEvent.timestamp + "');";

            dbCommand(accessCommand, "test_schema");

            //Get MapEvent properties
            PropertyInfo[] variableNames = typeof(MapEvent).GetProperties();

            //Loop through each property
            for (int i = 0; i < variableNames.Length; i++)
            {

                //typeof(MapEvent).GetField(variableNames[i].Name).SetValue(tempEvent, 1);
                Console.WriteLine(variableNames[i].Name);
                //Console.WriteLine("Test: "+tempEvent.id);
            }
        }


        public static string getCurrentTimestamp()
        {
            DateTime tempTime = DateTime.Now;
            string timestamp = ((DateTimeOffset)tempTime).ToUnixTimeSeconds().ToString();
            return timestamp;
        }
        public static void parseJson(ref string json, ref Dictionary<string, string> jsonMap, bool debug)
        {
            //cmd test
            //curl -X POST -H "Content-Type: application/json" -d "{\"appID\":\"dominikSiteData\",\"ip\":\"test5\" }" http://api.dkapps.tk/api -i

            if (debug)
            {
                Console.WriteLine("Parse Start");
            }
            //{"firstParam":"yourValue","secondParam":"yourOtherValue"}
            //Remove brackets
            json = json.Replace("{", "");
            json = json.Replace("}", "");

            //Split by comma
            string[] outputArray = json.Split(",");
            foreach (string line in outputArray)
            {
                //Console.WriteLine("Line: "+line);
                //Split by ":"
                string[] keyValue = line.Split("\":\"");
                //Console.WriteLine("Keyvalue length: "+ keyValue.Length+": "+ keyValue[0]);
                if (keyValue.Length > 1)
                {
                    //Remove quotes
                    keyValue[0] = keyValue[0].Replace("\"", "");
                    keyValue[1] = keyValue[1].Replace("\"", "");

                    jsonMap.Add(keyValue[0], keyValue[1]);
                    if (debug)
                    {
                        Console.WriteLine(keyValue[0] + ":" + keyValue[1]);
                    }

                }
            }
            if (debug)
            {
                Console.WriteLine("Parse Complete");
            }

        }
        public static string buildInsertCommand(ref Dictionary<string, string> commandMap, string tableName)
        {
            //String setup
            string insertPre = "INSERT INTO `test_schema`.`" + tableName + "` (";
            string insertBody = "";
            string insertPost = ") ";


            string valuePre = "VALUES (";
            string valueBody = "";
            string valuePost = ");";

            //Build insert and value body
            foreach (var element in commandMap)
            {
                insertBody += ("`" + element.Key + "`, ");
                valueBody += ("\"" + element.Value + "\", ");
            }
            //Remove last comma and space
            insertBody = insertBody.Substring(0, insertBody.Length - 2);
            valueBody = valueBody.Substring(0, valueBody.Length - 2);



            //Final string
            return (insertPre + insertBody + insertPost + valuePre + valueBody + valuePost);
        }
        public static void dbCommand(string command, string tableName)
        {
            //Connect to DB
            Console.WriteLine("start sql builder");
            MySqlConnectionStringBuilder conn_string = new MySqlConnectionStringBuilder();
            conn_string.Port = Secrets.port;
            conn_string.Server = Secrets.ip;
            conn_string.UserID = Secrets.userId;
            conn_string.Password = Secrets.password;
            conn_string.Database = Secrets.database;
            conn_string.SslMode = MySqlSslMode.Required;

            try
            {

                using (MySqlConnection sqlCon = new MySqlConnection(conn_string.ToString()))
                {
                    sqlCon.Open();
                    Console.WriteLine("opened");

                    Console.WriteLine("Command: " + command);
                    MySqlCommand SelectCommandAccess = new MySqlCommand(command, sqlCon);
                    SelectCommandAccess.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


    }
}
