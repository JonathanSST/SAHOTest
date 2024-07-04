using System;

namespace SahoAcs.DBModel
{
    public class Itinerary
    {

        public string StateDesc { get; set; }
        public DateTime CardTime { get; set; }
        public Int32? LogStatus { get; set; }
        public String PsnNo { get; set; }
        public String PsnName { get; set; }
        public String Note { get; set; }
        public Double? Distance { get; set; }
        public Decimal NowRecordID { get; set; }
        public Decimal LastRecordID { get; set; }
        public Int32? TimeZone { get; set; }
        public DateTime? LocalTime { get; set; }
        public double Oplatitude { get; set; }
        public double OpLongitude { get; set; }
        public string CardDate { get; set; }
        public double ItinAmount { get; set; }
        public double Amount { get; set; }
        public string OrgName { get; set; }
        public string TravelMode { get; set; }

    }
}