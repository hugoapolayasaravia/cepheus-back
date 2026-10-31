using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.GetMisPlantas
{
    public record MiPlantaResponse(string Code, string Name);

    /// <summary>Plantas en las que el usuario autenticado está inscrito (tabla UsuariosPlanta).</summary>
    public record GetMisPlantasQuery : IRequest<List<MiPlantaResponse>>;

    public class GetMisPlantasQueryHandler : IRequestHandler<GetMisPlantasQuery, List<MiPlantaResponse>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _current;

        public GetMisPlantasQueryHandler(IUnitOfWork uow, ICurrentUserService current)
        {
            _uow = uow;
            _current = current;
        }

        public async Task<List<MiPlantaResponse>> Handle(GetMisPlantasQuery request, CancellationToken ct)
        {
            var userId = _current.UserId;
            if (userId is null) return new();

            return await _uow.Logistica.Catalogos.UsuariosPlanta.Query().AsNoTracking()
                .Where(x => x.UserId == userId.Value)
                .OrderBy(x => x.PlantaCode)
                .Select(x => new MiPlantaResponse(x.PlantaCode, x.Planta.Name))
                .ToListAsync(ct);
        }
    }
}
