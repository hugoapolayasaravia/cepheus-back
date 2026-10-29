using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using Cepheus.Domain.Mantenimiento.Transacciones;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.UpdateVale;

public sealed class UpdateValeCommandHandler
    : IRequestHandler<UpdateValeCommand, ValeResponse>
{
    private readonly IUnitOfWork _uow;

    public UpdateValeCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<ValeResponse> Handle(
        UpdateValeCommand request,
        CancellationToken ct)
    {
        var planta = ValeRules.Normalize(request.PlantaCode);
        var code = ValeRules.Normalize(request.Code);

        var vale = await ValeReader.LoadForUpdateAsync(_uow, planta, code, ct);

        ValeRules.EnsureEditable(vale, "modificar");

        OrdenTrabajo? ordenTrabajo = null;

        if (vale.OrdenTrabajoCode is not null)
        {
            ordenTrabajo = await ValeCabecera.LoadOrdenTrabajoEnEjecucionAsync(
                _uow,
                planta,
                vale.OrdenTrabajoCode,
                ct);
        }

        var cabecera = await ValeCabecera.ResolverAsync(
            _uow,
            planta,
            vale.TipoValeCode,
            request.FechaEntrega,
            request.SubCentroCostoCode,
            request.SubCentroEjecutorCode,
            request.TrabajadorCode,
            request.UnidadNegocioCode,
            request.PlantaAfectadaCode,
            vale.OrdenTrabajoCode,
            ordenTrabajo,
            ct);

        vale.FechaEntrega = cabecera.FechaEntrega;
        vale.SubCentroCostoCode = cabecera.SubCentroCostoCode;
        vale.SubCentroEjecutorCode = cabecera.SubCentroEjecutorCode;
        vale.TrabajadorCode = cabecera.TrabajadorCode;
        vale.UnidadNegocioCode = cabecera.UnidadNegocioCode;
        vale.PlantaAfectadaCode = cabecera.PlantaAfectadaCode;

        // El IGV depende de la fecha de entrega: se recalculan los importes.
        await ValeRules.RecalculateTotalsAsync(_uow, vale, ct);

        _uow.Logistica.Transacciones.Vales.Update(vale);
        await ValeReader.SaveAsync(_uow, ct);

        return await ValeReader.GetAsync(_uow, planta, code, ct);
    }
}
