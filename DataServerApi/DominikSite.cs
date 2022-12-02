using DataServerApi;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DataServerApi
{
    //Map Event Object
    public class DominikSite
    {
        //JSON to DB translation
        const string dbName = "dominikSiteData";
        Dictionary<string, string> jsonToDB = new Dictionary<string, string>() { { "appID", "" }, { "ip", "ip" } };

        //Command and validity
        Dictionary<string, string> commandMap = new Dictionary<string, string>();
        public bool valid = true;

        public DominikSite(ref Dictionary<string, string> jsonMap)
        {
            //Add timestamp
            commandMap.Add("timestamp", Utils.getCurrentTimestamp());

            //Translate JSON to DB command
            foreach (var element in jsonToDB)
            {
                if (element.Value != "")
                {
                    //Check if required key is in json
                    if (jsonMap.ContainsKey(element.Key))
                    {
                        //Basic command check
                        string dbData = jsonMap[element.Key];

                        //Escape character
                        //dbData = "\\" + dbData;

                        commandMap.Add(element.Value, dbData);
                    }
                    else
                    {
                        this.valid = false;
                    }
                }
            }
        }

        public void insert()
        {
            //Build command
            string command = Utils.buildInsertCommand(ref this.commandMap, dbName);
            Console.WriteLine(command);
            //Execute command
            Utils.dbCommand(command, dbName);
        }

    }
}
