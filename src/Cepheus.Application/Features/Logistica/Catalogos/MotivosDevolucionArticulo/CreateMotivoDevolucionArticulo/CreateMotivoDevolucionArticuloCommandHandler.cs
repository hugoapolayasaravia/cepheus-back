using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.CreateMotivoDevolucionArticulo;

public sealed class CreateMotivoDevolucionArticuloCommandHandler
    : IRequestHandler<
        CreateMotivoDevolucionArticuloCommand,
        MotivoDevolucionArticuloResponse>
{
    private readonly IUnitOfWork _uow;

    public CreateMotivoDevolucionArticuloCommandHandler(
        IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<MotivoDevolucionArticuloResponse> Handle(
        CreateMotivoDevolucionArticuloCommand request,
        CancellationToken cancellationToken)
    {
        var code = await SequentialCodeGenerator.NextAsync(
            _uow.Logistica.Catalogos.MotivosDevolucionArticulo
                .Query()
                .Select(x => x.Code),
            length: 2,
            entityLabel: "MotivosDevolucionArticulo",
            cancellationToken);

        var motivo = new MotivoDevolucionArticulo
        {
            Code = code,
            Name = request.Name.Trim(),
            AffectsStock = request.AffectsStock,
            IsActive = true
        };

        await _uow.Logistica.Catalogos.MotivosDevolucionArticulo
            .AddAsync(
                motivo,
                cancellationToken);

        await _uow.SaveChangesAsync(cancellationToken);

        return Map(motivo);
    }

    internal static MotivoDevolucionArticuloResponse Map(
        MotivoDevolucionArticulo motivo)
    {
        return new MotivoDevolucionArticuloResponse
        {
            Code = motivo.Code,
            Name = motivo.Name,
            AffectsStock = motivo.AffectsStock,
            IsActive = motivo.IsActive,
            CreatedAt = motivo.CreatedAt,
            UpdatedAt = motivo.UpdatedAt,
            CreatedBy = motivo.CreatedBy,
            UpdatedBy = motivo.UpdatedBy,
            RowVersion = motivo.RowVersion
        };
    }
}