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
                                                                    AssetId = ch.AssetId,
                                                                    ConnectorType = c.ConnectorType,
                                                                    Cost = pt.TotalAmount,
                                                                    CreatedDateTime = DateTime.Now,
                                                                    EmployeeId = session.RfId,
                                                                    EndDateTime = session.EndTime,
                                                                    FuelingSeconds = (session.EndTime - session.StartTime).Value.TotalSeconds,
                                                                    HoseId = port.Id.ToString(),
                                                                    IssueDateTime = session.EndTime,
                                                                    IssueQty = 0,
                                                                    LocationId = ch.LocationId.ToString(),
                                                                    Meter1Reading = 0,
                                                                    StartDateTime = session.StartTime
                                                                }).FirstOrDefaultAsync();

            return sessionAndPaymentDTO;
        }
    }
}
