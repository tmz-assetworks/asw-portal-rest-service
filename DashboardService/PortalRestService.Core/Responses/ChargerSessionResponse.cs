
namespace PortalRestService.Core.Responses
{
    public class ChargerSessionResponse
    {
        public ChargerSessionResponse()
        {
            Data = new List<ChargerSessionStatus>();
        }
        public int? StatusCode { get; set; }
        public string? StatusMessage { get; set; }
        public List<ChargerSessionStatus> Data { get; set; }

    }
    public class ChargerSessionStatus
    {
        public DateTime? ChargerTime { get; set; }
        public string? Status { get; set; }
        public string? ChargeBoxId { get; set; }
        public int? Count { get; set; }
    }

    public class ChargerSessionResponseSelectedRecord
    {
        public string? Start_Time { get; set; }
        public string? Charger_Status { get; set; }
        public string? Device_Id { get; set; }
    }
    public class ChargerSessionByLocationResponse
    {
        public ChargerSessionByLocationResponse()
        {
            data = new List<ChargingSessionByLocationBO>();
        }
        public int? StatusCode { get; set; }
        public string? StatusMessage { get; set; }
        public List<ChargingSessionByLocationBO> data { get; set; }

    }

    public class ChargerSessionRequest
    {
        public List<int> LocationIds { get; set; }
        public string? Duration { get; set; }
        public string? Opratorid { get; set; }

    }
    public class LocationOpratorRequest
    {
        public List<int> LocationIds { get; set; }
        public string? opratorid { get; set; }

    }
    public class LocationDispenserRequest
    {
        public List<long> LocationIds { get; set; }
        public string? opratorid { get; set; }

    }
    public class LocationPerformingRequest
    {
        public List<int> LocationIds { get; set; }
        public string? Duration { get; set; }
        public string? Opratorid { get; set; }
        public int Orderby { get; set; }

    }

    public class ChargerStatusResponse
    {
        public ChargerStatusResponse()
        {
            data = new List<ChargerByLocationBO>();
        }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<ChargerByLocationBO> data { get; set; }
    }

    public class ChargerByLocationBO
    {
        public long ChargerId { get; set; }
        public string ChargingStatus { get; set; }
        public DateTime? StartTime { get; set; }
        public string ChargeBoxId { get; set; }

        public string? times { get; set; }
    }
    public class ChargerByLocationChartBO
    {
        public string Color { get; set; }
        public long Counts { get; set; }
        public string ChargeStatus { get; set; }

        public string? times { get; set; }
    }
    public class ChargerStatusForChartResponse
    {
        public ChargerStatusForChartResponse()
        {
            data = new List<ChargerByLocationChartBO>();
        }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<ChargerByLocationChartBO> data { get; set; }
    }
    public class ChargingSession
    {

        public long Id { get; set; }
        public long ChargerId { get; set; }
        public int? ChargingCost { get; set; }
        public string ChargingStatus { get; set; }
        public int? ConnectorId { get; set; }
        public string DeviceId { get; set; }
        public string ReasonForStop { get; set; }
        public int? StartMeterValue { get; set; }
        public int? StartSoc { get; set; }
        public DateTime? StartTime { get; set; }
        public int? EndMeterValue { get; set; }
        public int? EndSoc { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }

    public class DispenserByLocationIdResponse
    {
        public DispenserByLocationIdResponse()
        {
            data = new List<DispenserByLocation>();
        }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<DispenserByLocation> data { get; set; }


    }

    public class DispenserByLocation
    {
        public long LocationId { get; set; }

        public string LocationName { get; set; }

        public string ContactPersonName { get; set; }

        public string AddressLine1 { get; set; }

        public string LocationStatusName { get; set; }

        public long LocationStatusId { get; set; }

        public string ChargeBoxId { get; set; }
        public long ChargerId { get; set; }
        public string SerialNumber { get; set; }


    }

    public class ChargingSessionByLocationBO
    {
        public long Id { get; set; }
        public long ChargerId { get; set; }
        public int? ChargingCost { get; set; }
        public string ChargingStatus { get; set; }
        public int? ConnectorId { get; set; }
        public string DeviceId { get; set; }
        public string ReasonForStop { get; set; }
        public int? StartMeterValue { get; set; }
        public int? StartSoc { get; set; }
        public DateTime? StartTime { get; set; }
        public int? EndMeterValue { get; set; }
        public int? EndSoc { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public long LocationId { get; set; }

        public string LocationName { get; set; }

        public string ContactPersonName { get; set; }

        public string AddressLine1 { get; set; }

        public string LocationStatusName { get; set; }

        public long LocationStatusId { get; set; }

        public string ChargeBoxId { get; set; }
        //public long ChargerId { get; set; }
        public string SerialNumber { get; set; }
        public string? times { get; set; }
    }

    //public class ChargerStatusResponse
    //{
    //    public ChargerStatusResponse()
    //    {
    //        data = new List<ChargerStatus>();
    //    }
    //    public int StatusCode { get; set; }
    //    public string StatusMessage { get; set; }
    //    public List<ChargerStatus> data { get; set; }
    //}

    //public class ChargerStatus
    //{
    //    public long ChargerId { get; set; }
    //    public string ChargingStatus { get; set; }
    //    public DateTime? StartTime { get; set; }
    //    public string ChargeBoxId { get; set; }
    //}
    public class MilesAddedByLocationRequest
    {
        public List<int> LocationIds { get; set; }
        public string? Duration { get; set; }
        public string? Opratorid { get; set; }


    }
}

