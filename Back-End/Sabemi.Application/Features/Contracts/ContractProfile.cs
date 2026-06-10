using AutoMapper;
using Sabemi.Domain.Entities;
namespace Sabemi.Application.Features.Contracts;

public class ContractProfile : Profile
{
    public ContractProfile()
    {
        CreateMap<Contract, ContractModel>();
    }
}
