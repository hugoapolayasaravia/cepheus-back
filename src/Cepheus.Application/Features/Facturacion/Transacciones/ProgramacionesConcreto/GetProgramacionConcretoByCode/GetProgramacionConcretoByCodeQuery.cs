using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.GetProgramacionConcretoByCode;

public sealed record GetProgramacionConcretoByCodeQuery(string Code) : IRequest<ProgramacionConcretoResponse>;

public sealed class GetProgramacionConcretoByCodeQueryHandler
    : IRequestHandler<GetProgramacionConcretoByCodeQuery, ProgramacionConcretoResponse>
{
    private readonly IUnitOfWork _uow;

    public GetProgramacionConcretoByCodeQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<ProgramacionConcretoResponse> Handle(GetProgramacionConcretoByCodeQuery request, CancellationToken ct)
        => ProgramacionConcretoReader.GetAsync(_uow, ProgramacionConcretoRules.NormalizeCode(request.Code), ct);
}
