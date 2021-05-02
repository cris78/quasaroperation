using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Satellites;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class TopSecretController : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<List<Satellite>>> GetSatellites()
        {
            return await Mediator.Send(new List.Query());
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<Satellite>> GetSatelliteByName(string name)
        {
            return await Mediator.Send(new Details.Query { Name = name });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSatellite(List<SatelliteDto> satellites)
        {
            return HandleResult(await Mediator.Send(new Edit.Command { Satellites = satellites }));
        }
    }
}