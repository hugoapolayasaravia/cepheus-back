using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.CreateArticuloAnexo
{
    public class CreateArticuloAnexoCommandValidator : ArticuloAnexoValidator<CreateArticuloAnexoCommand>
    {
        public CreateArticuloAnexoCommandValidator(IUnitOfWork uow) : base(uow)
        {
            RuleFor(x => x)
                .MustAsync(async (x, ct) =>
                {
                    var scc = x.SubCentroCostoCode.Trim().ToUpper();
                    var art = x.ArticuloCode.Trim().ToUpper();
                    return !await uow.Logistica.Maestros.ArticuloAnexos.Query()
                        .AnyAsync(a => a.SubCentroCostoCode == scc && a.ArticuloCode == art, ct);
                })
                .WithMessage("Ya existe el enlace de anexo para ese subcentro y artículo.")
                .OverridePropertyName("ArticuloCode")
                .When(x => !string.IsNullOrWhiteSpace(x.SubCentroCostoCode) && !string.IsNullOrWhiteSpace(x.ArticuloCode));
        }
    }
}
