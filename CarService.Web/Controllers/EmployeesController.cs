using CarService.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace CarService.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET /api/employees?cityId=34
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? cityId)
        {
            var list = cityId.HasValue
                ? await _employeeService.GetByCityAsync(cityId.Value)
                : await _employeeService.GetAllAsync();

            var result = list.Select(e => new {
                id        = e.Id,
                firstName = e.FirstName,
                lastName  = e.LastName
            });

            return Ok(result);
        }
    }
}
