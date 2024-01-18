using Core.DataValidators;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace inventory_management_system_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryItemController : ControllerBase
    {
        private readonly IInventoryItemService _inventoryItemService;
        private readonly ILogger<InventoryItemController> _logger;
        public InventoryItemController(ILogger<InventoryItemController> logger, IInventoryItemService inventoryItemService)
        {
            _logger = logger;
            _inventoryItemService = inventoryItemService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? serial, [FromQuery] string? name, [FromQuery] string? date)
        {
            List<InventoryItem> inventoryItems = await _inventoryItemService.GetAll();

            if (serial is not null)
                inventoryItems = await _inventoryItemService.GetItemBySerial(serial);
            else if (name is not null)
                inventoryItems = await _inventoryItemService.GetItemByName(name);
            else if (date is not null)
            {
                DateTime result;
                if (!DateTime.TryParseExact(date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out result))
                    return BadRequest("Invalid date format");

                inventoryItems = await _inventoryItemService.GetItemByDate(result);
            }

            var items = new List<ReturnInventoryItem>();
            foreach (var item in inventoryItems)
            {
                var inventoryItemToReturn = new ReturnInventoryItem()
                {
                    id = item.Id,
                    serialimei = item.Serial,
                    name = item.Name,
                    supplier = item.Supplier,
                    date = item.Date.ToString("dd/MM/yyyy"),
                    quantity = item.Quantity,
                    notes = item.Notes,
                    alvlp = item.ALVLP,
                    ul = item.UL,
                    mdm = item.MDM,
                    reset = item.Reset,
                    gtg = item.GTG
                };

                items.Add(inventoryItemToReturn);
            }

            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Post(InsertInventoryValidator insertInventoryValidator)
        {
            foreach (var item in insertInventoryValidator.InventoryItems)
            {
                var inventoryItem = new InventoryItem()
                {
                    Serial = item.serialimei.ToString(),
                    Name = item.name,
                    Supplier = item.supplier,
                    Date = item.date,
                    Quantity = item.quantity,
                    Notes = item.notes,
                    ALVLP = item.alvlp,
                    UL = item.ul,
                    MDM = item.mdm,
                    Reset = item.reset,
                    GTG = item.gtg
                };

                if (!await _inventoryItemService.Create(inventoryItem))
                {
                    return BadRequest("An error has occured...");
                }
            }

            return Ok("Successful!");
        }

        [HttpPost("UpdateDate")]
        public async Task<IActionResult> UpdateDate(InsertInventoryValidator updateInventoryValidator)
        {
            foreach(var item in updateInventoryValidator.InventoryItems)
            {
                var inventoryItem = new InventoryItem()
                {
                    Id = item.Id.Value,
                    Serial = item.serialimei.ToString(),
                    Name = item.name,
                    Supplier = item.supplier,
                    Date = item.date,
                    Quantity = item.quantity,
                    Notes = item.notes,
                    ALVLP = item.alvlp,
                    UL = item.ul,
                    MDM = item.mdm,
                    Reset = item.reset,
                    GTG = item.gtg
                };

                if (!await _inventoryItemService.UpdateItem(inventoryItem))
                {
                    return BadRequest("An error has occured...");
                }
            }

            return Ok("Successful!");
        }
        
        [HttpGet("GetAllDates")]
        public async Task<IActionResult> GetAllDates()
        {
            var dates = _inventoryItemService.GetDistinctDates().Result;
            var distinctDates = new List<string>();

            foreach (var date in dates)
            {
                distinctDates.Add(date.ToString("dd/MM/yyyy"));
            }

            return Ok(distinctDates);
        }

        [HttpGet("GetInventoryByDate")]
        public async Task<IActionResult> GetInventoryByDate([Required][FromQuery] string date)
        {
            DateTime dateTime;

            if (DateTime.TryParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                var inventoryItems = await _inventoryItemService.GetInventoryByDate(dateTime);

                var items = new List<ReturnInventoryItem>();

                foreach (var item in inventoryItems)
                {
                    var inventoryItemToReturn = new ReturnInventoryItem()
                    {
                        serialimei = item.Serial,
                        name = item.Name,
                        supplier = item.Supplier,
                        date = item.Date.ToString("dd/MM/yyyy"),
                        quantity = item.Quantity,
                        notes = item.Notes,
                        alvlp = item.ALVLP,
                        ul = item.UL,
                        mdm = item.MDM,
                        reset = item.Reset,
                        gtg = item.GTG
                    };

                    items.Add(inventoryItemToReturn);
                }

                return Ok(items);
            }
            else
            {
                return BadRequest("Something went wrong when parsing your date...");
            }
        }
    }
}