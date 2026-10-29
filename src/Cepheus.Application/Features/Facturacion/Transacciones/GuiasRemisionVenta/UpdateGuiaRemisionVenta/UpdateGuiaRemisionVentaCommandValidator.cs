using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.CreateGuiaRemisionVenta;

namespace Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.UpdateGuiaRemisionVenta;

public sealed class UpdateGuiaRemisionVentaCommandValidator : AbstractValidator<UpdateGuiaRemisionVentaCommand>
{
    public UpdateGuiaRemisionVentaCommandValidator()
    {
        RuleFor(x => x.Number).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Data).NotNull();
        RuleFor(x => x.Data).SetValidator(new GuiaRemisionVentaFieldsValidator<UpdateGuiaRemisionVentaRequest>());
        RuleFor(x => x.Data.Detalles).NotEmpty().WithMessage("La guía debe tener al menos una línea.");
        RuleForEach(x => x.Data.Detalles).SetValidator(new GuiaRemisionVentaDetalleRequestValidator());
    }
}
