using Microsoft.AspNetCore.Mvc;
using NewSiteServer.Services;

namespace NewSiteServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ManagedActionController : ControllerBase
    {
        public readonly IHandlerService handlerService;
        public ManagedActionController(IHandlerService _handlerService)
        {
            handlerService = _handlerService;
        }

        [HttpPost("CreateRSAKeys")]
        public IActionResult CreateRSAKeys()
        {
            try
            {
                var result = handlerService.CreateRSAKeys();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }


        [HttpPost("SubmitMyPosition")]
        public async Task<IActionResult> SubmitMyPosition([FromForm] Candidate candidate)
        {
            try
            {
                var result = await handlerService.SubmitMyPosition(candidate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }




        }


      
    }
}
