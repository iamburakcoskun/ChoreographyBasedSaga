using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stock.API.Models;

namespace Stock.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StocksController(AppDbContext context)
        {
            _context = context;
        }
    }
}
