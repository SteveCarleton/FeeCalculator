using Microsoft.Extensions.Options;
using Scc.FeeCalculator.Configuration;
using Scc.FeeCalculator.Results;

namespace Scc.FeeCalculator.AppServices;

public class FeeCalculatorAppService(IOptions<FeeOptions> options, ILogger<FeeCalculatorAppService> logger) : IFeeCalculatorAppService
{
    private readonly IOptions<FeeOptions> _options = options;
    private readonly ILogger<FeeCalculatorAppService> _logger = logger;

    public FeeResult Calculate(decimal amount, bool preferredCustomer)
    {
        _logger.LogInformation("Calculation started");

        FeeResult result = new()
        {
            CalculatedFee = amount * _options.Value.BaseRate,
            InputAmount = amount,
            Preferred = preferredCustomer,
            BaseRate = _options.Value.BaseRate
        };

        if (preferredCustomer)
        {
            result.EffectiveRate = _options.Value.BaseRate - _options.Value.PreferredCustomerDiscount;
            result.CalculatedFee = amount * result.EffectiveRate;
        }

        if (result.CalculatedFee > _options.Value.MaxFee)
        {
            result.CalculatedFee = _options.Value.MaxFee;
            result.FeeCapped = true;
            _logger.LogWarning("Calculation capped");
        }

        return result;
    }
}
