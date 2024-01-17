using Core.DataValidators;
using Core.Models;

namespace Core.Interfaces
{
    public interface IInventoryItemService
    {
        Task<bool> Create(CreateInventoryItemValidator inventoryItemInfo);
        Task<InventoryItem> GetItemByID(int id);
        Task<InventoryItem> GetItemBySerial(string serial);
        Task<bool> UpdateItem(InventoryItem inventoryItem);
        Task<bool> DeleteItem(int id);
    }
}
