using AutoMapper;
using Sabemi.Domain.Entities;
namespace Sabemi.Application.Features.Webhooks;

public class PaymentWebhookEventProfile : Profile
{
    public PaymentWebhookEventProfile()
    {
        CreateMap<PaymentWebhookEvent, PaymentWebhookEventModel>();
    }
}
