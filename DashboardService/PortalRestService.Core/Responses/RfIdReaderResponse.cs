using PortalRestService.Core.PagingHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace PortalRestService.Core.Responses
{
     
    public class RfIdReaderResponse 
    {
        public int StatusCode;
        public string StatusMessage;
        public List<RFIDReaderDetails> data { get; set; }
        public PaginationResponse paginationResponse { get; set; }

    }

    public class RFIDReaderDetails
    {
        public long Id { get; set; }
        public string AssetId { get; set; }
        public string CardReader { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public long MakeId { get; set; }
        public long ModelId { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public long NetworkId { get; set; }
        public string NetworkName { get; set; }
        public long SerialNumber { get; set; }
        public long StatusId { get; set; }
        public Status Status { get; set; }
        public long SubNetworkId { get; set; }
        public string SubNetworkName { get; set; }
        public long WarrantyDuration { get; set; }
        public DateTime WarrantyExpiryDate { get; set; }
        public long LocationId { get; set; }
        public virtual LocationDTO Location { get; set; }
        public DateTime WarrantyStartDate { get; set; }
    }
    public class Status
    {
        public long Id { get; set; }
        public string StatusName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

    }

    public class RfIdReaderRequest : QueryStringParameters
    {
        public string operatorId { get; set; }
    }

    public class RfIdReaderDetailsResponse
    {
        public int StatusCode;
        public string StatusMessage;
        public RFIDReaderDetails data { get; set; }
    }
}
