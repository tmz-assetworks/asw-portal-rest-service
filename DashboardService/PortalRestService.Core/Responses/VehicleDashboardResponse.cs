namespace PortalRestService.Core.Responses
{


    public class VehiclesResponse
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public VehicleByIdData data { get; set; }
    }
    public class VehicleResponse
    {

        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public VehicleData data { get; set; }

    }
    public class VehicleByIdData
    {
        public string VIN { get; set; }
        public string department { get; set; }

        public string licencePlate { get; set; }

        public string domicileLocation { get; set; }

        public string vehicleMacAddress { get; set; }

        public string Make { get; set; }

        public long ModelYear { get; set; }

        public string Model { get; set; }
        public bool Status { get; set; }       

       public string rfId { get; set; }
        public List<ApplicableSubscriptionPlan> applicableSubscriptionPlans { get; set; }

    }
    public class ApplicableSubscriptionPlan
    {
        public string SubscriptionPlanName { get; set; }
        public string Type { get; set; }
        public string SubscriptionsValue { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string RfIdNumbers { get; set; }

    }
    public class VehicleData
    {
        public int Id { get; set; }
        public string VIN { get; set; }

        public string Make { get; set; }

        public long Model { get; set; }

        public long ModelYear { get; set; }
        public string licencePlate { get; set; }
        public string department { get; set; }
        public string domicileLocation { get; set; }
        public string vehicleMacAddress { get; set; }
        public bool isActive { get; set; }
        public string createdBy { get; set; }
        public DateTime createdOn { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedOn { get; set; }
         
        //public long vehicleModelYearid { get; set; }
        //public VehicleModelYear vehicleModelYear { get; set; }
        public int vehicleModelId { get; set; }
        public VehicleModel vehicleModel { get; set; }

        public int SubscriptionPlanCustomerId { get; set; }

        public SubscriptionPlan SubscriptionPlan { get; set; }
        public int vehicleMakeId { get; set; }
        public VehicleMake vehicleMake { get; set; }
        public  int vehicleRFIDid { get; set; }
        public List<VehicleRFID> vehicleRFID { get; set; }
        public List<ApplicableSubscriptionPlan> applicableSubscriptionPlans { get; set; }
    }

    public class VehicleMake
    {
        public int Id { get; set; }
        public string name { get; set; }
        public bool isActive { get; set; }
        public string createdBy { get; set; }
        public DateTime createdOn { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedOn { get; set; }
    }

    public class VehicleModel
    {
        public int Id { get; set; }
        public string name { get; set; }
        public bool isActive { get; set; }
        public string createdBy { get; set; }
        public DateTime createdOn { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedOn { get; set; }
    }

    public class VehicleModelYear
    {
        public int Id { get; set; }
        public string name { get; set; }
        public bool isActive { get; set; }
        public string createdBy { get; set; }
        public DateTime createdOn { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedOn { get; set; }
    }

    public class VehicleRFID
    {
        public int Id { get; set; }
        public string name { get; set; }
        public bool isActive { get; set; }
        public string createdBy { get; set; }

        public long VehicleId { get; set; }

        public DateTime createdOn { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedOn { get; set; }
    }

    public class SubscriptionPlan
    {

        public long CustomerId { get; set; }


        public string CustomerName { get; set; }

        public string SubscriptionPlanName { get; set; }


        public string Description { get; set; }


        public long CurrencyId { get; set; }



        public DateTime ValidFrom { get; set; }


        public DateTime ValidTo { get; set; }
        public long StatusId { get; set; }

        public long SubscriptionsGroupId { get; set; }

        public string SubscriptionsDetails { get; set; }

        public string SubscriptionsValue { get; set; }
    }





}