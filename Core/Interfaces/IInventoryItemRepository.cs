using Core.Models;

namespace Core.Interfaces
{
    public interface IInventoryItemRepository
    {
        Task<bool> Create(InventoryItem inventoryItem);
        Task<List<InventoryItem>> GetAll();
        Task<InventoryItem> GetItemByID(int id);
        Task<List<InventoryItem>> GetItemBySerial(string serial);
        Task<List<InventoryItem>> GetItemByName(string name);
        Task<List<InventoryItem>> GetItemByDate(DateTime date);
        Task<bool> UpdateItem(InventoryItem inventoryItem);
        Task<bool> DeleteItem(int id);
        Task<List<DateTime>> GetDistinctDates();
        Task<List<InventoryItem>> GetInventoryByDate(DateTime date);
        Task<List<InventoryItem>> GetInventoryByLocation(int locationId);
        Task<bool> TransferItem(int inventoryId, int locationId);
    }
}