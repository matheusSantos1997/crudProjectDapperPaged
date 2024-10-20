using crudDapperWithEf.API.DTOs.Client;
using crudDapperWithEf.API.DTOs.Validations.Product;
using FluentValidation;

namespace crudDapperWithEf.API.DTOs.Validations.Client
{
    public class ClientDTOValidation : AbstractValidator<CreateNewClientDTO>
    {
        public ClientDTOValidation()
        {
            RuleFor(x => x.ClientName)
            .NotNull().WithMessage("Nome do cliente é obrigatório")
            .NotEmpty().WithMessage("Nome do cliente é obrigatório")
            .MaximumLength(50).WithMessage("Nome do cliente deve ter no máximo 50 caracteres");

        RuleFor(x => x.Email)
            .NotNull().WithMessage("Email é obrigatório")
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(50).WithMessage("Email deve ter no máximo 50 caracteres");

        RuleFor(x => x.Address)
            .NotNull().WithMessage("Endereço é obrigatório")
            .NotEmpty().WithMessage("Endereço é obrigatório")
            .MaximumLength(40).WithMessage("Endereço deve ter no máximo 40 caracteres");

        RuleFor(x => x.Products)
            .NotNull().WithMessage("Produtos sao obrigatórios")
            .NotEmpty().WithMessage("A lista de produtos não pode estar vazia")
            .Must(products => products?.Count > 0).WithMessage("A lista de produtos não pode estar vazia")
            .ForEach(produto => produto.SetValidator(new ProductDTOValidation())); // Valida cada item na lista
        }
    }
}