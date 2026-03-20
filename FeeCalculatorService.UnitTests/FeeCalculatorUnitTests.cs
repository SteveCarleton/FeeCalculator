using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Scc.FeeCalculatorService.AppServices;
using Scc.FeeCalculatorService.Configuration;
using Scc.FeeCalculatorService.Results;

namespace FeeCalculatorService.UnitTests;

public class FeeCalculatorUnitTests
{
    private IOptions<FeeOptions> options = default!;
    private readonly Mock<ILogger<FeeCalculatorAppService>> loggerMock = new();

    private decimal inputAmount = 10m;
    private bool preferredCustomer = false;

    [Fact]
    public void NonPreferredCustomerNocap_ReturnsOkay()
    {
        // Arrange

        ArrangeFeeOptions();
        FeeCalculatorAppService appService = new(options, loggerMock.Object);

        // Act
        FeeResult result = appService.Calculate(inputAmount, preferredCustomer);

        // Assert

        Assert.Equal(inputAmount * options.Value.BaseRate, result.CalculatedFee);

        loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((value, _) => value.ToString()!.Contains("Calculation started")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
    }

    [Fact]
    public void PreferredCustomerNocap_ReturnsOkay()
    {
        // Arrange

        ArrangeFeeOptions();
        preferredCustomer = true;
        FeeCalculatorAppService appService = new(options, loggerMock.Object);

        // Act
        FeeResult result = appService.Calculate(inputAmount, preferredCustomer);

        // Assert

        Assert.Equal(options.Value.BaseRate - options.Value.PreferredCustomerDiscount, result.EffectiveRate);
        Assert.Equal(inputAmount * result.EffectiveRate, result.CalculatedFee);

        loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((value, _) => value.ToString()!.Contains("Calculation started")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
    }

    [Fact]
    public void FeeExceedsCap_ReturnsOkay()
    {
        // Arrange

        ArrangeFeeOptions();
        inputAmount = 100000m;
        preferredCustomer = true;

        FeeCalculatorAppService appService = new(options, loggerMock.Object);

        // Act
        FeeResult result = appService.Calculate(inputAmount, preferredCustomer);

        // Assert

        Assert.Equal(options.Value.MaxFee, result.CalculatedFee);
        Assert.True(result.FeeCapped);

        loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((value, _) => value.ToString()!.Contains("Calculation started")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

        loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((value, _) => value.ToString()!.Contains("Calculation capped")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
    }

    private void ArrangeFeeOptions()
    {
        FeeOptions feeOptions = new();
        options = Options.Create(feeOptions);
        options.Value.BaseRate = 0.05m;
        options.Value.MaxFee = 250.0m;
    }
}
