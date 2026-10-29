using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.GetCategoriaTrabajadorByCode
{
    public class GetCategoriaTrabajadorByCodeQueryHandler
        : IRequestHandler<GetCategoriaTrabajadorByCodeQuery, CategoriaTrabajadorResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetCategoriaTrabajadorByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CategoriaTrabajadorResponse> Handle(GetCategoriaTrabajadorByCodeQuery request, CancellationToken cancellationToken)
        {
            var categoriaTrabajador = await _uow.Rrhh.Catalogos.CategoriasTrabajador.Query()
                .AsNoTracking()
                .Where(c => c.Code == request.Code)
                .Select(c => new CategoriaTrabajadorResponse
                {
                    Code = c.Code,
                    Name = c.Name,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    RowVersion = c.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (categoriaTrabajador is null)
            {
                throw new KeyNotFoundException($"Categoría de trabajador {request.Code} no encontrada.");
            }

            return categoriaTrabajador;
        }
    }
}