using Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.CreateClienteResponsableLetra
{
    public class CreateClienteResponsableLetraCommandValidator : AbstractValidator<CreateClienteResponsableLetraCommand>
    {
        public CreateClienteResponsableLetraCommandValidator()
        {
            RuleFor(x => x.ClienteCode).NotEmpty().MaximumLength(5);
            Include(new ClienteResponsableLetraInputValidator<CreateClienteResponsableLetraCommand>());
        }
    }
}
