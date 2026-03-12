using BusinessLayer.IRepository;
using EntityModel.Models;
using Microsoft.AspNetCore.Mvc;


namespace Retailer_webApi.Controllers
{
    /// <summary>
  
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(IUnitOfWork unitOfWork, ILogger<CustomerController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomer()
        {
            var repository = _unitOfWork.Repository<Customer>();
            var customers = await repository.GetAllAsync();
            return Ok(customers);
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var repository = _unitOfWork.Repository<Customer>();
            var customer = await repository.GetByIdAsync(id);

            if (customer == null)
            {
                return NotFound($"customer with ID {id} not found.");
            }

            return Ok(customer);
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var repository = _unitOfWork.Repository<Customer>();
            
            // Check if product with same name already exists
            var existingCustomer = await repository.FirstOrDefaultAsync(p => p.Name == customer.Name);
            if (existingCustomer != null)
            {
                return Conflict($" with name '{customer.Name}' already exists.");
            }

            await repository.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.ID }, customer);
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] Customer customer)
        {
            if (id != customer.ID)
            {
                return BadRequest("Customer ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var repository = _unitOfWork.Repository<Customer>();
            var existingCustomer = await repository.GetByIdAsync(id);

            if (existingCustomer == null)
            {
                return NotFound($"Customer with ID {id} not found.");
            }

            // Update properties
            existingCustomer.Name = customer.Name;
            existingCustomer.Phone = customer.Phone;
            existingCustomer.EmailId = customer.EmailId;
           
           
            repository.Update(existingCustomer);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Delete a customer (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var repository = _unitOfWork.Repository<Customer>();
            var deleted = await repository.SoftDeleteAsync(id);

            if (!deleted)
            {
                return NotFound($"Customer with ID {id} not found.");
            }

            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Search products by category
        /// </summary>
        [HttpGet("customer/{customer}")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomersByEmail(string email)
        {
            var repository = _unitOfWork.Repository<Customer>();
            var customer = await repository.FindAsync(p => p.EmailId== email);
            return Ok(customer);
        }

        /// <summary>
        /// Example of using transactions with Unit of Work
        /// </summary>
        [HttpPost("bulk")]
        public async Task<ActionResult> CreateBulkCustomers([FromBody] List<Customer> customers)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var repository = _unitOfWork.Repository<Customer>();

                foreach (var customer in customers)
                {
                    await repository.AddAsync(customer);
                }

                await _unitOfWork.CommitTransactionAsync();
                return Ok($"Successfully created {customers.Count} customers.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bulk customers");
                return StatusCode(500, "An error occurred while creating customers.");
            }
        }
    }
}
