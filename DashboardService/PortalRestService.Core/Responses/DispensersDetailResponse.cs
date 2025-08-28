using PortalRestService.Core.PagingHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Core.Responses
{

    // For Chargers Details Grid
    /// <summary>
    /// Auther: Pradeep Date: 08/08/2022
    /// </summary>

    public class DispensersDetailResponse
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public List<DispensersDetail> data { get; set; }
        public PaginationResponse paginationResponse { get; set; }
    }
    public class LocationDispensersRequest : QueryStringParameters
    {
        public List<long> locationIds { get; set; }
    }
    public class DispensersDetail
    {

        public string ChargerName { get; set; }
        public string ChargerBoxId { get; set; }
        public string ChargerType { get; set; }
        public string FaultSince { get; set; }
        public string LocationContactName { get; set; }
        public string TimeReported { get; set; }
        public long LocationId { get; set; }
        public string State { get; set; }
        public string LocationContactNumber { get; set; }
        public string? AssetId { get; set; }
        public string? SimCardMSIDN { get; set; }
        public string? ModelName { get; set; }
        public string? MakeName { get; set; }
    }
    public class DispensersDetailRequest : QueryStringParameters
    {
        public string operatorId { get; set; }
    }
}

