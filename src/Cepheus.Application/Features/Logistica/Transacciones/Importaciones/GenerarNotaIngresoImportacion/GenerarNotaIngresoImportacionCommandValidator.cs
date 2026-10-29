// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/GenerarNotaIngresoImportacion/GenerarNotaIngresoImportacionCommandValidator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.GenerarNotaIngresoImportacion
{
    public class GenerarNotaIngresoImportacionCommandValidator : AbstractValidator<GenerarNotaIngresoImportacionCommand>
    {
        public GenerarNotaIngresoImportacionCommandValidator(IUnitOfWork uow)
        {
            RuleFor(x => x.PlantaCode).NotEmpty();
            RuleFor(x => x.ImportacionCode).NotEmpty();

            RuleFor(x => x.ProveedorCode).Cascade(CascadeMode.Stop).NotEmpty()
                .MustAsync(async (c, ct) => await uow.Logistica.Maestros.Proveedores.Query().AnyAsync(p => p.Code == c.Trim().ToUpper(), ct))
                .WithMessage("El proveedor indicado no existe.");

            RuleFor(x => x.ComprobantePagoCode).Cascade(CascadeMode.Stop).NotEmpty()
                .MustAsync(async (c, ct) => await uow.Comunes.ComprobantesPago.Query().AnyAsync(p => p.Code == c.Trim().ToUpper(), ct))
                .WithMessage("El comprobante de pago indicado no existe.");

            RuleFor(x => x.FormaPagoCode).Cascade(CascadeMode.Stop).NotEmpty()
                .MustAsync(async (c, ct) => await uow.Logistica.Catalogos.FormasPago.Query().AnyAsync(f => f.Code == c.Trim().ToUpper(), ct))
                .WithMessage("La forma de pago indicada no existe.");

            RuleFor(x => x.FechaEmision).NotEmpty();
            RuleFor(x => x.FechaRecepcion).NotEmpty();
            RuleFor(x => x.NumeroDocumento).MaximumLength(15);
            RuleFor(x => x.NumeroGuia).MaximumLength(15);
            RuleFor(x => x.NumeroReferencia).MaximumLength(15);

            RuleFor(x => x.ArticuloCodes)
                .Must(l => l is null || l.Count > 0)
                .WithMessage("Si envía ArticuloCodes debe indicar al menos un artículo.");
        }
    }
}
