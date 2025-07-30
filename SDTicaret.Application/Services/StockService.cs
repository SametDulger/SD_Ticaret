using AutoMapper;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Core.Interfaces;
using System.Linq.Expressions;

namespace SDTicaret.Application.Services;

public class StockService : IStockService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public StockService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<StockDto>> GetAllAsync()
    {
        var stocks = await _unitOfWork.Repository<Stock>().GetAllAsync();
        var stockDtos = _mapper.Map<IEnumerable<StockDto>>(stocks);
        
        // Stok durumlarını hesapla
        foreach (var stockDto in stockDtos)
        {
            CalculateStockStatus(stockDto);
        }
        
        return stockDtos;
    }

    public async Task<StockDto?> GetByIdAsync(int id)
    {
        var stock = await _unitOfWork.Repository<Stock>().GetByIdAsync(id);
        if (stock == null) return null;
        
        var stockDto = _mapper.Map<StockDto>(stock);
        CalculateStockStatus(stockDto);
        return stockDto;
    }

    public async Task<StockDto> AddAsync(StockDto dto)
    {
        var stock = _mapper.Map<Stock>(dto);
        await _unitOfWork.Repository<Stock>().AddAsync(stock);
        await _unitOfWork.SaveChangesAsync();
        
        var result = _mapper.Map<StockDto>(stock);
        CalculateStockStatus(result);
        return result;
    }

    public async Task<StockDto> UpdateAsync(StockDto dto)
    {
        var stock = await _unitOfWork.Repository<Stock>().GetByIdAsync(dto.Id);
        if (stock == null)
            throw new InvalidOperationException("Stok bulunamadı");

        _mapper.Map(dto, stock);
        _unitOfWork.Repository<Stock>().Update(stock);
        await _unitOfWork.SaveChangesAsync();
        
        var result = _mapper.Map<StockDto>(stock);
        CalculateStockStatus(result);
        return result;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var stock = await _unitOfWork.Repository<Stock>().GetByIdAsync(id);
        if (stock == null) return false;
        
        _unitOfWork.Repository<Stock>().Delete(stock);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<StockDto> StockInAsync(StockInDto dto)
    {
        var stock = await _unitOfWork.Repository<Stock>().GetByIdAsync(dto.StockId);
        if (stock == null)
            throw new InvalidOperationException("Stok bulunamadı");

        var previousQuantity = stock.Quantity;
        stock.Quantity += dto.Quantity;
        stock.LastStockInDate = DateTime.UtcNow;
        stock.TotalStockIn += dto.Quantity;

        // Stok hareketi kaydet
        var movement = new StockMovement
        {
            StockId = stock.Id,
            ProductId = stock.ProductId,
            BranchId = stock.BranchId,
            MovementType = "IN",
            Quantity = dto.Quantity,
            PreviousQuantity = previousQuantity,
            NewQuantity = stock.Quantity,
            Reason = dto.Reason,
            ReferenceNumber = dto.ReferenceNumber,
            UserId = dto.UserId,
            UserName = dto.UserName
        };

        await _unitOfWork.Repository<StockMovement>().AddAsync(movement);
        _unitOfWork.Repository<Stock>().Update(stock);
        await _unitOfWork.SaveChangesAsync();

        // Bildirim kontrolü
        await CheckAndCreateNotifications(stock);

        var result = _mapper.Map<StockDto>(stock);
        CalculateStockStatus(result);
        return result;
    }

    public async Task<StockDto> StockOutAsync(StockOutDto dto)
    {
        var stock = await _unitOfWork.Repository<Stock>().GetByIdAsync(dto.StockId);
        if (stock == null)
            throw new InvalidOperationException("Stok bulunamadı");

        if (stock.Quantity < dto.Quantity)
            throw new InvalidOperationException("Yetersiz stok");

        var previousQuantity = stock.Quantity;
        stock.Quantity -= dto.Quantity;
        stock.LastStockOutDate = DateTime.UtcNow;
        stock.TotalStockOut += dto.Quantity;

        // Stok hareketi kaydet
        var movement = new StockMovement
        {
            StockId = stock.Id,
            ProductId = stock.ProductId,
            BranchId = stock.BranchId,
            MovementType = "OUT",
            Quantity = dto.Quantity,
            PreviousQuantity = previousQuantity,
            NewQuantity = stock.Quantity,
            Reason = dto.Reason,
            ReferenceNumber = dto.ReferenceNumber,
            UserId = dto.UserId,
            UserName = dto.UserName
        };

        await _unitOfWork.Repository<StockMovement>().AddAsync(movement);
        _unitOfWork.Repository<Stock>().Update(stock);
        await _unitOfWork.SaveChangesAsync();

        // Bildirim kontrolü
        await CheckAndCreateNotifications(stock);

        var result = _mapper.Map<StockDto>(stock);
        CalculateStockStatus(result);
        return result;
    }

    public async Task<IEnumerable<StockDto>> GetLowStockItemsAsync()
    {
        var stocks = await _unitOfWork.Repository<Stock>().GetAllAsync(s => s.Quantity <= s.MinimumStock && s.IsLowStockAlertEnabled);
        var stockDtos = _mapper.Map<IEnumerable<StockDto>>(stocks);
        
        foreach (var stockDto in stockDtos)
        {
            CalculateStockStatus(stockDto);
        }
        
        return stockDtos;
    }

    public async Task<IEnumerable<StockDto>> GetOutOfStockItemsAsync()
    {
        var stocks = await _unitOfWork.Repository<Stock>().GetAllAsync(s => s.Quantity == 0);
        var stockDtos = _mapper.Map<IEnumerable<StockDto>>(stocks);
        
        foreach (var stockDto in stockDtos)
        {
            CalculateStockStatus(stockDto);
        }
        
        return stockDtos;
    }

    public async Task<IEnumerable<StockMovementDto>> GetStockMovementsAsync(int stockId)
    {
        var movements = await _unitOfWork.Repository<StockMovement>().GetAllAsync(m => m.StockId == stockId);
        return _mapper.Map<IEnumerable<StockMovementDto>>(movements);
    }

    public async Task<IEnumerable<StockNotificationDto>> GetUnreadNotificationsAsync()
    {
        var notifications = await _unitOfWork.Repository<StockNotification>().GetAllAsync(n => !n.IsRead);
        return _mapper.Map<IEnumerable<StockNotificationDto>>(notifications);
    }

    public async Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId, string userName)
    {
        var notification = await _unitOfWork.Repository<StockNotification>().GetByIdAsync(notificationId);
        if (notification == null) return false;

        notification.IsRead = true;
        notification.ReadDate = DateTime.UtcNow;
        notification.ReadByUserId = userId;
        notification.ReadByUserName = userName;

        _unitOfWork.Repository<StockNotification>().Update(notification);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private void CalculateStockStatus(StockDto stockDto)
    {
        stockDto.IsLowStock = stockDto.Quantity <= stockDto.MinimumStock && stockDto.Quantity > 0;
        stockDto.IsOutOfStock = stockDto.Quantity == 0;
        stockDto.IsOverstock = stockDto.Quantity > stockDto.MaximumStock;

        if (stockDto.IsOutOfStock)
            stockDto.StockStatus = "OUT";
        else if (stockDto.IsLowStock)
            stockDto.StockStatus = "LOW";
        else if (stockDto.IsOverstock)
            stockDto.StockStatus = "OVERSTOCK";
        else
            stockDto.StockStatus = "NORMAL";
    }

    private async Task CheckAndCreateNotifications(Stock stock)
    {
        if (!stock.IsLowStockAlertEnabled) return;

        var existingNotification = await _unitOfWork.Repository<StockNotification>()
            .GetAllAsync(n => n.StockId == stock.Id && !n.IsRead);

        // Mevcut bildirimi sil
        foreach (var notification in existingNotification)
        {
            _unitOfWork.Repository<StockNotification>().Delete(notification);
        }

        // Yeni bildirim oluştur
        if (stock.Quantity == 0)
        {
            var notification = new StockNotification
            {
                StockId = stock.Id,
                ProductId = stock.ProductId,
                BranchId = stock.BranchId,
                NotificationType = "OUT_OF_STOCK",
                Message = $"Ürün stokta tükendi. Ürün ID: {stock.ProductId}",
                CurrentQuantity = stock.Quantity,
                ThresholdQuantity = stock.MinimumStock
            };
            await _unitOfWork.Repository<StockNotification>().AddAsync(notification);
        }
        else if (stock.Quantity <= stock.MinimumStock)
        {
            var notification = new StockNotification
            {
                StockId = stock.Id,
                ProductId = stock.ProductId,
                BranchId = stock.BranchId,
                NotificationType = "LOW_STOCK",
                Message = $"Ürün stoku kritik seviyede. Mevcut: {stock.Quantity}, Minimum: {stock.MinimumStock}",
                CurrentQuantity = stock.Quantity,
                ThresholdQuantity = stock.MinimumStock
            };
            await _unitOfWork.Repository<StockNotification>().AddAsync(notification);
        }
        else if (stock.Quantity > stock.MaximumStock)
        {
            var notification = new StockNotification
            {
                StockId = stock.Id,
                ProductId = stock.ProductId,
                BranchId = stock.BranchId,
                NotificationType = "OVERSTOCK",
                Message = $"Ürün stoku maksimum seviyeyi aştı. Mevcut: {stock.Quantity}, Maksimum: {stock.MaximumStock}",
                CurrentQuantity = stock.Quantity,
                ThresholdQuantity = stock.MaximumStock
            };
            await _unitOfWork.Repository<StockNotification>().AddAsync(notification);
        }
    }
} 
