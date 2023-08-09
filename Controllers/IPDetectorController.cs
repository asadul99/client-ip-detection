using client_ip_detection.Models;

using Microsoft.AspNetCore.Mvc;

namespace client_ip_detection.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IPDetectorController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new User
            {
                Id = Random.Shared.Next(10),
                Name = index.ToString(),
            })
            .ToArray();
        }
    }
}