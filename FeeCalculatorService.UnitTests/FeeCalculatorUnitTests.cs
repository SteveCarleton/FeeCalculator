using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Scc.FeeCalculator;
using Scc.FeeCalculator.AppServices;
using Scc.FeeCalculator.Configuration;
using Scc.FeeCalculator.Results;

namespace FeeCalculatorService.UnitTests;

public class FeeCalculatorUnitTests
{
    [Fact]
    public void NonPreferredCustomerNocap_ReturnsOkay()
    {
        // Arrange

        FeeOptions feeOptions = new();
        var options = Options.Create(feeOptions);
        options.Value.BaseRate = 0.05m;
        options.Value.MaxFee = 250.0m;

        Mock<ILogger<FeeCalculatorAppService>> loggerMock = new();

        const decimal inputAmount = 10m;
        const bool preferredCustomer = false;

        FeeCalculatorAppService appService = new(options, loggerMock.Object);

        // Act
        FeeResult result = appService.Calculate(inputAmount, preferredCustomer);

        // Assert

        Assert.Equal(result.CalculatedFee, inputAmount * options.Value.BaseRate);
    }

    [Fact]
    public void PreferredCustomerNocap_ReturnsOkay()
    {
        FeeOptions feeOptions = new();
        var options = Options.Create(feeOptions);
        options.Value.BaseRate = 0.05m;
        options.Value.MaxFee = 250.0m;
        options.Value.PreferredCustomerDiscount = 0.01m;

        Mock<ILogger<FeeCalculatorAppService>> loggerMock = new();

        const decimal inputAmount = 10m;
        const bool preferredCustomer = true;

        FeeCalculatorAppService appService = new(options, loggerMock.Object);

        // Act
        FeeResult result = appService.Calculate(inputAmount, preferredCustomer);

        // Assert

        Assert.Equal(result.EffectiveRate, options.Value.BaseRate - options.Value.PreferredCustomerDiscount);
        Assert.Equal(result.CalculatedFee, inputAmount * result.EffectiveRate);
    }

    [Fact]
    public void FeeExceedsCap_ReturnsOkay()
    {
    }
}
