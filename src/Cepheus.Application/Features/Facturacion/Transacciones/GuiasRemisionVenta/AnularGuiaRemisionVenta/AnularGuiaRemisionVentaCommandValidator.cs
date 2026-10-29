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

namespace Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.AnularGuiaRemisionVenta;

public sealed class AnularGuiaRemisionVentaCommandValidator : AbstractValidator<AnularGuiaRemisionVentaCommand>
{
    public AnularGuiaRemisionVentaCommandValidator()
    {
        RuleFor(x => x.Number).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Data).NotNull();
        RuleFor(x => x.Data.Reason).NotEmpty().MaximumLength(250);
    }
}
