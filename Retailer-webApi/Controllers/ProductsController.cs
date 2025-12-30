using BusinessLayer.IRepository;
using EntityModel.Models;
using Microsoft.AspNetCore.Mvc;


namespace Retailer_webApi.Controllers
{
    /// <summary>
  
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IUnitOfWork unitOfWork, ILogger<ProductsController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var repository = _unitOfWork.Repository<Product>();
            var products = await repository.GetAllAsync();
            return Ok(products);
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var repository = _unitOfWork.Repository<Product>();
            var product = await repository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            return Ok(product);
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var repository = _unitOfWork.Repository<Product>();
            
            // Check if product with same name already exists
            var existingProduct = await repository.FirstOrDefaultAsync(p => p.Name == product.Name);
            if (existingProduct != null)
            {
                return Conflict($"Product with name '{product.Name}' already exists.");
            }

            await repository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.ID }, product);
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (id != product.ID)
            {
                return BadRequest("Product ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var repository = _unitOfWork.Repository<Product>();
            var existingProduct = await repository.GetByIdAsync(id);

            if (existingProduct == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            // Update properties
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.Category = product.Category;

            repository.Update(existingProduct);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Delete a product (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var repository = _unitOfWork.Repository<Product>();
            var deleted = await repository.SoftDeleteAsync(id);

            if (!deleted)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Search products by category
        /// </summary>
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(string category)
        {
            var repository = _unitOfWork.Repository<Product>();
            var products = await repository.FindAsync(p => p.Category == category);
            return Ok(products);
        }

        /// <summary>
        /// Example of using transactions with Unit of Work
        /// </summary>
        [HttpPost("bulk")]
        public async Task<ActionResult> CreateBulkProducts([FromBody] List<Product> products)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var repository = _unitOfWork.Repository<Product>();

                foreach (var product in products)
                {
                    await repository.AddAsync(product);
                }

                await _unitOfWork.CommitTransactionAsync();
                return Ok($"Successfully created {products.Count} products.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bulk products");
                return StatusCode(500, "An error occurred while creating products.");
            }
        }
    }
}
