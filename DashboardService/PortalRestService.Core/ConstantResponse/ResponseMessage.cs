using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Core.ConstantResponse
{
    public static class RespnoseCode
    {
        public static readonly int OK = 200;
        public static readonly int Multiple_Choices = 300;
        public static readonly int Moved_Permanently = 301;
        public static readonly int Found = 302;
        public static readonly int Not_Modified = 304;
        public static readonly int Temporary_Redirect = 307;
        public static readonly int Bad_Request = 400;
        public static readonly int Unauthorized = 401;
        public static readonly int Forbidden = 403;
        //public static readonly string Recordfound = "Record found";
    }
    public static class RespnoseMessage
    {
        public static readonly string Record_found = "Record Found";
        public static readonly string Record_not_found = "Record Not Found";
        public static readonly string Record_Updated_Successfully = "Record updated successfully.";
        public static readonly string Record_Save_Successfully = "Record save successfully.";
        public static readonly string Record_Not_Saved = "Record not saved";
        public static readonly string Record_Not_Updated = "Record not updated";
        public static readonly string Deleted = "Deleted";
        public static readonly string Not_Deleted = "Not Deleted";
        public static readonly string Opeartion_Failed = "Opeartion failed!";
        public static readonly string Invalid_Data = "Invalid data";
        public static readonly string Exception = "Exception";
        public static readonly string Faild = "Faild!";
        public static readonly string Mapped_RFIdReaderId_is_not_exits = "Mapped RFIdReaderId is not exits.";
        public static readonly string Mapped_LocationID_is_not_exits = "Mapped LocationID is not exits.";
        public static readonly string Mapped_CableID_is_not_exits = "Mapped CableId is not exists";
        public static readonly string Record_status_changed_successfully = "Record status changed successfully.";
        public static readonly string Please_provide_Pad_Id_value = "Please provide Pad Id value.";
        public static readonly string Please_provide_RfId_Reader_Id_value = "Please provide RfId Reader Id value.";
        public static readonly string Please_provide_PowerCabinet_Id_value = "Please provide PowerCabinet Id value.";
        public static readonly string Please_provide_Modem_Id_value = "Please provide Modem Id value.";
        public static readonly string Please_provide_Vehicle_Rfid_card_Assigned = "Please provide Vehicle RfId Card Assigned.";
        public static readonly string Please_provide_Vehicle_ModelYearId = "Please provide Vehicle ModelYearid.";
        public static readonly string Please_provide_Vehicle_Id_value = "Please provide Vehicle Id value.";
        public static readonly string Please_provide_Vehicle_ModelId = "Please provide Vehicle ModelId.";
        public static readonly string Please_provide_ModifiedBy_value = "Please provide ModifiedBy value.";
        public static readonly string Cable_Created_Successfully = "Cable Created Successfully";
        public static readonly string Cable_Not_Created = "Cable Not Created";
        public static readonly string Pos_Not_Created = "Pos Not Created";
        public static readonly string VehicleMake_Not_Created = "VehicleMake Not Created";
        public static readonly string Price_Plan_Not_Created = "Price Plan Not Created";
        public static readonly string Subscription_Plan_Not_Created = "Subscription Plan Not Created";
        public static readonly string Model_not_Created = "Model not Created";
        public static readonly string Vehicle_not_Updated = "Vehicle not Updated";
        public static readonly string Vehicle_not_deleted = "Vehicle not Deleted";
        public static readonly string Make_Master_not_Created = "Make Master not Created";
        public static readonly string Make_Master_not_updated = "Make Master not updated";
        public static readonly string Make_Master_not_deleted = "Make Master not Deleted";
        public static readonly string Duplicate_AssetId_can = "Duplicate AssetId can not be created.";
        public static readonly string Dublicate_entry_for = "Dublicate entry for :";
        public static readonly string Issue_with_mapper = "Issue with mapper ";
        public static readonly string Please_provide_ChargeBox_Id = "Please provide ChargeBox Id";
    }
}