using PortalRestService;
using PortalRestService.Core.PagingHelper;

namespace PortalRestService.Core.Responses
{

    public class CardDataResponse
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public List<CardData> data { get; set; }
    }

    // Dashboard Cards
    public class CardData
    {
        public string Type { get; set; }
        public int Count { get; set; }
        public List<StatusData> StatusData { get; set; }
    }
    public class DispenserResponse
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<Dispenser> data { get; set; }   //  Dispenser = Charger
    }
    public class Dispenser
    {
        public int id { get; set; }
        public string assetId { get; set; }
        public string description { get; set; }
        public string endPointUrl { get; set; }
        public string firmwareVersion { get; set; }
        public string hardwareSerialNumber { get; set; }
        public bool isActive { get; set; }
        public bool isAutomatic { get; set; }
        public bool isDeviceExists { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string makeName { get; set; }
        public string meterType { get; set; }
        public string modelName { get; set; }
        public bool multiplePorts { get; set; }
        public string pingSchedule { get; set; }
        public bool privateStation { get; set; }
        public string readingSchedule { get; set; }
        public string serialNumber { get; set; }
        public int locationId { get; set; }
        public LocationDTO? LocationDTO { get; set; }
        public int stationId { get; set; }
        public string chargeBoxId { get; set; }
        public string stationName { get; set; }
        public int vendorId { get; set; }
        public string? SimCardMSIDN { get; set; }
        public string? OEMOrderNumber { get; set; }
        public Vendor vendor { get; set; }
        public List<PortDTO> Ports { get; set; }
        public DateTime? InstallationDate { get; set; }
        public List<ChargerStatusDTO> ChargerStatus { get; set; }
        public LocationAddressDTO LocationAddress { get; set; }
    }
    public partial class ChargerStatusDTO
    {
        public int Id { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public string ContactPersonName { get; set; }
        public string AddressLine1 { get; set; }

        public string LocationStatusName { get; set; }
        public int? ChargerId { get; set; }
        public long LocationStatusId { get; set; }

        public string ChargeBoxId { get; set; }
        public string ChargerStatus1 { get; set; }
        public int? ConnectorId { get; set; }
        public string ConnectorStatus { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedoN { get; set; }
        public int? ReservationId { get; set; }
        public DateTime? ReservationExpiryDate { get; set; }
        public string IdTag { get; set; }
    }
    public class PortDTO
    {
        public long Id { get; set; }
        public long DispenserId { get; set; }
        public int ConnectorId { get; set; }
        public string CreatedBy { get; set; }
        public ConnectorDTO? ConnectorDTO { get; set; }
        public DateTime CreatedOn { get; set; }
        public string IncrementalPower { get; set; }
        public bool IsActive { get; set; }
        public string MaxPower { get; set; }
        public string MinPower { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string PortName { get; set; }
        public string Power { get; set; }
        public long ConnectorType { get; set; }
        public ChargerTypeDTO? ChargerTypeDTO { get; set; }
        public long ChargerTypeId { get; set; }
    }
    public class ChargerTypeDTO
    {
        public long Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ChargerTypeName { get; set; }
    }

    public class ConnectorDTO
    {
        public long Id { get; set; }
        public string CreatedBy { get; set; }
        public string ConnectorType { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }

    public class Vendor
    {
        public string createdBy { get; set; }
        public DateTime createdOn { get; set; }
        public int id { get; set; }
        public bool isActive { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedOn { get; set; }
        public int vendorId { get; set; }
        public string vendorName { get; set; }
    }
    public class MakeMaster
    {
        public int id { get; set; }
        public string name { get; set; }
        public object description { get; set; }
        public bool isActive { get; set; }
        public object createdBy { get; set; }
        public DateTime createdOn { get; set; }
        public object modifiedBy { get; set; }
        public DateTime modifiedOn { get; set; }
    }

    public class DispenserStatus
    {
        public int id { get; set; }
        public string dispenserStatusName { get; set; }
        public string createdBy { get; set; }
        public DateTime createdOn { get; set; }
        public bool isActive { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedOn { get; set; }
    }
    public class Locations
    {
        public int Commissioned { get; set; }
        public int UnderMaintenance { get; set; }
        public int UpComming { get; set; }
        public int Uncommisioned { get; set; }
        public int Decommissioned { get; set; }
        public int Installed { get; set; }
    }
    public class LocationResponse
    {
        public int StatusCode { get; set; }
        public string? StatusMessage { get; set; }
        public List<LocationDTO> data { get; set; }   //  Location Or Site
    }

    public class LocationDTO
    {
        public int Id { get; set; }
        //public int LocationAddressId { get; set; }
        public LocationAddressDTO LocationAddress { get; set; }
        public int LocationStatusId { get; set; }
        public LocationStatus LocationStatus { get; set; }
        public string LocationId { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonNumber { get; set; }
        public string GlobalTax { get; set; }
        public string TotalCapacity { get; set; }
        public string UtilityService { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int NetworkId { get; set; }
        public string NetworkName { get; set; }
        public string LocationName { get; set; }
        public int LocationNumber { get; set; }
        public int SubNetworkId { get; set; }
        public string SubNetworkName { get; set; }
        public string TimeZone { get; set; }
        public List<LocationSchedule>? LocationSchedule { get; set; }
        public List<OperatorUserMapper>? OperatorUserMapper { get; set; }
    }
    public class TotalLocationAndChargerResponse
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public int TotalLocations { get; set; }
        public int TotalDispenser { get; set; }// Charger
    }

    public class LocationsDispenserformapResponce
    {
        public LocationsDispenserformapResponce()
        {
            data = new List<LocationsDispenser>();
        }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<LocationsDispenser> data { get; set; }
    }

    public class LocationsDispenser
    {
        public long locationId { get; set; }
        public long DispenserId { get; set; }
        public string LocationName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string status { get; set; }
        public string ChargeBoxid { get; set; }
        public string? AssetId { get; set; }
        public string? MakeName { get; set; }
        public string? ModelName { get; set; }
    }

    public class LocationsDispenserDetailsResponce
    {
        public LocationsDispenserDetailsResponce()
        {
            data = new List<LocationsDispenserDetails>();
        }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public List<LocationsDispenserDetails> data { get; set; }

        public PaginationResponse paginationResponse { get; set; }

    }

    public class LocationsDispenserDetails
    {
        public long Id { get; set; }
        public string? LocationId { get; set; }
        public long DispenserId { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public string status { get; set; }
        public string NoofPort { get; set; }
        public string Available { get; set; }
        public string Connected { get; set; }
        public string Faulted { get; set; }
        public string ContactNo { get; set; }
        public string ContactName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public class LocationsDispenserpResponce
    {
        public LocationsDispenserpResponce()
        {
            data = new List<LocationsDispenser>();
        }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<LocationsDispenser> data { get; set; }
    }
    public class DispenserByLocations
    {
        public long DispenserId { get; set; }
        public long LocationId { get; set; }

        public string LocationName { get; set; }
        // public string DispenserName { get; set; }
        public string ContactPersonName { get; set; }

        public string AddressLine1 { get; set; }

        public string LocationStatusName { get; set; }

        public long LocationStatusId { get; set; }

        public string ChargeBoxId { get; set; }

        public string SerialNumber { get; set; }

        public string DispenserMake { get; set; }
        public string ProtocolName { get; set; }
        public string ChargerStatus { get; set; }
        public string DispenserModel { get; set; }
        public string ConnectorType { get; set; }
        public string NoofPort { get; set; }


    }
}
