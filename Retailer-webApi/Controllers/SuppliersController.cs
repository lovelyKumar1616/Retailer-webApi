using Microsoft.AspNetCore.Mvc;

namespace Retailer_webApi.Controllers
{
    using BusinessLayer.IRepository;
    using EntityModel.Models;
    using Microsoft.AspNetCore.Mvc;


    namespace Retailer_webApi.Controllers
    {
        /// <summary>

        /// </summary>
        [ApiController]
        [Route("api/[controller]")]
        public class SuppliersController : ControllerBase
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ILogger<SuppliersController> _logger;

            public SuppliersController(IUnitOfWork unitOfWork, ILogger<SuppliersController> logger)
            {
                _unitOfWork = unitOfWork;
                _logger = logger;
            }

            /// <summary>
            /// Get all suppliers
            /// </summary>
            [HttpGet]
            public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers()
            {
                var repository = _unitOfWork.Repository<Supplier>();
                var products = await repository.GetAllAsync();
                return Ok(products);
            }

            /// <summary>
            /// Get supplier by ID
            /// </summary>
            [HttpGet("{id}")]
            public async Task<ActionResult<Supplier>> GetSupplier(int id)
            {
                var repository = _unitOfWork.Repository<Supplier>();
                var product = await repository.GetByIdAsync(id);

                if (product == null)
                {
                    return NotFound($"Supplier with ID {id} not found.");
                }

                return Ok(product);
            }

            /// <summary>
            /// Create a new supplier
            /// </summary>
            [HttpPost]
            public async Task<ActionResult<Supplier>> CreateSupplier([FromBody] Supplier supplier)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                

                var repository = _unitOfWork.Repository<Supplier>();

                // Check if supplier with same name already exists
                var existingSupplier = await repository.FirstOrDefaultAsync(p => p.Name == supplier.Name);
                if (existingSupplier != null)
                {
                    return Conflict($"Supplier with name '{supplier.Name}' already exists.");
                }

                await repository.AddAsync(supplier);
                await _unitOfWork.SaveChangesAsync();

                return CreatedAtAction(nameof(GetSupplier), new { id = supplier.Id }, supplier);
            }

            /// <summary>
            /// Update an existing supplier
            /// </summary>
            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateSupplier(int id, [FromBody] Supplier supplier)
            {
                if (id != supplier.Id)
                {
                    return BadRequest("Supplier ID mismatch.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var repository = _unitOfWork.Repository<Supplier>();
                var existingSupplier = await repository.GetByIdAsync(id);

                if (existingSupplier == null)
                {
                    return NotFound($"Supplier with ID {id} not found.");
                }

                // Update properties
                existingSupplier.Name = supplier.Name;
                existingSupplier.ContactInfo = supplier.ContactInfo;
               
                

                repository.Update(existingSupplier);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }

            /// <summary>
            /// Delete a suppliers (soft delete)
            /// </summary>
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteSupplier(int id)
            {
                var repository = _unitOfWork.Repository<Supplier>();
                var deleted = await repository.SoftDeleteAsync(id);

                if (!deleted)
                {
                    return NotFound($"Supplier with ID {id} not found.");
                }

                await _unitOfWork.SaveChangesAsync();
                return NoContent();
            }

           

            /// <summary>
            /// Example of using transactions with Unit of Work
            /// </summary>
            [HttpPost("bulk")]
            public async Task<ActionResult> CreateBulkSuppliers([FromBody] List<Supplier> suppliers)
            {
                try
                {
                    await _unitOfWork.BeginTransactionAsync();

                    var repository = _unitOfWork.Repository<Supplier>();

                    foreach (var supplier in suppliers)
                    {
                        await repository.AddAsync(supplier);
                    }

                    await _unitOfWork.CommitTransactionAsync();
                    return Ok($"Successfully created {suppliers.Count} suppliers.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating bulk suppliers");
                    return StatusCode(500, "An error occurred while creating suppliers.");
                }
            }
        }
    }

}
