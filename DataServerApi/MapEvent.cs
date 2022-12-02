namespace DataServerApi
{

    //Map Event Object
    public class MapEvent
    {
        public MapEvent(int id, string eventName, string eventType, string coordinates, string startDate, string endDate, string description, string timestamp)
        {
            this.id = id;
            this.eventName = eventName;
            this.eventType = eventType;
            this.coordinates = coordinates;
            this.startDate = startDate;
            this.endDate = endDate;
            this.description = description;
            this.timestamp = timestamp;
        }
        public int id { get; }
        public string eventName { get; }
        public string eventType { get; }
        public string coordinates { get; }
        public string startDate { get; }
        public string endDate { get; }
        public string description { get; }
        public string timestamp { get; }


    }
}
