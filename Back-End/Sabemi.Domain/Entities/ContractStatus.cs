using Sabemi.Domain.Enums;
using Sabemi.Domain.Interfaces;

namespace Sabemi.Domain.Entities;

public class ContractStatus : IEntity
{
    public Guid Id { get; init; }
    public Guid ContractId { get; private set; }
    public EContractStatus Value { get; private set; }

    public virtual Contract? Contract { get; set; }

    public ContractStatus()
    {
        Id = Guid.NewGuid();
    }

    public static ContractStatus Create(Guid contractId, EContractStatus status)
    {
        return new ContractStatus
        {
            ContractId = contractId,
            Value = status
        };
    }

    public void SetStatus(EContractStatus newStatus)
    {
        Value = newStatus;
    }
}
