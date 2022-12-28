using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Models;
using PortalRestService.Infrastructure.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Repositories
{
    public class VehicleDashboardRepository : OcppRepository<VehicleByIdData>, IVehicleDashboardRepository
    {
        TokenBase _tokenBase;
        public VehicleDashboardRepository(Infrastructure.DBContext.ocpp_dbContext dbContext,TokenBase token) : base(dbContext)
        {
            _tokenBase = token;
        }

        // Get Vehicle detail by vehicleId
        // Auther:ATUL, Date : 
        public async Task<VehicleByIdData> VehicleDetailsById(long id)

        {
            VehicleByIdData vehicleByIdData = new VehicleByIdData();
            VehiclesResponse vehiclesResponse = new VehiclesResponse();

            DateTime currentDateTime = DateTime.Now;
            try
            {
                var m = _dbContext.Vehicle.Where(x => x.Id == id)
                     .Select(m => new Models.Vehicle
                     {
                         Id = m.Id,
                         VIN = m.VIN,
                         LicencePlate = m.LicencePlate,
                         Department = m.Department,
                         DomicileLocation = m.DomicileLocation,
                         VehicleMacAddress = m.VehicleMacAddress,
                         IsActive = m.IsActive,
                         CreatedBy = m.CreatedBy,
                         CreatedOn = m.CreatedOn,
                         ModifiedBy = m.ModifiedBy,
                         ModifiedOn = m.ModifiedOn,
                         ModelYear = m.ModelYear,
                         ModelName = m.ModelName,
                         MakeName = m.MakeName,
                         UnitNumber = m.UnitNumber,
                         vehicleRFID = (from obls in _dbContext.VehicleRFID.Where(x => x.VehicleId == m.Id)
                                        select new Models.VehicleRFID
                                        {
                                            Id = obls.Id,
                                            Name = obls.Name,
                                            IsActive = obls.IsActive,
                                            CreatedBy = obls.CreatedBy,
                                            VehicleId = obls.VehicleId,
                                            CreatedOn = obls.CreatedOn,
                                            ModifiedBy = obls.ModifiedBy,
                                            ModifiedOn = obls.ModifiedOn,
                                        }).ToList(),
                     }).FirstOrDefault();
                vehicleByIdData=new VehicleByIdData
                     {
                         VIN = m.VIN,
                         ModelYear = m.ModelYear,
                         MakeName = m.MakeName,
                         ModelName = m.ModelName,
                         licencePlate = m.LicencePlate,
                         department = m.Department,
                         domicileLocation = m.DomicileLocation,
                         vehicleMacAddress = m.VehicleMacAddress,
                         Status = m.IsActive,
                         UnitNumber = m.UnitNumber,
                    Id =m.Id,
                       
                         rfId = m.vehicleRFID != null ? String.Join(",", (m.vehicleRFID).Select(x => x.Name)) : "",

                };

                if (m != null)
                {
                    string[] vehicleRFIDs = _dbContext.VehicleRFID.Where(x => x.VehicleId == m.Id && x.IsActive == true).Select(v => v.Name.ToString()).ToArray();

                    string modelYear = m.ModelYear.ToString();
                    List<Models.SubscriptionsGroupDetails> subscriptionsGroupDetails = _dbContext.SubscriptionsGroupDetails.Where(s => s.IsActive == true && (vehicleRFIDs.Contains(s.Value) && s.Text.ToLower() == "rfid")
                    || (s.Text.ToLower() == "modelyear" && s.Value == modelYear) || (s.Text.ToLower() == "makename" && s.Value == m.MakeName) ||
                   (s.Text.ToLower() == "modelname" && s.Value == m.ModelName) || (s.Text.ToLower() == "vin" && s.Value == m.VIN)
                    ).ToList();

                    List<long> subscriptionsGroupIds = new List<long>();
                    foreach (var subscriptionsGroup in subscriptionsGroupDetails)
                    {
                        subscriptionsGroupIds.Add(subscriptionsGroup.SubscriptionsGroupId);
                    }
                    vehicleByIdData.applicableSubscriptionPlans = _dbContext.SubscriptionPlan.Where(x => subscriptionsGroupIds.Contains(x.SubscriptionsGroupId.Value)).OrderBy(p => p.Price)
                    .Where(p => p.ValidFrom <= currentDateTime && p.ValidTo >= currentDateTime && p.IsActive == true).Select(s => new ApplicableSubscriptionPlanDTO
                    {
                        RfIdNumbers = m.vehicleRFID != null ? String.Join(',', m.vehicleRFID.Select(s => s.Name)).ToString() : "",
                        SubscriptionPlanName = s.SubscriptionPlanName,
                        SubscriptionsValue = s.Price.ToString(),
                        ValidFrom = s.ValidFrom,
                        ValidTo = s.ValidTo,
                        Type = s.PriceType.PriceTypeName,

                    }).ToList<ApplicableSubscriptionPlanDTO>();

                   
                }
               

                return vehicleByIdData;

            }
            catch (Exception ex)
            {
                vehiclesResponse.StatusMessage = RespnoseMessage.Opeartion_Failed;
                vehiclesResponse.StatusCode = RespnoseCode.Bad_Request;
            }
            return vehicleByIdData;

        }

    }
}