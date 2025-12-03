using System;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController: BaseApiController
{
  [HttpGet("not-found")]
  public IActionResult GetNotFound() //404 not found
  {
    return NotFound();
  }


  [HttpGet("bad-request")]
  public IActionResult GetBadRequest()
  {
    return BadRequest("This is not a good request"); // 400 bad request
  }

    [HttpGet("unauthorized")]
  public IActionResult GetUnauthorized() //401 unauthorized
  {
    return Unauthorized();
  }

    [HttpGet("validation-error")]
  public IActionResult GetValidationError()
  {
    ModelState.AddModelError("Problem1", "This is the first error");
    ModelState.AddModelError("Problem2", "This is the second error");
    return ValidationProblem(); //400 bad request
  }

     [HttpGet("server-error")]
  public IActionResult GetServerError() //500 internal server error
  {
    throw new Exception ("This is a server error");
  }
}