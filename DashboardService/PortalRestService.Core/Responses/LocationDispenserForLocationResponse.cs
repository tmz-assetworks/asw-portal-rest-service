using PortalRestService.Core.PagingHelper;

namespace PortalRestService.Core.Responses
{
    public class LocationDispenserForLocationResponse
    {
        public LocationDispenserForLocationResponse()
        {
            data = new List<LocationDispenserForLocation>();
        }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public List<LocationDispenserForLocation> data { get; set; }
        public PaginationResponse paginationResponse { get; set; }
    }

    public class LocationDispenserForLocation
    {
        public int Id { get; set; }
        public string? AssetId { get; set; }
        public long locationId { get; set; }
        public int DispenserId { get; set; }
        public string ChargeBoxId { get; set; }
        public string? DispenserMake { get; set; }
        public string SerialNumber { get; set; }
        public string ProtocolName { get; set; }
        public string ChargerStatus { get; set; }
        public long DispenserStatusId { get; set; }
        public string? DispenserModel { get; set; }
        public string ConnectorType { get; set; }
        public string NoofPort { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
