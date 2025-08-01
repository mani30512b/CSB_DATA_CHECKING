using CSB_DATA_CHECKING.Models;

namespace CSB_DATA_CHECKING.Services
{
    public interface ICsbValidatorService
    {
        Task<CsbValidationResults> ValidateCsbFileAsync(IFormFile file);
    }
}
