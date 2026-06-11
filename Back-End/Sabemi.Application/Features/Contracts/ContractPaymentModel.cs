namespace Sabemi.Application.Features.Contracts;

public class ContractPaymentModel
{
    public Guid Id { get; init; }
    public Guid ContractId { get; set; }
    public Guid TransactionId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
