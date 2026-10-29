// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/UpdateImportacionDetalle/UpdateImportacionDetalleCommandValidator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.UpdateImportacionDetalle
{
    public class UpdateImportacionDetalleCommandValidator : AbstractValidator<UpdateImportacionDetalleCommand>
    {
        public UpdateImportacionDetalleCommandValidator(IUnitOfWork uow)
        {
            RuleFor(x => x.PlantaCode).NotEmpty();
            RuleFor(x => x.ImportacionCode).NotEmpty();
            RuleFor(x => x.ProveedorCode).NotEmpty();
            RuleFor(x => x.ArticuloCode).NotEmpty();

            RuleFor(x => x.ComprobantePagoCode).Cascade(CascadeMode.Stop)
                .MustAsync(async (code, ct) => await uow.Comunes.ComprobantesPago.Query().AnyAsync(c => c.Code == code.Trim().ToUpper(), ct))
                .WithMessage("El comprobante de pago indicado no existe.");

            RuleFor(x => x.NumeroDocumento).NotEmpty().MaximumLength(15);
            RuleFor(x => x.FechaEmision).NotEmpty();
            RuleFor(x => x.Cantidad).GreaterThan(0);
            RuleFor(x => x.ValorFob).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Flete).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Seguro).GreaterThanOrEqualTo(0);
            RuleFor(x => x.RowVersion).NotEmpty();
        }
    }
}
