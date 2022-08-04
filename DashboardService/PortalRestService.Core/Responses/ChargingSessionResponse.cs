namespace PortalRestService.Core.Responses
{

    public class ChargingSessionResponse
    {
        public ChargingSessionResponse()
        {
            Data = new List<ChargingSessionStatus>();
        }
        public int? StatusCode { get; set; }
        public string? Message { get; set; }
        public List<ChargingSessionStatus> Data { get; set; }

    }
    public class ChargingSessionStatus
    {
        public DateTime? ChargingTime { get; set; }
        public string? Status { get; set; }
        public string? ChargeBoxId { get; set; }
    }
}
