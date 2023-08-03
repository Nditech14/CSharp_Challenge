using CSharp_Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSharp_Challenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public AccessController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public List<string> GetFruits()
        {
            var allFruits = _appDbContext.fruits.ToList();

            var fruitsName = new List<string>();
            foreach (var fruit in allFruits)
            {
                fruitsName.Add(fruit.Name);
            }
            return fruitsName;
        }
    }
}
