using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.GetProgramacionOtrosByCode;

public sealed record GetProgramacionOtrosByCodeQuery(string Code) : IRequest<ProgramacionOtrosResponse>;

public sealed class GetProgramacionOtrosByCodeQueryHandler
    : IRequestHandler<GetProgramacionOtrosByCodeQuery, ProgramacionOtrosResponse>
{
    private readonly IUnitOfWork _uow;

    public GetProgramacionOtrosByCodeQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<ProgramacionOtrosResponse> Handle(GetProgramacionOtrosByCodeQuery request, CancellationToken ct)
        => ProgramacionOtrosReader.GetAsync(_uow, ProgramacionOtrosRules.NormalizeCode(request.Code), ct);
}
