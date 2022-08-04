using PortalRestService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PortalRestService.Core.Responses;

namespace RestService.Assets.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class ChargerController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ChargerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("status/summary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ChargerResponse> Get()
        {
            return await _mediator.Send(new GetAllChargerQuery());
        }

        [HttpGet("ChargingSession")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ChargingSessionResponse>>> ChargingSession()
        {
            try
            {
                var result = await _mediator.Send(new GetAllChargingSessionQuery());
                return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }
        
        [HttpGet("EnergyUsed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<EnergyUsedResponse>>> EnergyUsed()
        {
            try
            {
                var result = await _mediator.Send(new GetAllEnergyUsedQuery());
                return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }


        [HttpGet("GetCharger")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ChargerSessionResponse>>> GetCharger()
        {
            try
            {
                var result = await _mediator.Send(new GetChargingSessionQuery());
                return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }
    }
}
