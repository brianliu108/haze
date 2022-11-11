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

        [HttpGet("Products")]
        [Authorize]
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            return Ok(await _hazeContext.Products
                .Include(x => x.Categories).ThenInclude(x => x.сategory)
                .Include(x => x.Platforms).ThenInclude(x => x.platform)
                .ToListAsync());
        }

        [HttpGet("/Product/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> GetProduct(int Id)
        {
            Product product = await _hazeContext.Products
                .Include(x => x.Categories).ThenInclude(x => x.сategory)
                    .Include(x => x.Platforms).ThenInclude(x => x.platform)
                    .Where(x => x.Id == Id).FirstOrDefaultAsync();
            if(product == null)
                return BadRequest("Product dont exist!");
            return Ok(product);
        }

        [HttpPost("/Product")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProduct([FromBody] ProductJSON prod)
        {
            Product product = new Product
            {
                Price = prod.Price,
                ProductName = prod.ProductName,
                Description = prod.Description,
                Categories = new List<ProductCategory>(),
                Platforms = new List<ProductPlatform>()
            };

            _hazeContext.Products.Add(product);

            // Save product

            // Check if products options exist
            for (int i = 0; i < prod.CategoryIds.Count; i++)
            {
                var categoryBlya = await _hazeContext.Categories.Where(x => x.Id == prod.CategoryIds[i]).FirstOrDefaultAsync();

                if (categoryBlya == null)
                    return BadRequest("Category not found!");

                product.Categories.Add(new ProductCategory
                {
                    сategory = categoryBlya
                });
            }

            for (int i = 0; i < prod.PlatformIds.Count; i++)
            {
                var platformBlya = await _hazeContext.Platforms.Where(x => x.Id == prod.PlatformIds[i]).FirstOrDefaultAsync();

                if (platformBlya == null)
                    return BadRequest("Platform not found!");

                product.Platforms.Add(new ProductPlatform
                {
                    platform = platformBlya
                });
            }

            // Save platofrms and categories
            await _hazeContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("/Product/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteProduct(int Id)
        {
            Product product = await _hazeContext.Products
                .Include(x => x.Categories).ThenInclude(x => x.сategory)
                    .Include(x => x.Platforms).ThenInclude(x => x.platform)
                    .Where(x => x.Id == Id).FirstOrDefaultAsync();
            if (product == null)
                return BadRequest("Product dont exist!");

            _hazeContext.Products.Remove(product);
            await _hazeContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("/Product")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductJSON prod)
        {
            Product product = await _hazeContext.Products
                .Include(x => x.Categories).ThenInclude(x => x.сategory)
                .Include(x => x.Platforms).ThenInclude(x => x.platform)
                .Where(x => x.Id == prod.Id).FirstOrDefaultAsync();

            if (product == null)
                return BadRequest("Product dont exist!");

            for (int i = 0; i < product.Categories.Count; i++)
            {
                _hazeContext.ProductCategories.Remove(product.Categories[i]);
            }

            for (int i = 0; i < product.Platforms.Count; i++)
            {
                _hazeContext.ProductPlatforms.Remove(product.Platforms[i]);
            }

            product.Price = prod.Price;
            product.ProductName = prod.ProductName;
            product.Description = prod.Description;
            product.Platforms = new List<ProductPlatform>();
            product.Categories = new List<ProductCategory>();

            // Check if products options exist
            for (int i = 0; i < prod.CategoryIds.Count; i++)
            {
                var categoryBlya = await _hazeContext.Categories.Where(x => x.Id == prod.CategoryIds[i]).FirstOrDefaultAsync();

                if (categoryBlya == null)
                    return BadRequest("Category not found!");

                product.Categories.Add(new ProductCategory
                {
                    сategory = categoryBlya
                });
            }

            for (int i = 0; i < prod.PlatformIds.Count; i++)
            {
                var platformBlya = await _hazeContext.Platforms.Where(x => x.Id == prod.PlatformIds[i]).FirstOrDefaultAsync();

                if (platformBlya == null)
                    return BadRequest("Platform not found!");

                product.Platforms.Add(new ProductPlatform
                {
                    platform = platformBlya
                });
            }

            await _hazeContext.SaveChangesAsync();

            return Ok();
        }
    }
}