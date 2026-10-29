using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.AnularProgramacionOtros;

public sealed class AnularProgramacionOtrosCommandValidator : AbstractValidator<AnularProgramacionOtrosCommand>
{
    public AnularProgramacionOtrosCommandValidator() => RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
}
