using AutoMapper;
using MediatR;
using Sabemi.Application.Features.Contracts;
using Sabemi.Application.Features.Contracts.FindContracts;
using Sabemi.Domain.Entities;
using Sabemi.Domain.Interfaces.Repositories;
using System.Linq.Expressions;
namespace Sabemi.Application.Features.Webhooks.FindPayments;

internal class FindPaymentEventCommandHandler(
    IPaymentWebhookEventRepository repository,
    IMapper mapper
) : IRequestHandler<FindPaymentEventCommand, IEnumerable<PaymentWebhookEventModel>>
{
    public async Task<IEnumerable<PaymentWebhookEventModel>> Handle(FindPaymentEventCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<PaymentWebhookEvent> events = await repository.FindAsync(BuildFilter(request), cancellationToken);
        return mapper.Map<IEnumerable<PaymentWebhookEventModel>>(events);
    }

    private static Expression<Func<PaymentWebhookEvent, bool>> BuildFilter(FindPaymentEventCommand request)
    {
        return entity =>
        (request.ContractId == null || entity.ContractId == request.ContractId)
        &&
        (request.Status == null || entity.Status == request.Status);
    }
}
