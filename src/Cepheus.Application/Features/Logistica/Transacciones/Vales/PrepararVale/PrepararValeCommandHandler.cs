using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.PrepararVale;

public sealed class PrepararValeCommandHandler
    : IRequestHandler<PrepararValeCommand, ValeResponse>
{
    private readonly IUnitOfWork _uow;

    public PrepararValeCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<ValeResponse> Handle(
        PrepararValeCommand request,
        CancellationToken ct)
    {
        var planta = ValeRules.Normalize(request.PlantaCode);
        var code = ValeRules.Normalize(request.Code);

        var vale = await ValeReader.LoadForUpdateAsync(_uow, planta, code, ct);

        if (vale.Estado is not (EstadoVale.Aprobado or EstadoVale.EntregaParcial))
        {
            throw new InvalidOperationException(
                $"El Vale de Salida se encuentra {vale.Estado}; solo se prepara desde Aprobado o Entrega Parcial.");
        }

        if (ValeRules.EsTransferencia(vale))
        {
            throw new InvalidOperationException(
                "Un vale de tipo Transferencia no puede ser preparado por el almacén origen.");
        }

        await ValeRules.EnsurePeriodOpenAsync(
            _uow,
            planta,
            vale.FechaEntrega,
            "La Fecha de Entrega no puede ser menor o igual al cierre del período.",
            ct);

        vale.Preparado = true;

        await ValeReader.SaveAsync(_uow, ct);

        return await ValeReader.GetAsync(_uow, planta, code, ct);
    }
}
