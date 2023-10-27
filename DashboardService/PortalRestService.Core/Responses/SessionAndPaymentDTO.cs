using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Core.Responses
{
#pragma warning disable
    public class SessionAndPaymentDTO
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public SessionAndPaymentData sessionAndPaymentData { get; set; }
    }
    //public class SessionAndPaymentData
    //{
    //    //public string accountId { get; set; }
    //    public string assetId { get; set; }
    //    //public int assetInternalId { get; set; }
    //    //public string cardNumber { get; set; }
    //    //public string comments { get; set; }
    //    public string connectionType { get; set; }
    //    //public double cost { get; set; }
    //    public DateTime createdDateTime { get; set; }
    //    public string description { get; set; }
    //    public string employeeId { get; set; }
    //    //public string employeeName { get; set; }
    //    public DateTime endDateTime { get; set; }
    //    //public int? externalTransactionId { get; set; }
    //    public bool fromStock { get; set; }
    //    public int fuelingSeconds { get; set; }
    //    public string hoseId { get; set; }
    //    //public long hoseId { get; set; }
    //    //public int internalId { get; set; }
    //    //public int invoiceNumber { get; set; }
    //    public bool isReturn { get; set; }
    //    public bool isReturnable { get; set; }
    //    public DateTime issueDateTime { get; set; }
    //    //public string issueQty { get; set; }
    //    public double issueQty { get; set; }
    //    //public string locationDesc { get; set; }
    //    //public long locationId { get; set; }
    //    public string locationId { get; set; }
    //    //public string merchantAddress { get; set; }
    //    //public string merchantId { get; set; }
    //    //public string merchantName { get; set; }
    //    //public string merchantPostalCode { get; set; }
    //    public bool meter1Good { get; set; }
    //    public double meter1Reading { get; set; }
    //    public string meter1TypeId { get; set; }
    //    public bool meter2Good { get; set; }
    //    //public double meter2Reading { get; set; }
    //    public string meter2TypeId { get; set; }
    //    //public string poNumber { get; set; }
    //    public string productId { get; set; }
    //    public string productTypeId { get; set; }
    //    //public string referenceNumber { get; set; }
    //    //public string relatedUniqueId { get; set; }
    //    public DateTime startDateTime { get; set; }
    //    //public string stateProvince { get; set; }
    //    //public string tankId { get; set; }
    //    public double tax { get; set; }
    //    public double? unitCost { get; set; }
    //    //public string unitOfIssue { get; set; }
    //    //public string userData1 { get; set; }
    //    //public string userData2 { get; set; }
    //    //public string userData3 { get; set; }
    //    //public string userData4 { get; set; }
    //    //public string userData5 { get; set; }
    //    //public string userData6 { get; set; }
    //    public string vendorId { get; set; }
    //    public string vendorName { get; set; }
    //    public string _recordId { get; set; }
    //}

    public class SessionAndPaymentData
    {
        public string assetId { get; set; }
        public int assetInternalId { get; set; }
        public string connectionType { get; set; }
        public double? cost { get; set; }
        public DateTime createdDateTime { get; set; }
        public string description { get; set; }
        public string employeeNumber { get; set; }
        public DateTime endDateTime { get; set; }
        public bool fromStock { get; set; }
        public int fuelingSeconds { get; set; }
        public string hoseId { get; set; }
        public int internalId { get; set; }
        public int invoiceNumber { get; set; }
        public bool isReturn { get; set; }
        public bool isReturnable { get; set; }
        public DateTime issueDateTime { get; set; }
        public double issueQty { get; set; }
        public string locationId { get; set; }
        public bool meter1Good { get; set; }
        public double meter1Reading { get; set; }
        public string meter1TypeId { get; set; }
        public bool meter2Good { get; set; }
        public double meter2Reading { get; set; }
        public string meter2TypeId { get; set; }
        public string productId { get; set; }
        public string productTypeId { get; set; }
        public DateTime startDateTime { get; set; }
        public double tax { get; set; }
        public string vendorId { get; set; }
        public string vendorName { get; set; }
        public string _recordId { get; set; }
    }
}
