using AutoMapper;
using Sabemi.Domain.Entities;
using Sabemi.Domain.Enums;
namespace Sabemi.Application.Features.Contracts;

public class ContractProfile : Profile
{
    public ContractProfile()
    {
        CreateMap<ContractPayment, ContractPaymentModel>();
        CreateMap<Contract, ContractModel>()
            .ForMember(_ => _.Status, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                dest.Status = src.Status?.Value ?? EContractStatus.Pending;
            });
    }
}
