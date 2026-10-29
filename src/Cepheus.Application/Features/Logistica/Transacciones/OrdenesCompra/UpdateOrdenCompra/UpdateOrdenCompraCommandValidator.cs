using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.CreateOrdenCompra;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.UpdateOrdenCompra
{
    public class UpdateOrdenCompraCommandValidator : AbstractValidator<UpdateOrdenCompraCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateOrdenCompraCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode).Cascade(CascadeMode.Stop).NotEmpty()
                .MustAsync(async (c, ct) => await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == c.Trim().ToUpper(), ct))
                .WithMessage("La planta indicada no existe.");

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El código de la Orden de Compra es obligatorio.");

            RuleFor(x => x.TipoCompraCode).Cascade(CascadeMode.Stop).NotEmpty()
                .MustAsync(async (c, ct) => await _uow.Logistica.Catalogos.TiposCompra.Query().AnyAsync(t => t.Code == c.Trim().ToUpper(), ct))
                .WithMessage("El tipo de compra indicado no existe.");

            RuleFor(x => x.ComprobantePagoCode)
                .MustAsync(async (code, ct) =>
                    await _uow.Comunes.ComprobantesPago
                        .Query()
                        .AnyAsync(c => c.Code == code, ct))
                .WithMessage("El comprobante de pago indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.ComprobantePagoCode));

            RuleFor(x => x.ProveedorCode).Cascade(CascadeMode.Stop).NotEmpty()
                .MustAsync(async (c, ct) => await _uow.Logistica.Maestros.Proveedores.Query().AnyAsync(p => p.Code == c.Trim().ToUpper(), ct))
                .WithMessage("El proveedor indicado no existe.");

            RuleFor(x => x.CompradorCode).Cascade(CascadeMode.Stop).NotEmpty()
                .MustAsync(async (c, ct) => await _uow.Logistica.Catalogos.Compradores.Query().AnyAsync(p => p.Code == c.Trim().ToUpper(), ct))
                .WithMessage("El comprador indicado no existe.");

            RuleFor(x => x.MonedaCode).Cascade(CascadeMode.Stop).NotEmpty()
                .MustAsync(async (c, ct) => await _uow.Comunes.Monedas.Query().AnyAsync(m => m.Code == c.Trim().ToUpper(), ct))
                .WithMessage("La moneda indicada no existe.");

            RuleFor(x => x.LugarEnvioCode).Cascade(CascadeMode.Stop).NotEmpty()
                .MustAsync(async (c, ct) => await _uow.Logistica.Catalogos.LugaresEnvio.Query().AnyAsync(l => l.Code == c.Trim().ToUpper(), ct))
                .WithMessage("El lugar de envío indicado no existe.");

            RuleFor(x => x.FormaPagoCode).Cascade(CascadeMode.Stop).NotEmpty()
                .MustAsync(async (c, ct) => await _uow.Logistica.Catalogos.FormasPago.Query().AnyAsync(f => f.Code == c.Trim().ToUpper(), ct))
                .WithMessage("La forma de pago indicada no existe.");

            RuleFor(x => x.TramiteCode).Cascade(CascadeMode.Stop).NotEmpty()
                .MustAsync(async (c, ct) => await _uow.Logistica.Catalogos.Tramites.Query().AnyAsync(t => t.Code == c.Trim().ToUpper(), ct))
                .WithMessage("El trámite indicado no existe.");

            RuleFor(x => x.NotaCompraCode)
                .MustAsync(async (c, ct) => await _uow.Logistica.Catalogos.NotasCompra.Query().AnyAsync(n => n.Code == c!.Trim().ToUpper(), ct))
                .WithMessage("La nota indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.NotaCompraCode));

            RuleFor(x => x.UnidadNegocioCode).Cascade(CascadeMode.Stop).NotEmpty()
                .MustAsync(async (c, ct) => await _uow.Logistica.Catalogos.UnidadesNegocio.Query().AnyAsync(u => u.Code == c.Trim().ToUpper(), ct))
                .WithMessage("La unidad de negocio indicada no existe.");

            RuleFor(x => x.FechaEntrega).GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("La fecha de entrega no puede ser anterior a la fecha del sistema.");

            RuleFor(x => x.RowVersion)
                .NotNull()
                .NotEmpty()
                .WithMessage("La versión de la Orden de Compra es obligatoria.");

        }        
    }
}
