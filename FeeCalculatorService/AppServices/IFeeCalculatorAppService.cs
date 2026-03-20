using Scc.FeeCalculatorService.Results;

namespace Scc.FeeCalculatorService.AppServices;

public interface IFeeCalculatorAppService
{
    FeeResult Calculate(decimal amount, bool preferredCustomer);
}
