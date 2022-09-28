using PortalRestService.Core.PagingHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public long locationId { get; set; }
        public long DispenserId { get; set; }
        public string ChargeBoxId { get; set; }
        //public string DispenserName { get; set; }
        public string DispenserMake { get; set; }
        public string SerialNumber { get; set; }
        public string ProtocolName { get; set; }
        //public string ConnnectorType { get; set; }   
        public string ChargerStatus { get; set; }
        // public string ChargerPort { get; set; }

        public long DispenserStatusId { get; set; }
        public string DispenserModel { get; set; }
        public string ConnectorType { get; set; }
        public string NoofPort { get; set; }


    }
}
