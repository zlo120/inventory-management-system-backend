using Core.DataValidators.Inventory;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace inventory_management_system_backend.Controllers
{
    [Authorize(Policy = "BearerToken")]
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryItemController : ControllerBase
    {
        private readonly IInventoryItemService _inventoryItemService;
        private readonly IStatusService _statusService;
        private readonly ILocationService _locationService;
        private readonly ILogger<InventoryItemController> _logger;
        public InventoryItemController(ILogger<InventoryItemController> logger, IStatusService statusService, IInventoryItemService inventoryItemService, ILocationService locationService)
        {
            _logger = logger;
            _inventoryItemService = inventoryItemService;
            _locationService = locationService;
            _statusService = statusService;
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
                    gtg = item.GTG,
                    Status = item.Status,
                    Location = item.Location,
                };

                items.Add(inventoryItemToReturn);
            }

            return Ok(items);
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
                        gtg = item.GTG,
                        Status = item.Status,
                        Location = item.Location,
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

        [HttpGet("GetInventoryByLocation")]
        public async Task<IActionResult> GetInventoryByLocation([Required][FromQuery] int locationId)
        {
            try
            {
                var inventory = await _locationService.GetAllInventory(locationId);
                return Ok(inventory);
            }
            catch (LocationNotFoundException)
            {
                return BadRequest($"A location with ID {locationId} does not exist!");
            }

        }

        [HttpGet("GetInventoryByStatus")]
        public async Task<IActionResult> GetInventoryByStatus([Required][FromQuery] int statusId)
        {
            var inventory = await _inventoryItemService.GetInventoryByStatus(statusId);
            return Ok(inventory);
        }

        [HttpPost]
        public async Task<IActionResult> Post(InsertInventoryValidator insertInventoryValidator)
        {
            var status = await _statusService.GetStatusByID(1);
            var warehouse = await _locationService.GetLocationByID(1);
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
                    GTG = item.gtg,
                    LocationId = 1,
                    StatusId = 1
                };

                if (!await _inventoryItemService.Create(inventoryItem))
                {
                    return BadRequest("An error has occurred...");
                }
            }

            return Ok("Successful!");
        }

        [HttpPut]
        public async Task<IActionResult> Put(InsertInventoryValidator updateInventoryValidator)
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
        
        [HttpPut("UpdateInventoryStatus")]
        public async Task<IActionResult> UpdateInventoryStatus(UpdateInventoryStatusValidator updatedInfo)
        {
            try
            {
                await _inventoryItemService.UpdateStatus(updatedInfo.InventoryId, updatedInfo.StatusId);
            }
            catch (InventoryItemNotFoundException)
            {
                return BadRequest($"The inventory item with the ID {updatedInfo.InventoryId} does not exist.");
            }
            catch (StatusNotFoundException)
            {
                return BadRequest($"A status with the ID {updatedInfo.StatusId} does not exist.");
            }

            return Ok("Updated!");
        }

        [HttpPut("TransferInventory")]
        public async Task<IActionResult> TransferInventory(TransferInventoryValidator transferInfo)
        {
            try
            {
                if (!await _inventoryItemService.TransferItem(transferInfo.InventoryID, transferInfo.LocationID))
                {
                    return BadRequest("An error occurred while transferring");
                }
            }
            catch(LocationNotFoundException)
            {
                return BadRequest("A location with that ID does not exist.");
            }
            catch(InventoryItemNotFoundException)
            {
                return BadRequest("An inventory item with that ID does not exist.");
            }
            
            return Ok("Transfer successful!");
        }        
    }
}