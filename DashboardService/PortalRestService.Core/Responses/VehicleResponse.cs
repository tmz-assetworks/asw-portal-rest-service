using PortalRestService.Core.PagingHelper;

namespace PortalRestService.Core.Responses
{



    public class GetAllVehicleResponse
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public StatusList statusList { get; set; }
        public List<Vehicle> data { get; set; }
        public PaginationResponse paginationResponse { get; set; }

    }

    public class vehicleWithPagination
    {
        public string Active { get; set; }
        public string Inactive { get; set; }
        public List<Vehicle> data { get; set; }
        public PaginationResponse paginationResponse { get; set; }

    }

    public class StatusList
    {
        public string Type { get; set; }
        public int Count { get; set; }
        public List<StatusData> StatusData { get; set; }

    }


    public class Vehicle
    {
        public long id { get; set; }
        public string VIN { get; set; }
        public string ModelYear { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string LicencePlate { get; set; }
        public string Department { get; set; }
        public string DomicileLocation { get; set; }
        public string VehicleMacAddress { get; set; }
        public string RFIDCardAssigned { get; set; }
        public bool Status { get; set; }
    }

    public class AllVehicle
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string Active { get; set; }
        public string Inactive { get; set; }
        public List<GetAllVehicle> data { get; set; }
        public PaginationResponse paginationResponse { get; set; }

    }

    public class GetAllVehicle
    {


        public long Id { get; set; }
        public string VIN { get; set; }

        public string LicencePlate { get; set; }

        public string Department { get; set; }

        public string DomicileLocation { get; set; }

        public string VehicleMacAddress { get; set; }

        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
        public VehicleModelYear1 VehicleModelYear { get; set; }

        public VehicleModel1 VehicleModel { get; set; }

        public VehicleMake1 VehicleMake { get; set; }

        public ICollection<VehicleRFID1> VehicleRFID { get; set; }

    }

    public class VehicleModelYear1
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

    }

    public class VehicleModel1
    {

        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }

    public class VehicleMake1
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }

    public class VehicleRFID1
    {
        public long Id { get; set; }
        public long VehicleId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

    }

    public class GetAllVehicleRequest : QueryStringParameters
    {
        public string? opratorid { get; set; }
    }

}