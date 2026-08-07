using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Weatherapplication.Models;

namespace Weatherapplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryOrderapiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DeliveryOrderapiController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost("Save")]
        public IActionResult Save(DeliveryOrderDetail obj)
        {
            using var transaction = _context.Database.BeginTransaction();
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            try
            {
                obj.DODate = DateTime.Now;
                obj.UserId = userId;
                // Parent Save
                _context.DeliveryOrderDetail.Add(obj);
                _context.SaveChanges();

                // Child Save
                foreach (var item in obj.DeliveryOrderItem)
                {
                    item.DOId = obj.Id;

                    _context.DeliveryOrderItemDetail.Add(item);
                }

                _context.SaveChanges();

                transaction.Commit();

                return Ok(new
                {
                    message = "Saved Successfully",
                    InvoiceId = obj.Id
                });
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                var msg = ex.Message;

                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    msg += " --> " + ex.Message;
                }

                return BadRequest(msg);
            }
        }
        [HttpGet("GetItemist")]
        public IActionResult GetItemist()
        {
            var data = _context.ItemMaster
                .Where(x => x.ItemName != null && x.ItemName != "")
                .OrderBy(x => x.ItemName)
                .Select(x => new
                {
                    x.Id,
                    x.ItemName
                })
                .ToList();

            return Ok(data);
        }
        [HttpGet("GetItemcategory")]
        public IActionResult GetItemcategory()
        {
            var data = _context.Categories
                .Where(x => x.CategoryName != null && x.CategoryName != "")
                .OrderBy(x => x.CategoryName)
                .Select(x => new
                {
                    x.CategoryId,
                    x.CategoryName
                })
                .ToList();

            return Ok(data);
        }
        [HttpGet("GetItemListByCategory")]
        public IActionResult GetItemListByCategory(int categoryId)
        {
            var data = _context.ItemMaster
                .Where(x => x.categoryid == categoryId
                         && x.ItemName != null
                         && x.ItemName != "")
                .OrderBy(x => x.ItemName)
                .Select(x => new
                {
                    x.Id,
                    x.ItemName
                })
                .ToList();

            return Ok(data);
        }
        [HttpGet("GetCustomerList")]
        public IActionResult GetCustomerList()
        {
            int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            var data = _context.CustomerMaster
                .Where(x => x.CustomerName != null && x.CustomerName != "" && x.UserId == userId && x.partytype == 1 && x.IsActive == true)
                .OrderBy(x => x.CustomerName)
                .Select(x => new
                {
                    x.Id,
                    x.CustomerName
                })
                .ToList();

            return Ok(data);
        }
        private async Task<string> GenerateNextDONo()
        {
            string invoiceNo = "";

            using var command = _context.Database.GetDbConnection().CreateCommand();

            command.CommandText = "sp_GetNextDeliveryOrderDetail";
            command.CommandType = CommandType.StoredProcedure;

            await _context.Database.OpenConnectionAsync();

            var result = await command.ExecuteScalarAsync();

            if (result != null)
                invoiceNo = result.ToString();

            await _context.Database.CloseConnectionAsync();

            return invoiceNo;
        }
        [HttpGet("GetNextDONo")]
        public async Task<IActionResult> GetNextDONo()
        {
            var invoiceNo = await GenerateNextDONo();
            return Ok(invoiceNo);
        }

        [HttpGet("ConverttoDO/{id}")]
        public async Task<IActionResult> ConverttoDO(int id)
        {
            var sales = await _context.SalesDetail.FirstOrDefaultAsync(x => x.Id == id);

            if (sales == null)
                return NotFound();

            DeliveryOrderDetail model = new DeliveryOrderDetail();

            model.DONo = await GenerateNextDONo();
            model.SOId = sales.Id;
            model.Reference = sales.SalesNo;
            model.CustomerId = sales.StudentId;
            model.DODate = DateTime.Now;
            model.TotalAmount = sales.TotalAmount;
            model.TotalTax = sales.TotalTax;
            model.NetAmount = sales.NetAmount;
            model.UserId = sales.UserId;

            model.DeliveryOrderItem = await ( from s in _context.SalesItemDetail  join i in _context.ItemMaster
                                              on s.ItemId equals i.Id  where s.SalesId == id
                                         select new DeliveryOrderItemDetail
                                         {
                                             ItemId = s.ItemId,
                                             categoryid = i.categoryid, 
                                             SOQty = s.Qty,
                                             Rate = s.Rate,
                                             GST = s.GST
                                         }).ToListAsync();
                  return Ok(model);
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var data = (from pi in _context.DeliveryOrderDetail
                        join c in _context.CustomerMaster
                            on pi.CustomerId equals c.Id into cust
                        from c in cust.DefaultIfEmpty()
                        select new
                        {
                            pi.Id,
                            pi.DONo,
                            pi.Reference,
                            pi.DODate,
                            pi.CustomerId,
                            CustomerName = c != null ? c.CustomerName : "",
                            pi.TotalAmount,
                            pi.TotalTax,
                            pi.NetAmount
                        }).ToList();

            return Ok(data);
        }
        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            var invoice = _context.DeliveryOrderDetail.FirstOrDefault(x => x.Id == id);

            if (invoice == null)
                return NotFound();

            invoice.DeliveryOrderItem = _context.DeliveryOrderItemDetail.Where(x => x.DOId == id).ToList();

            return Ok(invoice);
        }

        [HttpPut("Update")]
        public IActionResult Update([FromBody] DeliveryOrderDetail obj)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                // Parent Update
                var invoice = _context.DeliveryOrderDetail.FirstOrDefault(x => x.Id == obj.Id);

                if (invoice == null)
                    return NotFound();

                invoice.Reference = obj.Reference;
                invoice.CustomerId = obj.CustomerId;
                invoice.DODate = obj.DODate;
                invoice.TotalAmount = obj.TotalAmount;
                invoice.TotalTax = obj.TotalTax;
                invoice.NetAmount = obj.NetAmount;

                _context.SaveChanges();

                // Purane Items Delete
                var oldItems = _context.DeliveryOrderItemDetail
                                       .Where(x => x.DOId == obj.Id)
                                       .ToList();

                _context.DeliveryOrderItemDetail.RemoveRange(oldItems);
                _context.SaveChanges();

                // Naye Items Insert
                foreach (var item in obj.DeliveryOrderItem)
                {
                    item.Id = 0; // New Insert
                    item.DOId = obj.Id;

                    _context.DeliveryOrderItemDetail.Add(item);
                }

                _context.SaveChanges();

                transaction.Commit();

                return Ok(new
                {
                    message = "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var customer = _context.DeliveryOrderDetail.Find(id);

            if (customer == null)
                return NotFound();

            _context.DeliveryOrderDetail.Remove(customer);

            _context.SaveChanges();

            return Ok();
        }
    }
}
