using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  
    public class ProductsController(StoreContext context) : BaseApiController
    {
        [HttpGet]   //api/products
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            return await context.Products.ToListAsync(); //call to database
        }

        [HttpGet("{id}")] //api/products/2
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await context.Products.FindAsync(id); //call to database 

            if (product == null) return NotFound();
            
            return product;
        }
  }
}
