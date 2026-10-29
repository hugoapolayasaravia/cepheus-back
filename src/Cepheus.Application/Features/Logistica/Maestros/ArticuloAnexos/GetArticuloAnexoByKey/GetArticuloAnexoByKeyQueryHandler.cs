using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.GetArticuloAnexoByKey
{
    public class GetArticuloAnexoByKeyQueryHandler : IRequestHandler<GetArticuloAnexoByKeyQuery, ArticuloAnexoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetArticuloAnexoByKeyQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ArticuloAnexoResponse> Handle(
            GetArticuloAnexoByKeyQuery request, CancellationToken cancellationToken)
        {
            var scc = request.SubCentroCostoCode.Trim().ToUpperInvariant();
            var art = request.ArticuloCode.Trim().ToUpperInvariant();

            var entity = await _uow.Logistica.Maestros.ArticuloAnexos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.SubCentroCostoCode == scc && a.ArticuloCode == art, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No existe el enlace de anexo del subcentro {scc} y el artículo {art}.");
            }

            return ArticuloAnexoMapper.Map(entity);
        }
    }
}
