namespace Scc.FeeCalculator.Results;

public class FeeResult
{
    public decimal InputAmount { get; set; }
    public bool Preferred {  get; set; }
    public decimal BaseRate { get; set; }
    public decimal EffectiveRate { get; set; }
    public decimal CalculatedFee { get; set; }
    public bool FeeCapped { get; set; }
}
