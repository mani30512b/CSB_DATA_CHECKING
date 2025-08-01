using CSB_DATA_CHECKING.Models;

namespace CSB_DATA_CHECKING.Rules
{
    public interface ICsbRule
    {
        string RuleName { get; }
        void Validate(List<string> headers, CsbRow row, int rowIndex, CsbValidationResults result);
    }
}
