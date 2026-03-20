using Microsoft.AspNetCore.Mvc;
using Scc.FeeCalculator.AppServices;
using Scc.FeeCalculator.Results;

namespace Scc.FeeCalculator.Controllers;

[ApiController]
[Route("[controller]")]
public class FeesController : ControllerBase
{
    [HttpGet(Name = "Estimate")]
    public IActionResult Get(IFeeCalculatorAppService calculator, [FromQuery] decimal amount, [FromQuery] bool preferred)
    {
        FeeResult result;

        try
        {
            result = calculator.Calculate(amount, preferred);
        }
        catch (InvalidAmountException exc)
        {
            return BadRequest(exc.Message);
        }

        return Ok(result);
    }
}
