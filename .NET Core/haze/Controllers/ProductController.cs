using haze.DataAccess;
using haze.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using System.Net.NetworkInformation;
using Microsoft.CodeAnalysis;

namespace haze.Controllers
{
    [ApiController]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;

        private HazeContext _hazeContext;
        public ProductController(ILogger<ProductController> logger, HazeContext hazeContext)
        {
            _logger = logger;
            _hazeContext = hazeContext;
        }

        [HttpGet("GetProducts")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            return Ok(await _hazeContext.Products
                .Include(x => x.Categories).Include(x => x.Platforms)
                .ToListAsync());
        }

        [HttpGet("/GetProduct/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> GetProduct(int Id)
        {
            try
            {
                Product product = await _hazeContext.Products
                    .Include(x => x.Categories).ThenInclude(x => x.Id).Where(x => x.Id == Id).FirstOrDefaultAsync();
                return Ok(product);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPost("/AddProduct")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProduct([FromBody] ProductJSON prod)
        {
            Product product = new Product
            {
                Price = prod.Price,
                ProductName = prod.ProductName,
                Description = prod.Description
            };

            _hazeContext.Products.Add(product);

            // Save product
            await _hazeContext.SaveChangesAsync();

            // Check if products options exist
            for (int i = 0; i < prod.CategoryIds.Count; i++)
            {
                var category = await _hazeContext.Categories.Where(x => x.Id == prod.CategoryIds[i]).FirstOrDefaultAsync();

                if (category == null)
                    return BadRequest("Category not found!");

                _hazeContext.ProductCategories.Add(new ProductCategory
                {
                    сategory = category,
                    Id = product.Id
                });
            }

            for (int i = 0; i < prod.PlatformIds.Count; i++)
            {
                var platform = await _hazeContext.Platforms.Where(x => x.Id == prod.PlatformIds[i]).FirstOrDefaultAsync();

                if (platform == null)
                    return BadRequest("Platform not found!");

                _hazeContext.ProductPlatforms.Add(new ProductPlatform
                {
                    platform = platform,
                    Id = product.Id
                });
            }

            // Save platofrms and categories
            await _hazeContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("/DeleteProduct/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteProduct(int Id)
        {
            var e = await _hazeContext.Events
                .Include(x => x.Products).ThenInclude(x => x.Categories)
                .Include(x => x.Products).ThenInclude(x => x.Platforms)
                .Where(x => x.Id == Id).FirstOrDefaultAsync();

            if (e == null)
                return BadRequest("Event not found!");

            _hazeContext.Events.Remove(e);
            await _hazeContext.SaveChangesAsync();

            return Ok(e);
        }

        [HttpPut("/UpdateProduct")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct([FromBody] Event e)
        {
            Event eToUpdate= _hazeContext.Events.Where(p => p.Id == e.Id).First();

            eToUpdate = e;

            await _hazeContext.SaveChangesAsync();

            return Ok();
        }

    }
}
