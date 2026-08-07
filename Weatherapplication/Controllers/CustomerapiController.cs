using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Weatherapplication.Models;

namespace Weatherapplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerapiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomerapiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Save")]
        public IActionResult Save(CustomerMaster obj)
        {
            obj.CreatedDate = DateTime.Now;

            _context.CustomerMaster.Add(obj);

            _context.SaveChanges();

            return Ok();
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            return Ok(_context.CustomerMaster.ToList());
        }

        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(_context.CustomerMaster.Find(id));
        }

        [HttpPut("Update")]
        public IActionResult Update(CustomerMaster obj)
        {
            _context.CustomerMaster.Update(obj);

            _context.SaveChanges();

            return Ok();
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var customer = _context.CustomerMaster.Find(id);

            if (customer == null)
                return NotFound();

            _context.CustomerMaster.Remove(customer);

            _context.SaveChanges();

            return Ok();
        }
    }
}
