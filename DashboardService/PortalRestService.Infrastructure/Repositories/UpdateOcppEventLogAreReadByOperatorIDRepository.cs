using Microsoft.EntityFrameworkCore;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Infrastructure.DBContext;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
using System.Net;

namespace PortalRestService.Infrastructure.Repositories
{
    public class UpdateOcppEventLogAreReadByOperatorIDRepository : OcppRepository<EventLogLocationResponse>, IUpdateOcppEventLogAreReadByOperatorIDRepository
    {
        private readonly TokenBase _tokenBase;
        public UpdateOcppEventLogAreReadByOperatorIDRepository(ocpp_dbContext dbContext, TokenBase token) : base(dbContext)
        {
            _tokenBase = token;
        }

        /// <inheritdoc/>
        public async Task<EventLogLocationResponse> UpdateOcppEventLogAreReadByOperator(List<int> eventLogIds)
        {

            // Fetch all relevant records to be updated
            var ocppEventLogsToUpdate = await _dbContext.OcppEventLogs
                   .Where(e => eventLogIds.Contains(e.Id) && e.IsRead == false)
                   .ToListAsync();

            if (ocppEventLogsToUpdate.Any())
            {
                // Update each entity in memory
                foreach (var item in ocppEventLogsToUpdate)
                {
                    item.IsRead = true;
                }

                // Batch update using DbContext
                _dbContext.OcppEventLogs.UpdateRange(ocppEventLogsToUpdate);
                await _dbContext.SaveChangesAsync();

                return new EventLogLocationResponse
                {
                    StatusMessage = RespnoseMessage.Record_Updated_Successfully,
                    StatusCode = (int)HttpStatusCode.OK
                };
            }

            return new EventLogLocationResponse
            {
                StatusMessage = RespnoseMessage.Record_Not_Updated,
                StatusCode = (int)HttpStatusCode.NotModified
            };
        }
    }
}