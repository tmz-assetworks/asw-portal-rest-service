using Microsoft.Extensions.Configuration;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Repositories
{

    public class GetLocationByIdRepository : OcppRepository<GetLocatinByIdResponse>, IGetLocationByIdRepository
    {
        TokenBase _tokenBase;

        private readonly string OccpIp = String.Empty;
        public GetLocationByIdRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, TokenBase token) : base(dbContext)
        {
            _tokenBase = token;


        }

        public async Task<GetLocatinByIdResponse> GetLocationById(LocationRequest locationRequest)
        {
            GetLocatinByIdResponse s = new GetLocatinByIdResponse();
            try
            {

                s.data = _dbContext.Locations.Where(t => t.Id == locationRequest.Id)
                    .Select(m => new Data
                    {
                        Id = (int)m.Id,
                        ContactPersonName = m.ContactPersonName,
                        ContactPersonNumber = m.ContactPersonNumber,
                        GlobalTax = m.GlobalTax,
                        TotalCapacity = m.TotalCapacity,
                        UtilityService = m.UtilityService,
                        CreatedBy = m.CreatedBy,
                        CreatedOn = m.CreatedOn,
                        Description = m.Description,
                        IsActive = m.IsActive,
                        AlternateMobileNumber = m.AlternateMobileNumber,
                        Email = m.Email,
                        ModifiedBy = m.ModifiedBy,
                        ModifiedOn = m.ModifiedOn,
                        DepartmentName = m.DepartmentName,
                        FuelProtectType = m.FuelProtectType,
                        LocationName = m.LocationName,
                        TimeZone = m.TimeZone,
                        LocationAddress = (from obls in _dbContext.LocationAddress.Where(x => x.Id == m.LocationAddressId)
                                           select new LocationAddress
                                           {
                                               Id = obls.Id,
                                               AddressLine1 = obls.AddressLine1,
                                               AddressLine2 = obls.AddressLine2,
                                               //CityId = obls.CityId,
                                               CityName = obls.CityName,
                                               CountryId = obls.CountryId,
                                               CountryName = obls.CountryName,
                                               CreatedBy = obls.CreatedBy,
                                               CreatedOn = obls.CreatedOn,
                                               IsActive = obls.IsActive,
                                               LandlineNumber = obls.LandlineNumber,
                                               Latitude = obls.Latitude,
                                               Longitude = obls.Longitude,
                                               ModifiedBy = obls.ModifiedBy,
                                               ModifiedOn = obls.ModifiedOn,
                                               PinCode = obls.PinCode,
                                               StateId = obls.StateId,
                                               StateName = obls.StateName
                                           }).FirstOrDefault(),

                        LocationStatus = (from obls in _dbContext.LocationStatus.Where(x => x.Id == m.LocationStatusId)
                                          select new LocationStatus
                                          {
                                              Id = obls.Id,
                                              LocationStatusName = obls.LocationStatusName,
                                              CreatedBy = obls.CreatedBy,
                                              CreatedOn = obls.CreatedOn,
                                              IsActive = obls.IsActive,
                                              ModifiedBy = obls.ModifiedBy,
                                              ModifiedOn = obls.ModifiedOn,
                                          }).FirstOrDefault(),
                        LocationSchedule = (from obls in _dbContext.LocationSchedule.Where(x => x.LocationId == m.Id)
                                            select new LocationSchedule
                                            {
                                                Day = obls.Day,
                                                StartTime = obls.StartTime,
                                                EndTime = obls.EndTime,
                                                CreatedBy = obls.CreatedBy,
                                                CreatedOn = obls.CreatedOn,
                                                Id = obls.Id,
                                                LocationId = obls.LocationId,
                                                IsActive = obls.IsActive,
                                                ModifiedBy = obls.ModifiedBy,
                                                ModifiedOn = obls.ModifiedOn,
                                                IsOpenAlldays = obls.IsOpenAlldays
                                            }).ToList(),
                        OperatorUserMapper = (from obls in _dbContext.OperatorUserMapper.Where(x => x.LocationId == m.Id)
                                              select new OperatorUserMapper
                                              {
                                                  UserId = obls.UserId.ToString(),
                                                  CreatedBy = obls.CreatedBy,
                                                  CreatedOn = obls.CreatedOn,
                                                  Id = obls.Id,
                                                  LocationId = obls.LocationId,
                                                  IsActive = obls.IsActive,
                                                  ModifiedBy = obls.ModifiedBy,
                                                  ModifiedOn = obls.ModifiedOn,
                                              }).ToList(),

                    }).ToList().FirstOrDefault();

                if (s.data!=null)
                    s.StatusMessage = RespnoseMessage.Record_found;
                else
                    s.StatusMessage = RespnoseMessage.Record_not_found;
                s.StatusCode = 200;
               

            }
            catch (Exception ex)
            {
                s.StatusMessage = RespnoseMessage.Opeartion_Failed;
                s.StatusCode = RespnoseCode.Bad_Request;

            }
            return s;
        }
    }
}
