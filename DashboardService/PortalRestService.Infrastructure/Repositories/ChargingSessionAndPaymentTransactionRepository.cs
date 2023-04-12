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
        public ChargingSessionAndPaymentTransactionRepository(Infrastructure.DBContext.ocpp_dbContext dbContext):base(dbContext)
        {
        }
        public async Task<SessionAndPaymentDTO> GetSessionAndPaymentDTOAsync(long PaymentTransactionId)
        {
            SessionAndPaymentDTO sessionAndPaymentDTO = new SessionAndPaymentDTO();
            sessionAndPaymentDTO.sessionAndPaymentData = await (from pt in _dbContext.PaymentTransaction.Where(x=> x.Id == PaymentTransactionId)
                                                                join session in _dbContext.ChargingSessions on pt.ChargingSessionId equals session.Id
                                                                join ch in _dbContext.Charger on session.DeviceId equals ch.ChargeBoxId
                                                                join port in _dbContext.Port on ch.Id equals port.ChargerId
                                                                join c in _dbContext.Connector on port.ConnectorType equals c.Id
                                                                where port.Connectorid == session.ConnectorId
                                                                select new SessionAndPaymentData
                                                                {
                                                                    assetId = ch.AssetId,
                                                                    connectionType = c.ConnectorType,
                                                                    cost =Convert.ToDouble(pt.TotalAmount),
                                                                    createdDateTime = DateTime.Now,
                                                                    employeeId = session.RfId,
                                                                    endDateTime = session.EndTime.Value,
                                                                    fuelingSeconds =Convert.ToInt32((session.EndTime - session.StartTime).Value.TotalSeconds),
                                                                    hoseId = port.Id,
                                                                    issueDateTime = session.EndTime.Value,
                                                                    issueQty = null,
                                                                    locationId = ch.LocationId.Value,
                                                                    meter1Reading = 0,
                                                                    startDateTime = session.StartTime.Value,
                                                                    description = "ELECTRICITY",
                                                                    externalTransactionId = 0,
                                                                    fromStock = false,
                                                                    tax = Convert.ToDouble(pt.Tax),
                                                                    unitCost = 0,
                                                                    meter1Good = true,
                                                                    meter2Good = false,
                                                                    meter2TypeId ="NONE",
                                                                    meter1TypeId = "MILES",
                                                                    vendorName = "AssetWorks CMS",
                                                                    isReturn = false,
                                                                    isReturnable = true,
                                                                    productId = "UNL",
                                                                    productTypeId ="FUEL"                                                                   
                                                                    
                                                                }).FirstOrDefaultAsync();

            return sessionAndPaymentDTO;
        }
    }
}
