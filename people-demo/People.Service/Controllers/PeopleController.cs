using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using People.Service.Models;

namespace People.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PeopleController : ControllerBase
    {
        private readonly ILogger<PeopleController> _logger;
        private readonly IPeopleProvider _provider;

        public PeopleController(IPeopleProvider provider,
            ILogger<PeopleController> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IReadOnlyCollection<Person>> Get()
        {
            await Task.Delay(3000);
            return _provider.GetPeople();
        }

        [HttpGet("{id}")]
        public async Task<Person?> Get(int id)
        {
            await Task.Delay(1000);
            return _provider.GetPeople().FirstOrDefault(p => p.Id == id);
        }

        [HttpGet("ids")]
        public IReadOnlyCollection<int> GetIds()
        {
            return _provider.GetPeople().Select(p => p.Id).ToList();
        }
    }
}
