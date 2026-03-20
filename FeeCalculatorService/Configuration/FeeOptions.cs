namespace Scc.FeeCalculatorService.Configuration;

public class FeeOptions
{
    public decimal BaseRate { get; set; }
    public decimal PreferredCustomerDiscount { get; set; }
    public decimal MaxFee { get; set; }
}
