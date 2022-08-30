namespace PortalRestService.Helper
{
    public class APIConstant
    {

        public const string GetLocationById = "Location/getlocationbyid?id=";
        public const string GetAllLocationName = "Location/getalllocationname";
        public const string Getlocationsdispenserformap = "Location/getlocationsdispenserformap";
        public const string GetLocationsDispenserDetails = "Location/getlocationsdispenserdetails";

        public const string GetSummaryData = "https://run.mocky.io/v3/0a2a634e-ebb6-4132-8bc3-b9668d27cbe4";

        public const string GetChargerSessionAll = "ChargingSession/getChargingSession";
        public const string GetDispenserByLocation = "Dispenser/getdispenserbylocationid?Id=";
        public const string GetDispenserByLocations = "Dispenser/getdispenserbylocations";

        public const string GetAllLocation = "Location/getAllLocation";

        public const string GetAllDispenser = "Dispenser/getAllDispenser";     //Auther: Pradeep, Date 19/07/2022
        public const string GetChargingSession = "ChargingSession/getChargingSession";     //Auther: Pradeep, Date 20/07/202
        public const string GetTotalLocationAndCharger = "ChargingInfrasture/getTotalLocationAndCharger";     //Auther: Pradeep, Date 20/07/202
                                                                                                              //http://51.141.73.41:6003/api/ChargingSession/getChargingSession
                                                                                                              //public const string GetSummaryData = "https://run.mocky.io/v3/0a2a634e-ebb6-4132-8bc3-b9668d27cbe4";
        public const string Getdispenserbylocation = "Location/getdispenserbylocation";

        
        public const string GetVehicleByID = "Vehicle/GetAllVehicleByID?id="; 

        public const string GetEventLogByLocationAll = "OcppEventLog/GetOcppEventLog";
        public const string UpdateOcppEventLogIsRead = "OcppEventLog/UpdateOcppEventLogIsRead?id=";


        public const string GetlAllVehicle = "Vehicle/GetAllVechicle";
        public const string GetDispensersList = "Dispenser/GetDispensersList";            // Auther: Pradeep, Date 08/08/2022

        public const string GetDispenserByChargeboxId = "Dispenser/getdispenserbychargeboxid?ChargeBoxId=";

    }
}
