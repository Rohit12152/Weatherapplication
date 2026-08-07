using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.Style;
using System.Data;
using Weatherapplication.Models;

namespace Weatherapplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseInvoiceapiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PurchaseInvoiceapiController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost("Save")]
        public IActionResult Save(PurchaseInvoiceDetail obj)
        {
            using var transaction = _context.Database.BeginTransaction();
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            try
            {
                obj.PurchaseInvoiceDate = DateTime.Now;
                obj.UserId = userId;
                // Parent Save
                _context.PurchaseInvoiceDetail.Add(obj);
                _context.SaveChanges();

                // Child Save
                foreach (var item in obj.PurchaseInvoiceItem)
                {
                    item.PurchaseInvoiceId = obj.Id;

                    _context.PurchaseInvoiceItemDetail.Add(item);
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
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var data = (from pi in _context.PurchaseInvoiceDetail
                        join c in _context.CustomerMaster
                            on pi.CustomerId equals c.Id into cust
                        from c in cust.DefaultIfEmpty()
                        select new
                        {
                            pi.Id,
                            pi.PurchaseInvoiceNo,
                            pi.Reference,
                            pi.PurchaseInvoiceDate,
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
            var invoice = _context.PurchaseInvoiceDetail.FirstOrDefault(x => x.Id == id);

            if (invoice == null)
                return NotFound();

            invoice.PurchaseInvoiceItem = _context.PurchaseInvoiceItemDetail.Where(x => x.PurchaseInvoiceId == id).ToList();

            return Ok(invoice);
        }

        [HttpPut("Update")]
        public IActionResult Update([FromBody] PurchaseInvoiceDetail obj)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                // Parent Update
                var invoice = _context.PurchaseInvoiceDetail.FirstOrDefault(x => x.Id == obj.Id);

                if (invoice == null)
                    return NotFound();

                invoice.Reference = obj.Reference;
                invoice.CustomerId = obj.CustomerId;
                invoice.PurchaseInvoiceDate = obj.PurchaseInvoiceDate;
                invoice.TotalAmount = obj.TotalAmount;
                invoice.TotalTax = obj.TotalTax;
                invoice.NetAmount = obj.NetAmount;

                _context.SaveChanges();

                // Purane Items Delete
                var oldItems = _context.PurchaseInvoiceItemDetail
                                       .Where(x => x.PurchaseInvoiceId == obj.Id)
                                       .ToList();

                _context.PurchaseInvoiceItemDetail.RemoveRange(oldItems);
                _context.SaveChanges();

                // Naye Items Insert
                foreach (var item in obj.PurchaseInvoiceItem)
                {
                    item.Id = 0; // New Insert
                    item.PurchaseInvoiceId = obj.Id;

                    _context.PurchaseInvoiceItemDetail.Add(item);
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
            var customer = _context.PurchaseInvoiceDetail.Find(id);

            if (customer == null)
                return NotFound();

            _context.PurchaseInvoiceDetail.Remove(customer);

            _context.SaveChanges();

            return Ok();
        }
        [HttpGet("GetCustomerList")]
        public IActionResult GetCustomerList()
        {
            int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            var data = _context.CustomerMaster
                .Where(x => x.CustomerName != null && x.CustomerName != "" && x.UserId == userId)
                .OrderBy(x => x.CustomerName)
                .Select(x => new
                {
                    x.Id,
                    x.CustomerName
                })
                .ToList();

            return Ok(data);
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
        private async Task<string> GenerateNextPurchaseInvoiceNo()
        {
            string invoiceNo = "";

            using var command = _context.Database.GetDbConnection().CreateCommand();

            command.CommandText = "sp_GetNextPurchaseInvoiceNo";
            command.CommandType = CommandType.StoredProcedure;

            await _context.Database.OpenConnectionAsync();

            var result = await command.ExecuteScalarAsync();

            if (result != null)
                invoiceNo = result.ToString();

            await _context.Database.CloseConnectionAsync();

            return invoiceNo;
        }
        [HttpGet("GetNextPurchaseInvoiceNo")]
        public async Task<IActionResult> GetNextPurchaseInvoiceNo()
        {
            var invoiceNo = await GenerateNextPurchaseInvoiceNo();
            return Ok(invoiceNo);
        }

        [HttpGet("ConverttoPurchaseInvoice/{id}")]
        public async Task<IActionResult> ConverttoPurchaseInvoice(int id)
        {
            var purchase = await _context.PurchaseDetail.FirstOrDefaultAsync(x => x.Id == id);

            if (purchase == null)
                return NotFound();

            PurchaseInvoiceDetail model = new PurchaseInvoiceDetail();

            model.PurchaseInvoiceNo = await GenerateNextPurchaseInvoiceNo();
            model.poid = purchase.Id;
            model.Reference = purchase.PoNo;
            model.CustomerId = purchase.StudentId;
            model.PurchaseInvoiceDate = DateTime.Now;
            model.TotalAmount = purchase.TotalAmount;
            model.TotalTax = purchase.TotalTax;
            model.NetAmount = purchase.NetAmount;
            model.UserId = purchase.UserId;

            model.PurchaseInvoiceItem = await _context.PurchaseItemDetail
                                                       .Where(x => x.PoId == id)
                                                       .Select(x => new PurchaseInvoiceItemDetail
                                                       {
                                                           ItemId = x.ItemId,
                                                           Qty = x.Qty,
                                                           Rate = x.Rate,
                                                           Amount = x.Amount,
                                                           TaxAmount = x.TaxAmount,
                                                           TotalAmount = x.TotalAmount
                                                       }).ToListAsync();

            return Ok(model);
        }
    }
}
