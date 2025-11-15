using ECommerceApi.Dtos.Order;

namespace ECommerceApi.Services.Interfaces
{
    public interface IOrderService
    {
        Task<CheckoutResponse> CheckoutAsync(CheckoutRequest request, Guid userId);
        Task<List<OrderHistoryResponse>> GetOrderHistoriesAsync(Guid userId);
    }
}