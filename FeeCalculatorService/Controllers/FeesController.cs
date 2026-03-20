using Microsoft.AspNetCore.Mvc;
using Scc.FeeCalculatorService.AppServices;
using Scc.FeeCalculatorService.Results;

namespace Scc.FeeCalculatorService.Controllers;

[ApiController]
[Route("[controller]")]
public class FeesController : ControllerBase
{
    [HttpGet(Name = "Estimate")]
    public IActionResult Get(IFeeCalculatorAppService calculator, [FromQuery] decimal amount, [FromQuery] bool preferred = false)
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
