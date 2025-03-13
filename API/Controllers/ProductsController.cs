using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IGenericRepository<Product> repo) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery]ProductSpecParams specParams){
            // Cách 1: Lấy trực tiếp từ ProductRepository
            // var products = await repo.ListAllAsync();
            // return Ok(products);

            // Cách 2: Xữ lý từ IGenericRepository
            /*
            var spec = new ProductSpecification(specParams);
            var products = await repo.ListAsync(spec);
            var count = await repo.CountAsync(spec);

            var pagination = new Pagination<Product>(specParams.PageIndex, specParams.PageSize, count, products);

            return Ok(pagination);
            */

            // Cách 3: Thừa kế BaseApiController
            var spec = new ProductSpecification(specParams);
            return await CreatePagedResult(repo, spec, specParams.PageIndex, specParams.PageSize);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id){
            var product = await repo.GetByIdAsync(id);

            if(product == null) return NotFound();

            return product;
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands(){
            var spec = new BrandListSpecification();
            return Ok(await repo.ListAsync(spec));
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes(){
            var spec = new TypeListSpecification();
            return Ok(await repo.ListAsync(spec));
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product){
            repo.Add(product);
        
            if(await repo.SaveAllAsync()){
                return CreatedAtAction("GetProduct", new { id = product.Id }, product);
            }

            return BadRequest("Problem creating the product");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product){
            if(product.Id != id || !ProductExists(id)){
                return BadRequest("Cannot update this product");
            }

            repo.Update(product);

            if(await repo.SaveAllAsync()){
                return NoContent();
            }

            return BadRequest("Problem updating the product");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id){
            var product = await repo.GetByIdAsync(id);

            if(product == null) return NotFound();

            repo.Remove(product);

            if(await repo.SaveAllAsync()){
                return NoContent();
            }

            return BadRequest("Problem deleting the product");
        }

        private bool ProductExists(int id){
            return repo.Exists(id);
        }
    }
}
