using BusinessLayer.IRepository;
using EntityModel.Models;
using Microsoft.AspNetCore.Mvc;


namespace Retailer_webApi.Controllers
{
    /// <summary>
  
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IUnitOfWork unitOfWork, ILogger<EmployeeController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetCustomer()
        {
            var repository = _unitOfWork.Repository<Employee>();
            var Employee = await repository.GetAllAsync();
            return Ok(Employee);
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var repository = _unitOfWork.Repository<Employee>();
            var employee = await repository.GetByIdAsync(id);

            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }

            return Ok(employee);
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee([FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var repository = _unitOfWork.Repository<Employee>();
            
            // Check if product with same name already exists
            var existingEmployee = await repository.FirstOrDefaultAsync(p => p.Name == employee.Name);
            if (existingEmployee != null)
            {
                return Conflict($"existEmployee with name '{employee.Name}' already exists.");
            }

            await repository.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = employee.ID }, employee);
        }

        /// <summary>
        /// Update an existing employee
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            if (id != employee.ID)
            {
                return BadRequest("Employee ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var repository = _unitOfWork.Repository<Employee>();
            var existingEmployee = await repository.GetByIdAsync(id);

            if (existingEmployee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }

            // Update properties
            existingEmployee.Name = employee.Name;
            existingEmployee.Role = employee.Role;
            existingEmployee.Orders = employee.Orders;
           
            repository.Update(existingEmployee);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Delete a product (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var repository = _unitOfWork.Repository<Employee>();
            var deleted = await repository.SoftDeleteAsync(id);

            if (!deleted)
            {
                return NotFound($"Employee with ID {id} not found.");
            }

            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Search products by category
        /// </summary>
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetProductsByCategory(string category)
        {
            var repository = _unitOfWork.Repository<Employee>();
            var employee = await repository.FindAsync(p => p.Name == p.Name);
            return Ok(employee);
        }

        /// <summary>
        /// Example of using transactions with Unit of Work
        /// </summary>
        [HttpPost("bulk")]
        public async Task<ActionResult> CreateBulkProducts([FromBody] List<Employee> employees)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var repository = _unitOfWork.Repository<Employee>();

                foreach (var employee in employees)
                {
                    await repository.AddAsync(employee);
                }

                await _unitOfWork.CommitTransactionAsync();
                return Ok($"Successfully created {employees.Count} Employees.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bulk Employees");
                return StatusCode(500, "An error occurred while creating Employees.");
            }
        }
    }
}
