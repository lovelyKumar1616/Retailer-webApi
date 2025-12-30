using BusinessLayer.IRepository;
using EntityModel.Models;
using Microsoft.AspNetCore.Mvc;


namespace Retailer_webApi.Controllers
{
    /// <summary>
  
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IUnitOfWork unitOfWork, ILogger<InventoryController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Get all inventories
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetAllInventory()
        {
            var repository = _unitOfWork.Repository<Inventory>();
            var inventory = await repository.GetAllAsync();
            return Ok(inventory);
        }

        /// <summary>
        /// Get inventory by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Inventory>> GetInventory(int id)
        {
            var repository = _unitOfWork.Repository<Inventory>();
            var Inventory = await repository.GetByIdAsync(id);

            if (Inventory == null)
            {
                return NotFound($"Inventory with ID {id} not found.");
            }

            return Ok(Inventory);
        }

        /// <summary>
        /// Create a new inventory
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Inventory>> CreateInventory([FromBody] Inventory inventory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var repository = _unitOfWork.Repository<Inventory>();
            
            // Check if inventory with same name already exists
            var existingExisting = await repository.FirstOrDefaultAsync(p => p.ID == inventory.ID);
            if (existingExisting != null)
            {
                return Conflict($"Inventory with name '{inventory.ID}' already exists.");
            }

           
          


            await repository.AddAsync(inventory);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateInventory), new { id = inventory.ID }, inventory);
        }

        /// <summary>
        /// Update an existing inventory
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInventory(int id, [FromBody] Inventory inventory)
        {
            if (id != inventory.ID)
            {
                return BadRequest("inventory ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var repository = _unitOfWork.Repository<Inventory>();
            var existingInventory = await repository.GetByIdAsync(id);

            if (existingInventory == null)
            {
                return NotFound($"Inventory with ID {id} not found.");
            }

            // Update properties
            existingInventory.QuantityAvailable = existingInventory.QuantityAvailable;
            existingInventory.Product = existingInventory.Product;
            existingInventory.ProductId = existingInventory.ProductId;
           
            repository.Update(existingInventory);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Delete a inventory (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var repository = _unitOfWork.Repository<Inventory>();
            var deleted = await repository.SoftDeleteAsync(id);

            if (!deleted)
            {
                return NotFound($"Inventory with ID {id} not found.");
            }

            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Search inventory by category
        /// </summary>
        
        /// <summary>
        /// Example of using transactions with Unit of Work
        /// </summary>
        [HttpPost("bulk")]
        public async Task<ActionResult> CreateBulkInventories([FromBody] List<Inventory> inventory)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var repository = _unitOfWork.Repository<Inventory>();

                foreach (var invent in inventory)
                {
                    await repository.AddAsync(invent);
                }

                await _unitOfWork.CommitTransactionAsync();
                return Ok($"Successfully created {inventory.Count} Inventory.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bulk Inventory");
                return StatusCode(500, "An error occurred while creating Inventory.");
            }
        }
    }
}
