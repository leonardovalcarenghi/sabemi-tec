using Sabemi.Domain.Enums;
namespace Sabemi.Application.Features.Contracts;

public class ContractModel
{
    public Guid Id { get; set; }
    public EContractStatus Status { get; set; }
    public string Name { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal PendingAmount { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<ContractPaymentModel> Payments { get; set; } = [];
}
