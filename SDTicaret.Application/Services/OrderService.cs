using AutoMapper;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Core.Interfaces;
using System.Linq.Expressions;

namespace SDTicaret.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync()
    {
        var orders = await _unitOfWork.Repository<Order>().GetAllAsync();
        var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);
        
        foreach (var orderDto in orderDtos)
        {
            await CalculateOrderDetails(orderDto);
        }
        
        return orderDtos;
    }

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(id);
        if (order == null) return null;
        
        var orderDto = _mapper.Map<OrderDto>(order);
        await CalculateOrderDetails(orderDto);
        return orderDto;
    }

    public async Task<OrderDto> AddAsync(OrderDto dto)
    {
        var order = _mapper.Map<Order>(dto);
        order.OrderNumber = GenerateOrderNumber();
        order.OrderDate = DateTime.UtcNow;
        order.OrderStatus = "Pending";
        
        await _unitOfWork.Repository<Order>().AddAsync(order);
        await _unitOfWork.SaveChangesAsync();
        
        var result = _mapper.Map<OrderDto>(order);
        await CalculateOrderDetails(result);
        return result;
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
    {
        var order = new Order
        {
            CustomerId = dto.CustomerId,
            OrderNumber = GenerateOrderNumber(),
            OrderDate = DateTime.UtcNow,
            OrderStatus = "Pending",
            ShippingAddress = dto.ShippingAddress,
            BillingAddress = dto.BillingAddress,
            Notes = dto.Notes,
            ShippingMethod = dto.ShippingMethod,
            ShippingCost = dto.ShippingCost,
            TaxAmount = dto.TaxAmount,
            DiscountAmount = dto.DiscountAmount
        };

        // Toplam tutarı hesapla
        decimal subtotal = dto.OrderItems.Sum(item => item.Quantity * item.UnitPrice);
        order.TotalAmount = subtotal + dto.ShippingCost + dto.TaxAmount - dto.DiscountAmount;

        await _unitOfWork.Repository<Order>().AddAsync(order);
        await _unitOfWork.SaveChangesAsync();

        // Sipariş kalemlerini ekle
        foreach (var itemDto in dto.OrderItems)
        {
            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice
            };
            await _unitOfWork.Repository<OrderItem>().AddAsync(orderItem);
        }

        // Durum geçmişini ekle
        var statusHistory = new OrderStatusHistory
        {
            OrderId = order.Id,
            PreviousStatus = "",
            NewStatus = "Pending",
            Notes = "Sipariş oluşturuldu",
            StatusChangeDate = DateTime.UtcNow
        };
        await _unitOfWork.Repository<OrderStatusHistory>().AddAsync(statusHistory);

        await _unitOfWork.SaveChangesAsync();

        var result = _mapper.Map<OrderDto>(order);
        await CalculateOrderDetails(result);
        return result;
    }

    public async Task<OrderDto> UpdateAsync(OrderDto dto)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(dto.Id);
        if (order == null)
            throw new InvalidOperationException("Sipariş bulunamadı");

        _mapper.Map(dto, order);
        _unitOfWork.Repository<Order>().Update(order);
        await _unitOfWork.SaveChangesAsync();
        
        var result = _mapper.Map<OrderDto>(order);
        await CalculateOrderDetails(result);
        return result;
    }

    public async Task<OrderDto> UpdateOrderStatusAsync(UpdateOrderStatusDto dto)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(dto.OrderId);
        if (order == null)
            throw new InvalidOperationException("Sipariş bulunamadı");

        var previousStatus = order.OrderStatus;
        order.OrderStatus = dto.NewStatus;

        // Durum değişikliği tarihlerini güncelle
        switch (dto.NewStatus)
        {
            case "Confirmed":
                order.ConfirmedDate = DateTime.UtcNow;
                break;
            case "Processing":
                order.ProcessingDate = DateTime.UtcNow;
                break;
            case "Shipped":
                order.ShippedDate = DateTime.UtcNow;
                order.TrackingNumber = dto.TrackingNumber;
                break;
            case "Delivered":
                order.DeliveredDate = DateTime.UtcNow;
                break;
            case "Cancelled":
                order.CancelledDate = DateTime.UtcNow;
                order.CancellationReason = dto.CancellationReason;
                break;
        }

        // Durum geçmişini ekle
        var statusHistory = new OrderStatusHistory
        {
            OrderId = order.Id,
            PreviousStatus = previousStatus,
            NewStatus = dto.NewStatus,
            Notes = dto.Notes,
            ChangedByUserId = dto.ChangedByUserId,
            ChangedByUserName = dto.ChangedByUserName,
            StatusChangeDate = DateTime.UtcNow
        };

        await _unitOfWork.Repository<OrderStatusHistory>().AddAsync(statusHistory);
        _unitOfWork.Repository<Order>().Update(order);
        await _unitOfWork.SaveChangesAsync();

        var result = _mapper.Map<OrderDto>(order);
        await CalculateOrderDetails(result);
        return result;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(id);
        if (order == null) return false;
        
        _unitOfWork.Repository<Order>().Delete(order);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelOrderAsync(int orderId, string reason, int? userId = null, string? userName = null)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);
        if (order == null) return false;

        if (order.OrderStatus == "Delivered" || order.OrderStatus == "Cancelled")
            throw new InvalidOperationException("Bu sipariş iptal edilemez");

        var updateDto = new UpdateOrderStatusDto
        {
            OrderId = orderId,
            NewStatus = "Cancelled",
            CancellationReason = reason,
            ChangedByUserId = userId,
            ChangedByUserName = userName
        };

        await UpdateOrderStatusAsync(updateDto);
        return true;
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status)
    {
        var orders = await _unitOfWork.Repository<Order>().GetAllAsync(o => o.OrderStatus == status);
        var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);
        
        foreach (var orderDto in orderDtos)
        {
            await CalculateOrderDetails(orderDto);
        }
        
        return orderDtos;
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(int customerId)
    {
        var orders = await _unitOfWork.Repository<Order>().GetAllAsync(o => o.CustomerId == customerId);
        var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);
        
        foreach (var orderDto in orderDtos)
        {
            await CalculateOrderDetails(orderDto);
        }
        
        return orderDtos;
    }

    public async Task<IEnumerable<OrderStatusHistoryDto>> GetOrderStatusHistoryAsync(int orderId)
    {
        var history = await _unitOfWork.Repository<OrderStatusHistory>().GetAllAsync(h => h.OrderId == orderId);
        return _mapper.Map<IEnumerable<OrderStatusHistoryDto>>(history);
    }

    public async Task<IEnumerable<OrderDto>> GetPendingOrdersAsync()
    {
        return await GetOrdersByStatusAsync("Pending");
    }

    public async Task<IEnumerable<OrderDto>> GetProcessingOrdersAsync()
    {
        return await GetOrdersByStatusAsync("Processing");
    }

    public async Task<IEnumerable<OrderDto>> GetShippedOrdersAsync()
    {
        return await GetOrdersByStatusAsync("Shipped");
    }

    private async Task CalculateOrderDetails(OrderDto orderDto)
    {
        if (orderDto == null) return;
        
        // Sipariş kalemlerini getir
        var orderItems = await _unitOfWork.Repository<OrderItem>().GetAllAsync(oi => oi.OrderId == orderDto.Id);
        orderDto.OrderItems = _mapper.Map<ICollection<OrderItemDto>>(orderItems);
        orderDto.ItemCount = orderItems.Count();

        // Durum geçmişini getir
        var statusHistory = await _unitOfWork.Repository<OrderStatusHistory>().GetAllAsync(sh => sh.OrderId == orderDto.Id);
        orderDto.StatusHistory = _mapper.Map<ICollection<OrderStatusHistoryDto>>(statusHistory);

        // Final tutarı hesapla
        orderDto.FinalAmount = orderDto.TotalAmount + orderDto.ShippingCost + orderDto.TaxAmount - orderDto.DiscountAmount;
    }

    private string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }
} 
