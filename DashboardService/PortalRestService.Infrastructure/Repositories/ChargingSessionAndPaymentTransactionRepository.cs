using Microsoft.EntityFrameworkCore;
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
#pragma warning disable
	public class ChargingSessionAndPaymentTransactionRepository : OcppRepository<SessionAndPaymentDTO>, IChargingSessionAndPaymentTransactionRepository
	{
		public ChargingSessionAndPaymentTransactionRepository(Infrastructure.DBContext.ocpp_dbContext dbContext) : base(dbContext)
		{
		}
		public async Task<SessionAndPaymentDTO> GetSessionAndPaymentDTOAsyncOld(long PaymentTransactionId)
		{
			DateTime createdDateTime1 = DateTime.Now;
			SessionAndPaymentDTO sessionAndPaymentDTO = new SessionAndPaymentDTO();
			sessionAndPaymentDTO.sessionAndPaymentData = await (from pt in _dbContext.PaymentTransaction.Where(x => x.Id == PaymentTransactionId)
																join session in _dbContext.ChargingSessions on pt.ChargingSessionId equals session.Id
																join vehiclerfid in _dbContext.VehicleRFID on session.RfId equals vehiclerfid.Name
																join vehicle in _dbContext.Vehicle on vehiclerfid.VehicleId equals vehicle.Id
																join ch in _dbContext.Charger on session.DeviceId equals ch.ChargeBoxId
																join port in _dbContext.Port on ch.Id equals port.ChargerId
																join c in _dbContext.Connector on port.ConnectorType equals c.Id
																join lc in _dbContext.Locations on ch.LocationId equals lc.Id
																where port.Connectorid == session.ConnectorId
																select new SessionAndPaymentData
																{
																	//assetId = ch.AssetId, 
																	//assetId = session.RfId,
																	assetId=vehicle.AssetId,
																	connectionType = c.ConnectorType,
																	//cost = Math.Round(Convert.ToDouble(pt.TotalAmount), 3),
																	//unitCost=Math.Round(Convert.ToDouble(pt.TotalAmount), 3),
																	createdDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
																	//employeeId = session.RfId,
																	endDateTime = DateTime.SpecifyKind(session.EndTime.Value, DateTimeKind.Utc),
																	fuelingSeconds = Convert.ToInt32((session.EndTime - session.StartTime).Value.TotalSeconds),
																	hoseId = port.PortName,
																	issueDateTime = DateTime.SpecifyKind(session.EndTime.Value, DateTimeKind.Utc),
																	//issueQty = null,
																	issueQty = Math.Round((Convert.ToDouble(session.EndMeterValue) - Convert.ToDouble(session.StartMeterValue)) / 1000, 3),
																	//locationId = ch.LocationId.Value,
																	locationId = lc.LocationId,
																	meter1Reading = 0,
																	startDateTime = DateTime.SpecifyKind(session.StartTime.Value, DateTimeKind.Utc),
																	description = "ELECTRICITY",
																	//externalTransactionId = 0,
																	fromStock = false,
																	tax = Math.Round(Convert.ToDouble(pt.Tax), 3),
																	//unitCost = 0,
																	meter1Good = true,
																	meter2Good = false,
																	meter2TypeId = "NONE",
																	meter1TypeId = "MILES",
																	vendorName = "AssetWorks CMS",
																	isReturn = false,
																	isReturnable = true,
																	productId = "EC",
																	productTypeId = "FUEL",
																	vendorId = "V915",
																	_recordId=Convert.ToString(PaymentTransactionId)

																}).FirstOrDefaultAsync();

			return sessionAndPaymentDTO;
		}

        public async Task<SessionAndPaymentDTO> GetSessionAndPaymentDTOAsync(long PaymentTransactionId)
        {
            DateTime createdDateTime1 = DateTime.Now;
            SessionAndPaymentDTO sessionAndPaymentDTO = new SessionAndPaymentDTO();
            sessionAndPaymentDTO.sessionAndPaymentData = await (from pt in _dbContext.PaymentTransaction.Where(x => x.Id == PaymentTransactionId)
                                                                join session in _dbContext.ChargingSessions on pt.ChargingSessionId equals session.Id
                                                                join vehiclerfid in _dbContext.VehicleRFID on session.RfId equals vehiclerfid.Name
                                                                join vehicle in _dbContext.Vehicle on vehiclerfid.VehicleId equals vehicle.Id
                                                                join ch in _dbContext.Charger on session.DeviceId equals ch.ChargeBoxId
                                                                join port in _dbContext.Port on ch.Id equals port.ChargerId
                                                                join c in _dbContext.Connector on port.ConnectorType equals c.Id
                                                                join lc in _dbContext.Locations on ch.LocationId equals lc.Id
                                                                where port.Connectorid == session.ConnectorId
                                                                select new SessionAndPaymentData
                                                                {
                                                                    assetId = vehicle.AssetId,
                                                                    assetInternalId = 0,
                                                                    connectionType = c.ConnectorType,
                                                                    cost = Math.Round(Convert.ToDouble(pt.TotalAmount), 3),
                                                                    createdDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                                                                    employeeNumber = "AssetWorksCMS",
                                                                    endDateTime = DateTime.SpecifyKind(session.EndTime.Value, DateTimeKind.Utc),
                                                                    fuelingSeconds = Convert.ToInt32((session.EndTime - session.StartTime).Value.TotalSeconds),
                                                                    hoseId = port.PortName,
                                                                    internalId = 0,
                                                                    invoiceNumber = 0,
                                                                    issueDateTime = DateTime.SpecifyKind(session.EndTime.Value, DateTimeKind.Utc),
                                                                    issueQty = Math.Round((Convert.ToDouble(session.EndMeterValue) - Convert.ToDouble(session.StartMeterValue)) / 1000, 3),
                                                                    locationId = lc.LocationId,
                                                                    meter1Reading = 0,
                                                                    meter2Reading = 0,
                                                                    startDateTime = DateTime.SpecifyKind(session.StartTime.Value, DateTimeKind.Utc),
                                                                    description = "ELECTRICITY",
                                                                    fromStock = false,
                                                                    tax = Math.Round(Convert.ToDouble(pt.Tax), 3),
                                                                    meter1Good = true,
                                                                    meter2Good = false,
                                                                    meter2TypeId = "NONE",
                                                                    meter1TypeId = "MILES",
                                                                    vendorName = "AssetWorks CMS",
                                                                    isReturn = false,
                                                                    isReturnable = true,
                                                                    productId = "EC",
                                                                    productTypeId = "FUEL",
                                                                    vendorId = "V915",
                                                                    _recordId = Convert.ToString(PaymentTransactionId)

                                                                }).FirstOrDefaultAsync();

            return sessionAndPaymentDTO;
        }
    }
}
