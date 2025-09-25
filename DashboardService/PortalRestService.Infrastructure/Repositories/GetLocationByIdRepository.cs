using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
using Serilog;
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

        public async Task<GetLocatinByIdResponse> GetLocationById1(LocationRequest locationRequest)
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
                                           select new LocationAddressDTO
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


        public async Task<GetLocatinByIdResponse> GetLocationById(LocationRequest locationRequest)
        {
            var response = new GetLocatinByIdResponse();

            try
            {
                var location = await _dbContext.Locations
                    .Where(l => l.Id == locationRequest.Id)
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

                        // Location Address
                        LocationAddress = _dbContext.LocationAddress
                            .Where(a => a.Id == m.LocationAddressId)
                            .Select(a => new LocationAddressDTO
                            {
                                Id = a.Id,
                                AddressLine1 = a.AddressLine1,
                                AddressLine2 = a.AddressLine2,
                                CityName = a.CityName,
                                CountryId = a.CountryId,
                                CountryName = a.CountryName,
                                CreatedBy = a.CreatedBy,
                                CreatedOn = a.CreatedOn,
                                IsActive = a.IsActive,
                                LandlineNumber = a.LandlineNumber,
                                Latitude = a.Latitude,
                                Longitude = a.Longitude,
                                ModifiedBy = a.ModifiedBy,
                                ModifiedOn = a.ModifiedOn,
                                PinCode = a.PinCode,
                                StateId = a.StateId,
                                StateName = a.StateName
                            })
                            .FirstOrDefault(),

                        // Location Status
                        LocationStatus = _dbContext.LocationStatus
                            .Where(s => s.Id == m.LocationStatusId)
                            .Select(s => new LocationStatus
                            {
                                Id = s.Id,
                                LocationStatusName = s.LocationStatusName,
                                CreatedBy = s.CreatedBy,
                                CreatedOn = s.CreatedOn,
                                IsActive = s.IsActive,
                                ModifiedBy = s.ModifiedBy,
                                ModifiedOn = s.ModifiedOn
                            })
                            .FirstOrDefault(),

                        // Location Schedule
                        LocationSchedule = _dbContext.LocationSchedule
                            .Where(ls => ls.LocationId == m.Id)
                            .Select(ls => new LocationSchedule
                            {
                                Day = ls.Day,
                                StartTime = ls.StartTime,
                                EndTime = ls.EndTime,
                                CreatedBy = ls.CreatedBy,
                                CreatedOn = ls.CreatedOn,
                                Id = ls.Id,
                                LocationId = ls.LocationId,
                                IsActive = ls.IsActive,
                                ModifiedBy = ls.ModifiedBy,
                                ModifiedOn = ls.ModifiedOn,
                                IsOpenAlldays = ls.IsOpenAlldays
                            })
                            .ToList(),

                        // Operator User Mapper
                        OperatorUserMapper = _dbContext.OperatorUserMapper
                            .Where(oum => oum.LocationId == m.Id)
                            .Select(oum => new OperatorUserMapper
                            {
                                UserId = oum.UserId.ToString(),
                                CreatedBy = oum.CreatedBy,
                                CreatedOn = oum.CreatedOn,
                                Id = oum.Id,
                                LocationId = oum.LocationId,
                                IsActive = oum.IsActive,
                                ModifiedBy = oum.ModifiedBy,
                                ModifiedOn = oum.ModifiedOn
                            })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                response.data = location;
                response.StatusMessage = location != null ? RespnoseMessage.Record_found : RespnoseMessage.Record_not_found;
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetLocationById for LocationId {LocationId}", locationRequest.Id);
                response.StatusMessage = RespnoseMessage.Opeartion_Failed;
                response.StatusCode = RespnoseCode.Bad_Request;
            }

            return response;
        }
    }
}
