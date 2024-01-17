using Core.Models;

namespace Core.Interfaces
{
    public interface IInventoryItemRepository
    {
        Task<bool> Create(InventoryItem inventoryItem);
        Task<InventoryItem> GetItemByID(int id);
        Task<InventoryItem> GetItemBySerial(string serial);
        Task<bool> UpdateItem(InventoryItem inventoryItem);
        Task<bool> DeleteItem(int id);
    }
}
