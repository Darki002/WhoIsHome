using Galaxus.Functional;
using Microsoft.AspNetCore.Mvc;
using WhoIsHome.WebApi.Models;

namespace WhoIsHome.WebApi.Controllers;

public abstract class WhiIsHomeControllerBase<T, TModel> : ControllerBase
{
    protected IActionResult BuildResponse(Result<T, string> result, Func<T, TModel> convert)
    {
        if (result.IsErr)
        {
            return BadRequest(result.Err.Unwrap());
        }

        var model = convert(result.Unwrap());
        return Ok(model);
    }
}