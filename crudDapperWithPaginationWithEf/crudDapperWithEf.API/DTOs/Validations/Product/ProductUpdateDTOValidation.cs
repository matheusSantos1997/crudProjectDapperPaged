using crudDapperWithEf.API.DTOs.Product;
using FluentValidation;

namespace crudDapperWithEf.API.DTOs.Validations.Product
{
    public class ProductUpdateDTOValidation : AbstractValidator<UpdateProductDTO>
    {
        public ProductUpdateDTOValidation()
        {
            RuleFor(x => x.ProductName).NotNull().WithMessage("Nome do produto é obrigatório")
                                       .NotEmpty().WithMessage("Nome do produto é obrigatório")
                                       .MaximumLength(50).WithMessage("Nome do produto deve ter no minimo 50 caracteres");

            RuleFor(x => x.ProductType).NotNull().WithMessage("Tipo do produto é obrigatório")
                                       .NotEmpty().WithMessage("Tipo do produto é obrigatório")
                                       .MaximumLength(50).WithMessage("Nome do produto deve ter no minimo 50 caracteres");

            RuleFor(x => x.Price).NotNull().WithMessage("Preco do produto e obrigatorio")
                                 .NotEmpty().WithMessage("Preco do produto e obrigatorio");
        }
    }
}