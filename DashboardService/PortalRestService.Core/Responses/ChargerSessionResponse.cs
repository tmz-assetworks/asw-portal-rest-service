
using PortalRestService.Core.PagingHelper;

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
        public string? chargerBoxId { get; set; }
        public string? Duration { get; set; }
        public string? Opratorid { get; set; }

    }
    public class LocationOpratorRequest
    {
        public List<int> LocationIds { get; set; }
        public string? ChargeBoxId { get; set; }
        public string? operatorid { get; set; }

    }
    public class LocationDispenserDetailRequest : QueryStringParameters
    {
        public List<long> LocationIds { get; set; }
        public string? opratorId { get; set; }

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
        public string? svalue { get; set; }
    }
    public class ChargerByLocationChartBO
    {
        public string Color { get; set; }
        public long Counts { get; set; }
        public string ChargeStatus { get; set; }

        public string? times { get; set; }

        public string? svalue { get; set; }
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
        public long DispenserId { get; set; }
        public string SerialNumber { get; set; }
        public string ConnectorType { get; set; }

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
        public double? StartMeterValue { get; set; }
        public int? StartSoc { get; set; }
        public DateTime? StartTime { get; set; }
        public double? EndMeterValue { get; set; }
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

        public string?  svalue { get; set; }
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
        public string? chargerBoxId { get; set; }
        public string? Opratorid { get; set; }


    }
    public class EventLogRequest : QueryStringParameters
    {
        public List<int> LocationIds { get; set; }

        public string? Opratorid { get; set; }

        public List<string> ChargerBoxIds { get; set; }

    }

    public class EventLogRequestBO
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
    public class OcppEventLogRequest
    {
        public List<string> chargerboxid { get; set; }
       

    }
    public class ChartDetailsListRequest : QueryStringParameters
    {
        public List<int> LocationIds { get; set; }
        public string? ChargeBoxId { get; set; }
        public string? Duration { get; set; }
        public string? Opratorid { get; set; }
        public string? Flag { get; set; }
        public string? Fromdate { get; set; }
        public string? Todate { get; set; }
        public List<string>? status { get; set; }

        public bool? IsExport { get; set; }
        public string? ChartType { get; set; }


    }
    public class ChargerSessionListRequest : QueryStringParameters
    {
        public List<string> chargerboxid { get; set; }
        public string? Fromdate { get; set; }
        public string? Todate { get; set; }
        public List<string>? status { get; set; }
    }
    public class ChargerInformationRequest
    {
        public string ChargeBoxId { get; set; }
        public string OperatorId { get; set; }
    }
    public class ChargerInformationResponse
    {
        public ChargerInformationResponse()
        {
            data = new ChargerInfo();
        }
        public int? StatusCode { get; set; }
        public string? StatusMessage { get; set; }
        public ChargerInfo data { get; set; }
    }
    public class ChargerInfo
    {
        public string HardwareSerialNumber { get; set; }
        public string? ChargeBoxId { get; set; }
        public string Charger { get; set; }
        public string ChargerType { get; set; }
        public string ChargerStatus { get; set; }
        public DateTime? InstalledDate { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string ConnectorIds { get; set; }
        public long ConnectorType { get; set; }       
    }
}

