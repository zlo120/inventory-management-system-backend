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

        public async Task<bool> Create(InventoryItem inventoryItem)
        {
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

        public async Task<List<InventoryItem>> GetItemBySerial(string serial)
        {
            return await _inventoryItemRepository.GetItemBySerial(serial);
        }

        public async Task<bool> UpdateItem(InventoryItem inventoryItem)
        {
            return await _inventoryItemRepository.UpdateItem(inventoryItem);
        }

        public async Task<List<DateTime>> GetDistinctDates()
        {
            return await _inventoryItemRepository.GetDistinctDates();
        }

        public async Task<List<InventoryItem>> GetInventoryByDate(DateTime date)
        {
            return await _inventoryItemRepository.GetInventoryByDate(date);
        }

        public async Task<List<InventoryItem>> GetAll()
        {
            return await _inventoryItemRepository.GetAll();
        }

        public async Task<List<InventoryItem>> GetItemByName(string name)
        {
            return await _inventoryItemRepository.GetItemByName(name);
        }

        public async Task<List<InventoryItem>> GetItemByDate(DateTime date)
        {
            return await _inventoryItemRepository.GetItemByDate(date);
        }
    }
}