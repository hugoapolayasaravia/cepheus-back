using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.CreateDocumentoCompra
{
    public class CreateDocumentoCompraCommandValidator : DocumentoCompraCabeceraValidator<CreateDocumentoCompraCommand>
    {
        public CreateDocumentoCompraCommandValidator(IUnitOfWork uow) : base(uow)
        {
            RuleFor(x => x.Detalles)
                .NotEmpty()
                .WithMessage("El documento debe tener al menos una línea.");

            RuleForEach(x => x.Detalles)
                .ChildRules(line =>
                {
                    line.RuleFor(l => l.PlantaCode)
                        .Cascade(CascadeMode.Stop)
                        .NotEmpty()
                        .MustAsync(async (c, ct) =>
                            await uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == c.Trim().ToUpper(), ct))
                        .WithMessage("La planta indicada no existe.");

                    line.RuleFor(l => l.CuentaContableCode)
                        .Cascade(CascadeMode.Stop)
                        .NotEmpty()
                        .MustAsync(async (c, ct) =>
                            await uow.Contabilidad.Maestros.CuentasContables.Query()
                                .AnyAsync(a => a.Code == c.Trim().ToUpper(), ct))
                        .WithMessage("La cuenta contable indicada no existe.");

                    line.RuleFor(l => l)
                        .Must(l => string.IsNullOrWhiteSpace(l.AnexoTipoCode) == string.IsNullOrWhiteSpace(l.AnexoCode))
                        .WithMessage("El anexo requiere tipo de anexo y código.")
                        .OverridePropertyName("Anexo");

                    line.RuleFor(l => l)
                        .MustAsync(async (l, ct) =>
                        {
                            var tip = l.AnexoTipoCode!.Trim().ToUpper();
                            var ane = l.AnexoCode!.Trim().ToUpper();
                            return await uow.Contabilidad.Maestros.Anexos.Query()
                                .AnyAsync(a => a.TipoAnexoCode == tip && a.Code == ane, ct);
                        })
                        .WithMessage("El anexo indicado no existe.")
                        .OverridePropertyName("Anexo")
                        .When(l => !string.IsNullOrWhiteSpace(l.AnexoTipoCode) && !string.IsNullOrWhiteSpace(l.AnexoCode));

                    line.RuleFor(l => l.Detalle).NotEmpty().MaximumLength(50);

                    line.RuleFor(l => l.Cantidad).GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");
                    line.RuleFor(l => l.PrecioSol).GreaterThanOrEqualTo(0).WithMessage("El precio no puede ser negativo.");

                    line.RuleFor(l => l.UnidadMedidaCode)
                        .Cascade(CascadeMode.Stop)
                        .NotEmpty()
                        .MustAsync(async (c, ct) =>
                            await uow.Logistica.Catalogos.UnidadesMedida.Query()
                                .AnyAsync(u => u.Code == c.Trim().ToUpper(), ct))
                        .WithMessage("La unidad de medida indicada no existe.");

                    line.RuleFor(l => l.NumeroGuiaVenta).MaximumLength(10);

                    line.RuleFor(l => l)
                        .Must(l => string.IsNullOrWhiteSpace(l.ProductoTipoCode) == string.IsNullOrWhiteSpace(l.ProductoCode))
                        .WithMessage("El producto requiere tipo de producto y código.")
                        .OverridePropertyName("Producto");

                    line.RuleFor(l => l)
                        .MustAsync(async (l, ct) =>
                        {
                            var tip = l.ProductoTipoCode!.Trim().ToUpper();
                            var cod = l.ProductoCode!.Trim().ToUpper();
                            return await uow.Facturacion.Maestros.Productos.Query()
                                .AnyAsync(p => p.TipoProductoCode == tip && p.Code == cod, ct);
                        })
                        .WithMessage("El producto indicado no existe.")
                        .OverridePropertyName("Producto")
                        .When(l => !string.IsNullOrWhiteSpace(l.ProductoTipoCode) && !string.IsNullOrWhiteSpace(l.ProductoCode));
                });
        }
    }
}
