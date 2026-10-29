using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Domain.Comunes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.PeriodosIgv.UpdatePeriodoIgv;

public class UpdatePeriodoIgvCommandHandler : IRequestHandler<UpdatePeriodoIgvCommand, PeriodoIgvResponse>
{
    private readonly IUnitOfWork _uow;

    public UpdatePeriodoIgvCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<PeriodoIgvResponse> Handle(UpdatePeriodoIgvCommand request, CancellationToken ct)
    {
        var current = await _uow.Comunes.PeriodosIgv.Query().AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Período de IGV {request.Id} no encontrado.");

        var periodo = new PeriodoIgv
        {
            Id = request.Id,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            PorcentajeIgv = request.PorcentajeIgv,
            CreatedAt = current.CreatedAt,
            CreatedBy = current.CreatedBy,
            RowVersion = request.RowVersion
        };

        _uow.Comunes.PeriodosIgv.Update(periodo);

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException(
                "El período de IGV fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
        }

        return PeriodoIgvResponse.From(periodo);
    }
}
