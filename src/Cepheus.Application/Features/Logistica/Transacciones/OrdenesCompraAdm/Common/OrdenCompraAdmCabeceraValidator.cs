// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion/Common/OrdenCompraAdministracionCabeceraValidator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Logistica.Enum;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common
{
    public abstract class OrdenCompraAdmCabeceraValidator<T> : AbstractValidator<T>
        where T : IOrdenCompraAdmCabecera
    {
        protected OrdenCompraAdmCabeceraValidator(IUnitOfWork uow)
        {
            RuleFor(x => x.ComprobantePagoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await uow.Comunes.ComprobantesPago.Query()
                        .AnyAsync(p => p.Code == c.Trim().ToUpper(), ct))
                .WithMessage("El comprobante de pago indicado no existe.");

            RuleFor(x => x.EmitirDocumento)
                .MaximumLength(30);

            RuleFor(x => x.ProveedorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await uow.Logistica.Maestros.Proveedores.Query()
                        .AnyAsync(p => p.Code == c.Trim().ToUpper(), ct))
                .WithMessage("El proveedor indicado no existe.");

            RuleFor(x => x.CompradorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await uow.Logistica.Catalogos.Compradores.Query()
                        .AnyAsync(p => p.Code == c.Trim().ToUpper(), ct))
                .WithMessage("El comprador indicado no existe.");

            RuleFor(x => x.MonedaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await uow.Comunes.Monedas.Query()
                        .AnyAsync(m => m.Code == c.Trim().ToUpper(), ct))
                .WithMessage("La moneda indicada no existe.");

            RuleFor(x => x.LugarEnvioCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await uow.Logistica.Catalogos.LugaresEnvio.Query()
                        .AnyAsync(l => l.Code == c.Trim().ToUpper(), ct))
                .WithMessage("El lugar de envío indicado no existe.");

            RuleFor(x => x.FormaPagoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await uow.Logistica.Catalogos.FormasPago.Query()
                        .AnyAsync(f => f.Code == c.Trim().ToUpper(), ct))
                .WithMessage("La forma de pago indicada no existe.");

            RuleFor(x => x.TramiteCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await uow.Logistica.Catalogos.Tramites.Query()
                        .AnyAsync(t => t.Code == c.Trim().ToUpper(), ct))
                .WithMessage("El trámite indicado no existe.");

            RuleFor(x => x.NotaCompraCode)
                .MustAsync(async (c, ct) =>
                    await uow.Logistica.Catalogos.NotasCompra.Query()
                        .AnyAsync(n => n.Code == c!.Trim().ToUpper(), ct))
                .WithMessage("La nota indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.NotaCompraCode));

            RuleFor(x => x.UnidadNegocioCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await uow.Logistica.Catalogos.UnidadesNegocio.Query()
                        .AnyAsync(u => u.Code == c.Trim().ToUpper(), ct))
                .WithMessage("La unidad de negocio indicada no existe.");

            RuleFor(x => x.Motivo)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(m => System.Enum.TryParse<MotivoOrdenCompraAdministracion>(m, true, out var v)
                           && System.Enum.IsDefined(v))
                .WithMessage("El motivo indicado no es válido.");

            RuleFor(x => x.NoGravableCompra).GreaterThanOrEqualTo(0);
            RuleFor(x => x.ServicioCompra).GreaterThanOrEqualTo(0);
            RuleFor(x => x.IgvExteriorCompra).GreaterThanOrEqualTo(0);
        }
    }
}
