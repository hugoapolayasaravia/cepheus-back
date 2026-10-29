using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.UpdateMotivoDevolucionArticulo;

public sealed class UpdateMotivoDevolucionArticuloCommandHandler
    : IRequestHandler<
        UpdateMotivoDevolucionArticuloCommand,
        MotivoDevolucionArticuloResponse>
{
    private readonly IUnitOfWork _uow;

    public UpdateMotivoDevolucionArticuloCommandHandler(
        IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<MotivoDevolucionArticuloResponse> Handle(
        UpdateMotivoDevolucionArticuloCommand request,
        CancellationToken cancellationToken)
    {
        var code = request.Code.Trim().ToUpperInvariant();

        var current =
            await _uow.Logistica.Catalogos.MotivosDevolucionArticulo
                .Query()
                .FirstOrDefaultAsync(
                    m => m.Code == code,
                    cancellationToken);

        if (current is null)
        {
            throw new KeyNotFoundException(
                $"Motivo de devolución {request.Code} no encontrado.");
        }

        // Actualizamos la entidad de dominio existente.
        current.Name = request.Name.Trim();
        current.AffectsStock = request.AffectsStock;

        _uow.Logistica.Catalogos.MotivosDevolucionArticulo.Update(current);

        try
        {
            await _uow.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException(
                "El motivo de devolución fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
        }

        return new MotivoDevolucionArticuloResponse
        {
            Code = current.Code,
            Name = current.Name,
            AffectsStock = current.AffectsStock,
            IsActive = current.IsActive,
            CreatedAt = current.CreatedAt,
            UpdatedAt = current.UpdatedAt,
            CreatedBy = current.CreatedBy,
            UpdatedBy = current.UpdatedBy,
            RowVersion = current.RowVersion
        };
    }
}