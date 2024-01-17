using Core.DataValidators;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services
{
    public class InventoryItemService : IInventoryItemService
    {
        private readonly IInventoryItemRepository _inventoryItemRepository;
        public InventoryItemService(IInventoryItemRepository inventoryItemRepository)
        {
            _inventoryItemRepository = inventoryItemRepository;
        }

        public async Task<bool> Create(CreateInventoryItemValidator inventoryItemInfo)
        {
            var inventoryItem = new InventoryItem()
            {
                Serial = inventoryItemInfo.Serial,
                Name = inventoryItemInfo.Name,
                Supplier = inventoryItemInfo.Supplier,
                Date = inventoryItemInfo.Date,
                Quantity = inventoryItemInfo.Quantity,
                Notes = inventoryItemInfo.Notes,
                ALVLP = inventoryItemInfo.ALVLP,
                UL = inventoryItemInfo.UL,
                MDM = inventoryItemInfo.MDM,
                Reset = inventoryItemInfo.Reset,
                GTG = inventoryItemInfo.GTG
            };

            return await _inventoryItemRepository.Create(inventoryItem);
        }

        public async Task<bool> DeleteItem(int id)
        {
            return await _inventoryItemRepository.DeleteItem(id);
        }

        public async Task<InventoryItem> GetItemByID(int id)
        {
            return await _inventoryItemRepository.GetItemByID(id);
        }

        public async Task<InventoryItem> GetItemBySerial(string serial)
        {
            return await _inventoryItemRepository.GetItemBySerial(serial);
        }

        public async Task<bool> UpdateItem(InventoryItem inventoryItem)
        {
            return await _inventoryItemRepository.UpdateItem(inventoryItem);
        }
    }
}