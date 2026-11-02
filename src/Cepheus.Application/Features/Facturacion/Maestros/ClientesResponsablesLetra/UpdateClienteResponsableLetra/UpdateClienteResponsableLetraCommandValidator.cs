using Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.UpdateClienteResponsableLetra
{
    public class UpdateClienteResponsableLetraCommandValidator : AbstractValidator<UpdateClienteResponsableLetraCommand>
    {
        public UpdateClienteResponsableLetraCommandValidator()
        {
            RuleFor(x => x.ClienteCode).NotEmpty().MaximumLength(5);
            RuleFor(x => x.Sequence).NotEmpty().MaximumLength(10);
            Include(new ClienteResponsableLetraInputValidator<UpdateClienteResponsableLetraCommand>());
        }
    }
}
