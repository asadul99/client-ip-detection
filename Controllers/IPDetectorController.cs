using Microsoft.AspNetCore.Mvc;

namespace client_ip_detection.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IPDetectorController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public IPDetectorController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public string GetClientIP()
        {
            //get the clientIP
            var clientIP = _contextAccessor?.HttpContext.Connection?.RemoteIpAddress?.MapToIPv4()?.ToString();
            return clientIP;
        }
    }
}