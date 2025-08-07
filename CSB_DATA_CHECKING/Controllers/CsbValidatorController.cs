using CSB_DATA_CHECKING.Helper.ExceptionLogHelper; // Add this using
using CSB_DATA_CHECKING.Helpers;
using CSB_DATA_CHECKING.Services;
using Microsoft.AspNetCore.Mvc;

namespace CSB_DATA_CHECKING.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CsbUploadController : ControllerBase
    {
        private readonly ICsbValidatorService _validator;
        private readonly IWebHostEnvironment _env;
        private readonly ExceptionLogger _logger = new(); // Added logger instance

        public CsbUploadController(ICsbValidatorService validator, IWebHostEnvironment env)
        {
            _validator = validator;
            _env = env;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadCsbFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                var result = await _validator.ValidateCsbFileAsync(file);

                string? downloadUrl = null;

                if (result.CellErrors.Any())
                {
                    stream.Position = 0;
                    var highlightedBytes = ExcelHighlightHelper.HighlightErrorsInExcel(stream, result.CellErrors);

                    string fileName = $"ValidationResult_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    string validatedFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "validated");

                    if (!Directory.Exists(validatedFolder))
                        Directory.CreateDirectory(validatedFolder);

                    string fullPath = Path.Combine(validatedFolder, fileName);
                    await System.IO.File.WriteAllBytesAsync(fullPath, highlightedBytes);

                    var request = HttpContext.Request;
                    downloadUrl = $"{request.Scheme}://{request.Host}/validated/{fileName}";
                }

                return Ok(new
                {
                    result.Success,
                    result.Message,
                    result.FileName,
                    result.PassedRules,
                    result.FailedRules,
                    result.CellErrors,
                    DownloadUrl = downloadUrl
                });
            }
            catch (Exception ex)
            {
                // Log the exception to ErrorLog.txt
                _logger.ExceptionLog("CsbUploadController.UploadCsbFile", ex.ToString());
                return StatusCode(500, "An error occurred while processing the file.");
            }
        }

        //[HttpGet("download")]
        //public IActionResult Download(string fileName)
        //{
        //    string path = Path.Combine(_env.WebRootPath ?? "wwwroot", "validated", fileName);

        //    if (!System.IO.File.Exists(path))
        //        return NotFound("File not found.");

        //    byte[] bytes = System.IO.File.ReadAllBytes(path);
        //    return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        //}
    }
}
