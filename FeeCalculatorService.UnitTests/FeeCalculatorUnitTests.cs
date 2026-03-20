using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
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

        Assert.Equal(inputAmount * options.Value.BaseRate, result.CalculatedFee);
    }

    [Fact]
    public void PreferredCustomerNocap_ReturnsOkay()
    {
        // Arrange

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

        Assert.Equal(options.Value.BaseRate - options.Value.PreferredCustomerDiscount, result.EffectiveRate);
        Assert.Equal(inputAmount * result.EffectiveRate, result.CalculatedFee);
    }

    [Fact]
    public void FeeExceedsCap_ReturnsOkay()
    {
        // Arrange

        FeeOptions feeOptions = new();
        var options = Options.Create(feeOptions);
        options.Value.BaseRate = 0.05m;
        options.Value.MaxFee = 250.0m;
        options.Value.PreferredCustomerDiscount = 0.01m;

        Mock<ILogger<FeeCalculatorAppService>> loggerMock = new();

        const decimal inputAmount = 100000m;
        const bool preferredCustomer = true;

        FeeCalculatorAppService appService = new(options, loggerMock.Object);

        // Act
        FeeResult result = appService.Calculate(inputAmount, preferredCustomer);

        // Assert

        Assert.Equal(options.Value.MaxFee, result.CalculatedFee);
        Assert.True(result.FeeCapped);
    }
}
