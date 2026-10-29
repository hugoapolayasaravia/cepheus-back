using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.GetMaquinaByCode
{
    public class GetMaquinaByCodeQueryHandler
        : IRequestHandler<GetMaquinaByCodeQuery, MaquinaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetMaquinaByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<MaquinaResponse> Handle(
            GetMaquinaByCodeQuery request,
            CancellationToken cancellationToken)
        {
            var maquina = await _uow.Mantenimiento.Catalogos.Maquinas.Query()
                .AsNoTracking()
                .Where(m => m.Code == request.Code)
                .Select(m => new MaquinaResponse
                {
                    Code = m.Code,
                    Name = m.Name,
                    IsActive = m.IsActive,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    RowVersion = m.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (maquina is null)
            {
                throw new KeyNotFoundException($"Máquina {request.Code} no encontrada.");
            }

            return maquina;
        }
    }
}
