// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/UpdateImportacionGasto/UpdateImportacionGastoCommandValidator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.UpdateImportacionGasto
{
    public class UpdateImportacionGastoCommandValidator : AbstractValidator<UpdateImportacionGastoCommand>
    {
        public UpdateImportacionGastoCommandValidator(IUnitOfWork uow)
        {
            RuleFor(x => x.PlantaCode).NotEmpty();
            RuleFor(x => x.ImportacionCode).NotEmpty();
            RuleFor(x => x.ProveedorCode).NotEmpty();
            RuleFor(x => x.NumeroDocumento).NotEmpty().MaximumLength(15);

            RuleFor(x => x.ComprobantePagoCode).Cascade(CascadeMode.Stop)
                .MustAsync(async (code, ct) => await uow.Comunes.ComprobantesPago.Query().AnyAsync(c => c.Code == code.Trim().ToUpper(), ct))
                .WithMessage("El comprobante de pago indicado no existe.");

            RuleFor(x => x.MonedaCode).Cascade(CascadeMode.Stop).NotEmpty()
                .Must(c => c.Trim().ToUpper() is "PEN" or "USD")
                .WithMessage("La moneda del gasto debe ser PEN o USD.")
                .MustAsync(async (c, ct) => await uow.Comunes.Monedas.Query().AnyAsync(m => m.Code == c.Trim().ToUpper(), ct))
                .WithMessage("La moneda indicada no existe.");

            RuleFor(x => x.FechaEmision).NotEmpty();
            RuleFor(x => x.NetoGasto).GreaterThanOrEqualTo(0);
            RuleFor(x => x.NetoGastoInafecto).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Igv).GreaterThanOrEqualTo(0).When(x => x.Igv.HasValue);
            RuleFor(x => x.RowVersion).NotEmpty();
        }
    }
}
