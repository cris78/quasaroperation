using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Satellites;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class TopSecretController : BaseApiController
    {
        /// <summary>
        /// Get Collection of Satellites
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<Satellite>>> GetSatellites()
        {
            return await Mediator.Send(new List.Query());
        }

        /// <summary>
        /// Get Satellite Information by Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("{name}")]
        public async Task<ActionResult<Satellite>> GetSatelliteByName(string name)
        {
            return await Mediator.Send(new Details.Query { Name = name });
        }

        /// <summary>
        /// Update Satellite Information, calculate ship position by trilateration and decode the message
        /// </summary>
        /// <param name="satellites"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateSatellite(List<SatelliteDto> satellites)
        {
            return HandleResult(await Mediator.Send(new Edit.Command { Satellites = satellites }));
        }
    }
}