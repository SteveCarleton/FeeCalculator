using Microsoft.AspNetCore.Mvc;
using Scc.FeeCalculator.AppServices;

namespace Scc.FeeCalculator.Controllers;

[ApiController]
[Route("[controller]")]
public class FeesController : ControllerBase
{
    [HttpGet(Name = "Estimate")]
    public IActionResult Get(IFeeCalculatorAppService calculator, [FromQuery] decimal amount, [FromQuery] bool preferred)
    {
        var result = calculator.Calculate(amount, preferred);
        return Ok(result);
    }
}
