using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
using System.Text;
using Newtonsoft.Json;
using PortalRestService.Application;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.EnumData;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Models;
using PortalRestService.Infrastructure.Models;

namespace PortalRestService.Infrastructure.Repositories
{
    public class LocationDispenserRepository : OcppRepository<LocationDispenserForLocationResponse>, ILocationDispenserRepository
    {
        private readonly ILocationRepository _locationRepository;
        public LocationDispenserRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, ILocationRepository locationRepository) : base(dbContext)
        {
            _locationRepository = locationRepository;
        }

        //async Task<LocationDispenserForLocationResponse> ILocationDispenserRepository.GetDispenserByLocation(LocationDispensersRequest locationDispensersRequest)
        //{
        //    LocationDispenserForLocationResponse objLocationDispneser = new LocationDispenserForLocationResponse();
        //    List<long> locationList = await _locationRepository.GetAllLocationIdByObjectId();
        //    objLocationDispneser.data = (from location in locationDispensersRequest.locationIds.Count > 0 ? _dbContext.Locations.Where(x => locationDispensersRequest.locationIds.Contains(x.Id) && locationList.Contains(x.Id)) : _dbContext.Locations.Where(x => locationList.Contains(x.Id))
        //                                 join charger in !string.IsNullOrEmpty(locationDispensersRequest.SearchParam) == true ? _dbContext.Charger.Where(d => locationDispensersRequest.SearchParam.ToLower().Contains(d.ChargeBoxId.ToLower())) : _dbContext.Charger
        //                                 on location.Id equals charger.LocationId
        //                                 join address in _dbContext.LocationAddress
        //                                 on location.LocationAddressId equals address.Id
        //                                 join Status in _dbContext.LocationStatus
        //                                 on location.LocationStatusId equals Status.Id
        //                                 select new LocationDispenserForLocation
        //                                 {
        //                                     DispenserId = charger.Id,
        //                                     locationId = location.Id,
        //                                     ChargeBoxId = charger.ChargeBoxId,
        //                                     ChargerStatus = charger.ChargerStatuses == null || charger.ChargerStatuses.Count == 0 ? "Offline" :
        //                           charger.ChargerStatuses.ToList()[0].Chargerstatus.Replace("charging", "Busy").Replace("suspendedev", "Busy").Replace("uspendedevse", "Busy")
        //                         .Replace("finishing", "Busy").Replace("preparing", "Busy"),
        //                                     ConnectorType = String.Join(",", _dbContext.Connector.Where(cnn => charger.Ports.Where(p => p.ChargerId == charger.Id).Select(s => s.ConnectorType).Contains(cnn.Id)).Select(z => z.ConnectorType)),
        //                                     DispenserModel = charger.ModelName,
        //                                     ProtocolName = charger.ProtocolName,
        //                                     NoofPort = charger.Ports.Count.ToString(),
        //                                     DispenserMake = charger.MakeName,
        //                                     ModifiedAt = charger.CreatedOn,
        //                                     AssetId = charger.AssetId
        //                                 }

        //                   ).OrderByDescending(m => m.ModifiedAt).ToList<LocationDispenserForLocation>();
        //    return objLocationDispneser;
        //}

        async Task<LocationDispenserForLocationResponse>ILocationDispenserRepository.GetDispenserByLocation(LocationDispensersRequest locationDispensersRequest)
        {
            var response = new LocationDispenserForLocationResponse();

            List<long> allowedLocationIds =
                await _locationRepository.GetAllLocationIdByObjectId();

            // ----------------------------
            // Base location query
            // ----------------------------
            IQueryable<Location> locationQuery =
                locationDispensersRequest.locationIds != null &&
                locationDispensersRequest.locationIds.Any()
                    ? _dbContext.Locations.Where(x =>
                        locationDispensersRequest.locationIds.Contains(x.Id) &&
                        allowedLocationIds.Contains(x.Id))
                    : _dbContext.Locations.Where(x =>
                        allowedLocationIds.Contains(x.Id));

            // ----------------------------
            // Base charger query
            // ----------------------------
            IQueryable<Charger> chargerQuery = _dbContext.Charger;

            if (!string.IsNullOrWhiteSpace(locationDispensersRequest.SearchParam))
            {
                string search = locationDispensersRequest.SearchParam.ToLower();
                chargerQuery = chargerQuery.Where(c =>
                    c.ChargeBoxId.ToLower().Contains(search));
            }

            int activationStatus = locationDispensersRequest.ActivationStatus ?? 1;

            if (activationStatus != 0)
            {
                if (activationStatus == 1)
                {
                    chargerQuery = chargerQuery.Where(c => c.IsActive);
                }
                else if (activationStatus == 2)
                {
                    chargerQuery = chargerQuery.Where(c => !c.IsActive);
                }
            }

            // ----------------------------
            // Final query
            // ----------------------------
            response.data = (
                from location in locationQuery
                join charger in chargerQuery
                    on location.Id equals charger.LocationId
                join address in _dbContext.LocationAddress
                    on location.LocationAddressId equals address.Id
                join status in _dbContext.LocationStatus
                    on location.LocationStatusId equals status.Id
                select new LocationDispenserForLocation
                {
                    DispenserId = charger.Id,
                    locationId = location.Id,
                    ChargeBoxId = charger.ChargeBoxId,

                    ChargerStatus =
                        charger.ChargerStatuses == null ||
                        charger.ChargerStatuses.Count == 0
                            ? "Offline"
                            : charger.ChargerStatuses.First().Chargerstatus
                                .Replace("charging", "Busy")
                                .Replace("suspendedev", "Busy")
                                .Replace("uspendedevse", "Busy")
                                .Replace("finishing", "Busy")
                                .Replace("preparing", "Busy"),

                    ConnectorType = string.Join(",",
                        _dbContext.Connector
                            .Where(cnn =>
                                charger.Ports
                                    .Where(p => p.ChargerId == charger.Id)
                                    .Select(s => s.ConnectorType)
                                    .Contains(cnn.Id))
                            .Select(z => z.ConnectorType)),

                    DispenserModel = charger.ModelName,
                    ProtocolName = charger.ProtocolName,
                    NoofPort = charger.Ports.Count.ToString(),
                    DispenserMake = charger.MakeName,
                    ModifiedAt = charger.CreatedOn,
                    AssetId = charger.AssetId
                })
                .OrderByDescending(m => m.ModifiedAt)
                .ToList();

            return response;
        }

    }
}
