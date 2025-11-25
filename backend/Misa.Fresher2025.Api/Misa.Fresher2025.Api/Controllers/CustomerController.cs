using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Misa.Core.DTo;
using Misa.Core.Entities;
using Misa.Core.Interfaces.Service;

namespace Misa.Fresher2025.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : BaseController<Customer, string, string[], CustomerDto>
    {
        private readonly ICustomerService _service;
        public CustomerController(ICustomerService service)
        : base(service)
        {
            _service = service;
        }
        [HttpGet("create-code")]
        public IActionResult CreateCode()
        {
            var code = _service.CreateCustomerCode();
            return Ok(code);
        }
        [HttpPost("check-email")]
        public IActionResult CheckEmail([FromBody] string email)
        {
            var code = _service.CheckEmail(email);
            return Ok(code);
        }
        [HttpPost("check-phone")]
        public IActionResult CheckPhone([FromBody] string phone)
        {
            var code = _service.CheckPhone(phone);
            return Ok(code);
        }
        [HttpPost("import-excel")]
        public IActionResult ExportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ");
            using var stream = file.OpenReadStream();
            var errors = _service.ImportCustomers(stream);
            return Ok(errors);
        }
    }
}
