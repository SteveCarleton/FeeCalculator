using Scc.FeeCalculator.Results;

namespace Scc.FeeCalculator.AppServices;

public interface IFeeCalculatorAppService
{
    FeeResult Calculate(decimal amount, bool preferredCustomer);
}
