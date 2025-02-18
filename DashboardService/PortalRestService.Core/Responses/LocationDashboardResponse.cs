using PortalRestService.Core.PagingHelper;

namespace PortalRestService.Core.Responses
{
    public class Data
    {
        public int Id { get; set; }
        public int LocationAddressId { get; set; }
        public LocationAddressDTO LocationAddress { get; set; }
        public int LocationStatusId { get; set; }
        public LocationStatus LocationStatus { get; set; }
        public string DepartmentName { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonNumber { get; set; }
        public string AlternateMobileNumber { get; set; }
        public string Email { get; set; }
        public string GlobalTax { get; set; }
        public string TotalCapacity { get; set; }
        public string UtilityService { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        //public int NetworkId { get; set; }
        // public string NetworkName { get; set; }
        public string LocationName { get; set; }
        public int LocationNumber { get; set; }
        //public int SubNetworkId { get; set; }
        // public string SubNetworkName { get; set; }
        public string FuelProtectType { get; set; }
        public string TimeZone { get; set; }
        public List<LocationSchedule> LocationSchedule { get; set; }
        public List<OperatorUserMapper>? OperatorUserMapper { get; set; }
    }

    public class GetLocatinByIdResponse
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public Data data { get; set; }

    }
    public class OperatorAlertRequest : QueryStringParameters
    {
        public List<int> LocationIds { get; set; }
        public string operatorId { get; set; }
        public List<string> chargerBoxIds { get; set; }
        public bool isRead { get; set; }
    }
    public class TaskCount
    {

        public int Counts { get; set; }

    }
    public class AlertResponse
    {

        public int EventLogId { get; set; }
        public string? ChargeBoxId { get; set; }
        public string? Category { get; set; }
        public string? MessageType { get; set; }
        public DateTime? DateTime { get; set; }
        public string? IPAddress { get; set; }
        public string LocationsName { get; set; }
        public string RequestPayload { get; set; }
        public string ResponsePayload { get; set; }
        public bool IsRead { get; set; }

        public string Flag { get; set; }
        public string UserId { get; set; }
        public string? AssetId { get; set; }


    }

    public class OperatorAlertResponse
    {
        public OperatorAlertResponse()
        {
            data = new List<AlertResponse>();
        }
        public int? StatusCode { get; set; }
        public string? StatusMessage { get; set; }
        public TaskCount TaskCount { get; set; }
        public List<AlertResponse> data { get; set; }

        public PaginationResponse paginationResponse { get; set; }
    }

    public class AllLocationQueryResponse
    {
        public AllLocationQueryResponse()
        {
            data = new List<LocationData>();
        }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public List<LocationData> data { get; set; }
    }

    public class LocationData
    {
        public long Id { get; set; }
        public string LocationName { get; set; }
    }

    public class StatusSummaryData
    {
        public string Type { get; set; }
        public int Count { get; set; }
        public List<StatusItemData> StatusData { get; set; }
    }

    public class StatusSummary
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public List<StatusSummaryData> data { get; set; }
    }

    public class StatusItemData
    {
        public string Key { get; set; }
        public int value { get; set; }
    }

    public class LocationStatusByLocationIdResponse
    {
        public string LocationName { get; set; }
        public string ContactPersonName { get; set; }

        public long LocationStatusId { get; set; }
        public string LocationStatusName { get; set; }


    }

    //public class LocationDashboardResponse
    //{

    //    public int? StatusCode { get; set; }
    //    public string? StatusMessage { get; set; }

    //    public List<LocationDashboard> data { get; set; }

    //}
    //public class LocationDashboard
    //{
    //    public LocationDashboard()
    //    {
    //        locationSchedule = new List<LocationSchedule>();
    //    }
    //    public long Id { get; set; }

    //    public long LocationAddressId { get; set; }

    //    public long LocationStatusId { get; set; }

    //    public long LocationId { get; set; }

    //    public string ContactPersonName { get; set; }

    //    public string GlobalTax { get; set; }

    //    public string TotalCapacity { get; set; }

    //    public string UtilityService { get; set; }

    //    public string CreatedBy { get; set; }

    //    public DateTime CreatedOn { get; set; }

    //    public string Description { get; set; }


    //    public string ModifiedBy { get; set; }

    //    public DateTime ModifiedOn { get; set; }

    //    public long NetworkId { get; set; }

    //    public string NetworkName { get; set; }

    //    public string LocationName { get; set; }

    //    public long LocationNumber { get; set; }

    //    public long SubNetworkId { get; set; }

    //    public string SubNetworkName { get; set; }

    //    public string TimeZone { get; set; }

    //    public string FuelProductType { get; set; }

    //    public LocationAddress LocationAddress { get; set; }

    //    public LocationStatus locationStatus { get; set; }

    //    public List<LocationSchedule> locationSchedule { get; set; }

    //}

    public partial class LocationAddressDTO
    {

        public long Id { get; set; }


        public string AddressLine1 { get; set; }


        public string AddressLine2 { get; set; }


        //public long CityId { get; set; }


        public string CityName { get; set; }


        public long CountryId { get; set; }


        public string CountryName { get; set; }


        public string CreatedBy { get; set; }


        public DateTime CreatedOn { get; set; }


        public bool IsActive { get; set; }


        public string LandlineNumber { get; set; }


        public double Latitude { get; set; }


        public double Longitude { get; set; }

        public string ModifiedBy { get; set; }


        public DateTime ModifiedOn { get; set; }


        public string PinCode { get; set; }


        public long StateId { get; set; }


        public string StateName { get; set; }

    }

    public partial class LocationStatus
    {


        public long Id { get; set; }


        public string LocationStatusName { get; set; }


        public string CreatedBy { get; set; }


        public DateTime CreatedOn { get; set; }


        public bool IsActive { get; set; }


        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }



    }

    public partial class LocationSchedule
    {


        public long Id { get; set; }


        public string Day { get; set; }


        public long LocationId { get; set; }
        //  public virtual Location Location { get; set; }



        public string StartTime { get; set; }


        public string EndTime { get; set; }


        public string CreatedBy { get; set; }


        public DateTime CreatedOn { get; set; }


        public bool IsActive { get; set; }


        public string ModifiedBy { get; set; }


        public DateTime ModifiedOn { get; set; }
        public bool IsOpenAlldays { get; set; }
    }

    public partial class OperatorUserMapper
    {
        public long Id { get; set; }

        public long LocationId { get; set; }

        public string UserName { get; set; }

        public string UserId { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
    }

    public partial class Department
    {
        public long Id { get; set; }

        public string DepartmentName { get; set; }

        public string ContactPersonName { get; set; }

        public string Address { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }


        public DateTime ModifiedOn { get; set; }
    }

    public class AllLocationStatusQueryResponse
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public List<LocationStatusData> data { get; set; }
    }

    public class LocationStatusData
    {
        public long Id { get; set; }
        public string LocationName { get; set; }

        public string LocationStatus { get; set; }
    }

    public class AllLocationStatusChartBO
    {

        public long Counts { get; set; }
        public string LocationStatus { get; set; }
        public string Color { get; set; }

    }

    public class LocationStatusQueryResponse
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public List<AllLocationStatusChartBO> data { get; set; }
    }
    public class LocationPerformingResponse
    {


        public int? MeterValue { get; set; }
        public string LocationName { get; set; }

        public string Orderby { get; set; }
        public string Color { get; set; }
    }
    public class LocationPerformingChartResponse
    {
        public LocationPerformingChartResponse()
        {
            data = new List<LocationPerformingResponse>();
        }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<LocationPerformingResponse> data { get; set; }
    }
    public class MilesAddedByLocationResponse
    {


        public double RangeAdded { get; set; }
        public string Times { get; set; }
        public string svalue { get; set; }


    }
    public class MilesAddedByLocationChartResponse
    {
        public MilesAddedByLocationChartResponse()
        {
            data = new List<MilesAddedByLocationResponse>();
        }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<MilesAddedByLocationResponse> data { get; set; }
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

    }
    public class EventLogLocation
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string DeviceId { get; set; }
        public string EventLogDataSource { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string RequestId { get; set; }
        public string RequestPayload { get; set; }
        public string RequestType { get; set; }
        public string ResponsePayload { get; set; }
        public string LocationId { get; set; }
        public string LocationName { get; set; }
        public bool IsRead { get; set; }

        public string RequestTypeColor { get; set; }
    }
    public class EventLogLocationResponse
    {
        public EventLogLocationResponse()
        {
            data = new List<EventLogLocation>();
        }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<EventLogLocation> data { get; set; }
        public PaginationResponse paginationResponse { get; set; }
    }
    public class EventLogLocationIsReadUpdat
    {
        public int Id { get; set; }

    }
    public class ChartDetailsList
    {
        public long Id { get; set; }
        public string ChargerName { get; set; }
        public string UID { get; set; }
        public string ChargerType { get; set; }
        public string FaultSince { get; set; }
        public string FaultDescription { get; set; }
        public DateTime? TimeReported { get; set; }

        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public string ChargeBoxId { get; set; }
        public string? ChargingStatus { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Startsoc { get; set; }
        public int? EndSoc { get; set; }
        public string? ReasoneForStop { get; set; }
        public string? Startmetervalue { get; set; }
        public string? Endmetervalue { get; set; }
        public string? LocationStatus { get; set; }
    }

    public class ChartDetailsListResponse
    {
        public ChartDetailsListResponse()
        {
            data = new List<ChartDetailsList>();
        }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<ChartDetailsList> data { get; set; }
        public PaginationResponse paginationResponse { get; set; }
    }

    public class ChargerSessionDetailsList
    {
        public long Id { get; set; }
        public string Sessionid { get; set; }
        public string Duration { get; set; }
        public double Usage { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ChargeBoxId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string ChargingStatus { get; set; }
        public int? Startsoc { get; set; }
        public int? EndSoc { get; set; }
        public string? ReasoneForStop { get; set; }
        public int? Startmetervalue { get; set; }
        public int? Endmetervalue { get; set; }
        public string? AssetId {  get; set; }


    }

    public class ChargerSessionDetailsListResponse
    {
        public ChargerSessionDetailsListResponse()
        {
            data = new List<ChargerSessionDetailsList>();
        }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<ChargerSessionDetailsList> data { get; set; }
        public PaginationResponse paginationResponse { get; set; }
    }
    public class CommandList
    {

        public long Id { get; set; }
        public string value { get; set; }
    }
    public class CommandListResponse
    {
        public CommandListResponse()
        {
            data = new List<CommandList>();
        }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<CommandList> data { get; set; }

    }
    public class ChargeBoxIDList
    {

        public long id { get; set; }
        public string chargeboxid { get; set; }
    }
    public class ChargeBoxIDListResponse
    {
        public ChargeBoxIDListResponse()
        {
            data = new List<ChargeBoxIDList>();
        }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<ChargeBoxIDList> data { get; set; }

    }

    public class SaveNotificationResponse
    {

        public int? StatusCode { get; set; }
        public string? StatusMessage { get; set; }
    }
    public class NotificationCommand
    {

        public int Id { get; set; }
        public string flag { get; set; }
    }
    public class LocationRequest
    {

        public int Id { get; set; }

    }

    public class OcppEventLogAndTaskNotificationRequest
    {
        public int EventLogId { get; set; }
        public string? Category { get; set; }
    }
}
