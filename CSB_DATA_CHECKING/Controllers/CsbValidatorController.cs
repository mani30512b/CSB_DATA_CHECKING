using CSB_DATA_CHECKING.Services;
using Microsoft.AspNetCore.Mvc;

namespace CSB_DATA_CHECKING.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CsbValidatorController : ControllerBase
    {
        private readonly ICsbValidatorService _csbValidatorService;

        public CsbValidatorController(ICsbValidatorService csbValidatorService)
        {
            _csbValidatorService = csbValidatorService;
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            var result = await _csbValidatorService.ValidateCsbFileAsync(file);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
