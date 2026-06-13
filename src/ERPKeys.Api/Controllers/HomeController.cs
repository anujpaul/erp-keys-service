using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public string home()
    {
        return "ERP Keys";
    }
}