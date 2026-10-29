// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion/CreateOrdenCompraAdministracion/CreateOrdenCompraAdministracionCommandValidator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.CreateOrdenCompraAdm
{
    public class CreateOrdenCompraAdmCommandValidator
        : OrdenCompraAdmCabeceraValidator<CreateOrdenCompraAdmCommand>
    {
        public CreateOrdenCompraAdmCommandValidator(IUnitOfWork uow) : base(uow)
        {
            RuleFor(x => x.FechaEntrega)
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("La fecha de entrega no puede ser anterior a la fecha del sistema.");

            RuleFor(x => x.Detalles)
                .NotEmpty()
                .WithMessage("La Orden de Compra de Administración debe tener al menos una línea.");

            RuleForEach(x => x.Detalles)
                .ChildRules(line =>
                {
                    line.RuleFor(l => l.Glosa)
                        .NotEmpty()
                        .MaximumLength(500);

                    line.RuleFor(l => l.UnidadMedidaCode)
                        .Cascade(CascadeMode.Stop)
                        .NotEmpty()
                        .MustAsync(async (c, ct) =>
                            await uow.Logistica.Catalogos.UnidadesMedida.Query()
                                .AnyAsync(u => u.Code == c.Trim().ToUpper(), ct))
                        .WithMessage("La unidad de medida indicada no existe.");

                    line.RuleFor(l => l.Cantidad)
                        .GreaterThan(0)
                        .WithMessage("La cantidad debe ser mayor a 0.");

                    line.RuleFor(l => l.Precio)
                        .GreaterThanOrEqualTo(0)
                        .WithMessage("El precio no puede ser negativo.");

                    line.RuleFor(l => l.Descuento)
                        .GreaterThanOrEqualTo(0)
                        .WithMessage("El descuento no puede ser negativo.");

                    line.RuleFor(l => l.SubCentroCostoCode)
                        .Cascade(CascadeMode.Stop)
                        .NotEmpty()
                        .MustAsync(async (c, ct) =>
                            await uow.Logistica.Maestros.SubCentrosCosto.Query()
                                .AnyAsync(s => s.Code == c.Trim().ToUpper(), ct))
                        .WithMessage("El subcentro de costo indicado no existe.");
                });
        }
    }
}
