namespace PortalRestService.Core.Responses
{
    public class ChargerResponse
    {
        public ChargerResponse()
        {
            chargerData = new List<ChargerData>();
        }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public List<ChargerData> chargerData { get; set; }
    }

    public class ChargerData
    {
        public ChargerData()
        {
            StatusData = new List<StatusData>();
        }
        public string Type { get; set; }
        public int Count { get; set; }
        public List<StatusData> StatusData { get; set; }
    }
    public class StatusData
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Color { get; set; }
    }


    public class ChargingInfustructure
    {
        public string Key { get; set; }
        public int Value { get; set; }
    }

    public class SummaryDetail
    {
        public List<ChargingInfustructure> chargingInfustructure { get; set; }
        public List<Revenue> Revenue { get; set; }
        public List<EnergyUsed> EnergyUsed { get; set; }
        public List<EnergyPoint> EnergyPoints { get; set; }
    }

    public class EnergyPoint
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class EnergyUsed
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class Revenue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class SummaryData
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public List<SummaryDetail> Data { get; set; }
    }


    public class LocationsDispenserStatus
    {
        public long Id { get; set; }
        public string Status { get; set; }
    }


}

