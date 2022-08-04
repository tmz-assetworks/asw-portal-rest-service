
using PortalRestService;
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
        public int dispenserStatusId { get; set; }
        public DispenserStatus dispenserStatus { get; set; }
        public string description { get; set; }
        public string endPointUrl { get; set; }
        public string firmwareVersion { get; set; }
        public string hardwareSerialNumber { get; set; }
        public bool isActive { get; set; }
        public bool isAutomatic { get; set; }
        public bool isDeviceExists { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public int makeMasterId { get; set; }
        public MakeMaster makeMaster { get; set; }
        public string meterType { get; set; }
        public int modelId { get; set; }
        public object model { get; set; }
        public bool multiplePorts { get; set; }
        public string pingSchedule { get; set; }
        public bool privateStation { get; set; }
        public string readingSchedule { get; set; }
        public string serialNumber { get; set; }
        public int locationId { get; set; }
        public Location location { get; set; }
        public int stationId { get; set; }
        public string chargeBoxId { get; set; }
        public string stationName { get; set; }
        public int vendorId { get; set; }
        public Vendor vendor { get; set; }
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
        public List<Location> data { get; set; }   //  Location Or Site
    }

    public class Location
    {

        public int Id { get; set; }
        public int LocationAddressId { get; set; }
        public LocationAddress LocationAddress { get; set; }
        public int LocationStatusId { get; set; }
        public LocationStatus LocationStatus { get; set; }
        public int LocationId { get; set; }
        public string ContactPersonName { get; set; }
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
        public List<LocationSchedule> LocationSchedule { get; set; }


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
}

public class LocationsDispenserDetails
{
        public long locationId { get; set; }
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



}
