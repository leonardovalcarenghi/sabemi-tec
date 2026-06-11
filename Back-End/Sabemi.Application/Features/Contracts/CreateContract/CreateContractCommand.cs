using MediatR;
using System.ComponentModel.DataAnnotations;
namespace Sabemi.Application.Features.Contracts.CreateContract;

public class CreateContractCommand : IRequest<Guid>
{
    [Required(ErrorMessage = "O nome do contrato é obrigatório.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "O valor total do contrato é obrigatório.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor total do contrato deve ser maior que zero.")]
    public decimal TotalAmount { get; set; }
}
