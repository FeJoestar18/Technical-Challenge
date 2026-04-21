using Microsoft.AspNetCore.Mvc;

namespace Technical_Challenge.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public string Get() => "API funcionando";
    }
}
