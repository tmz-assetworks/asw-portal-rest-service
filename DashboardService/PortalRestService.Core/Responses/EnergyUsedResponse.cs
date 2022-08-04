namespace PortalRestService.Core.Responses
{

    public class EnergyUsedResponse
    {
        public EnergyUsedResponse()
        {
            Data = new List<EnergyUsedStatus>();
        }
        public int? StatusCode { get; set; }
        public string? Message { get; set; }
        public List<EnergyUsedStatus> Data { get; set; }

    }
    public class EnergyUsedStatus
    {
        public DateTime? EnergyTime { get; set; }
        public int? EnergyUsed { get; set; }
    }
}
