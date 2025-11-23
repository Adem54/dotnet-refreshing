using EFDBFirst.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFDBFirst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly NorthwindContext _dbContext;
        public ProductController(NorthwindContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("All", Name = "GetAllProducts")]

        public async Task<IEnumerable<Product>> GetAll()
        {
            //ToListAsync in alti kirmizili cizili geliyor sa o zaman(using Microsoft.EntityFrameworkCore; bunu eklememisizdir), ToList problem olmadan gelyiorsa bu da zaten System.Linq den geldgi icin sorun olmaz..
            return await _dbContext.Products.ToListAsync();
        }


    }
}
