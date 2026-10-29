using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.Common;
using Cepheus.Domain.Mantenimiento.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.CreateUnidadParada
{
    /// <summary>Reemplaza Logi_sp_AgregaMUnidad_Parada: no graba con 0 horas; inserta o actualiza por clave.</summary>
    public class CreateUnidadParadaCommandHandler : IRequestHandler<CreateUnidadParadaCommand, UnidadParadaResponse?>
    {
        private readonly IUnitOfWork _uow;

        public CreateUnidadParadaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UnidadParadaResponse?> Handle(
            CreateUnidadParadaCommand request, CancellationToken cancellationToken)
        {
            // Legacy: if @Horas_Pad = 0 return
            if (request.Horas == 0)
            {
                return null;
            }

            var scc = request.SubCentroCostoCode.Trim().ToUpperInvariant();
            var fecha = request.Fecha.Date;
            var plantaAfectada = request.PlantaAfectadaCode.Trim().ToUpperInvariant();
            var fallaCode = string.IsNullOrWhiteSpace(request.FallaCode)
                ? null
                : request.FallaCode.Trim().ToUpperInvariant();

            // Con seguimiento: si existe se actualiza (el último dato gana, como el UPDATE del SP legado).
            var existing = await _uow.Mantenimiento.Transacciones.UnidadesParada.Query()
                .FirstOrDefaultAsync(x => x.SubCentroCostoCode == scc && x.Fecha == fecha, cancellationToken);

            if (existing is null)
            {
                await _uow.Mantenimiento.Transacciones.UnidadesParada.AddAsync(new UnidadParada
                {
                    SubCentroCostoCode = scc,
                    Fecha = fecha,
                    Horas = request.Horas,
                    Estado = request.Estado,
                    PlantaAfectadaCode = plantaAfectada,
                    FallaCode = fallaCode
                }, cancellationToken);
            }
            else
            {
                existing.Horas = request.Horas;
                existing.Estado = request.Estado;
                existing.PlantaAfectadaCode = plantaAfectada;
                existing.FallaCode = fallaCode;
            }

            await _uow.SaveChangesAsync(cancellationToken);

            return await _uow.Mantenimiento.Transacciones.UnidadesParada.Query()
                .AsNoTracking()
                .Where(x => x.SubCentroCostoCode == scc && x.Fecha == fecha)
                .Select(UnidadParadaProjection.ToResponse)
                .FirstAsync(cancellationToken);
        }
    }
}
