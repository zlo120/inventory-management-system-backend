﻿using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class InventoryItemRepository : IInventoryItemRepository
    {
        private readonly Context _context;
        private readonly ILogger<InventoryItemRepository> _logger;
        public InventoryItemRepository(Context context, ILogger<InventoryItemRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Create(InventoryItem inventoryItem)
        {
            _context.InventoryItems.Add(inventoryItem);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Critical error occured when saving changes: {ex}", DateTime.UtcNow.ToLongTimeString());
                return false;
            }
        }

        public async Task<InventoryItem> GetItemByID(int id)
        {
            return await _context.InventoryItems.Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<InventoryItem> GetItemBySerial(string serial)
        {
            return await _context.InventoryItems.Where(u => u.Serial == serial).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateItem(InventoryItem inventoryItem)
        {
            var originalItem = await _context.InventoryItems.Where(i => i.Id == inventoryItem.Id).FirstOrDefaultAsync();
            if (originalItem is null)
            {
                return false;
            }

            originalItem.Name = inventoryItem.Name;
            originalItem.Supplier = inventoryItem.Supplier;
            originalItem.Date = inventoryItem.Date;
            originalItem.Quantity = inventoryItem.Quantity;
            originalItem.Notes = inventoryItem.Notes;

            if (inventoryItem.ALVLP is not null) originalItem.ALVLP = inventoryItem.ALVLP;
            if (inventoryItem.UL is not null) originalItem.UL = inventoryItem.UL;
            if (inventoryItem.MDM is not null) originalItem.MDM = inventoryItem.MDM;
            if (inventoryItem.Reset is not null) originalItem.Reset = inventoryItem.Reset;
            if (inventoryItem.GTG is not null) originalItem.GTG = inventoryItem.GTG;

            _context.Update(originalItem);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Critical error occured when saving changes: {ex}", DateTime.UtcNow.ToLongTimeString());
                return false;
            }
        }

        public async Task<bool> DeleteItem(int id)
        {
            var originalItem = await _context.InventoryItems.Where(i => i.Id == id).FirstOrDefaultAsync();

            if (originalItem is null)
            {
                return false;
            }

            _context.InventoryItems.Remove(originalItem);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Critical error occured when saving changes: {ex}", DateTime.UtcNow.ToLongTimeString());
                return false;
            }
        }
    }
}