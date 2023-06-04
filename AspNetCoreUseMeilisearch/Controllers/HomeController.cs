using Domain.Interfaces;
using Meilisearch;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreUseMeilisearch.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<HomeController> _logger;
        private readonly IMeilisearchService _meilisearchService;

        public HomeController(ILogger<HomeController> logger, IMeilisearchService meilisearchService)
        {
            _logger = logger;
            _meilisearchService = meilisearchService;
        }

        [HttpGet]
        public async Task Get()
        {
            await _meilisearchService.GetAllIndexes();
        }
    }
}