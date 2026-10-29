using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.GetEspecialidadByCode
{
    public class GetEspecialidadByCodeQueryHandler : IRequestHandler<GetEspecialidadByCodeQuery, EspecialidadTrabajadorResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetEspecialidadByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EspecialidadTrabajadorResponse> Handle(GetEspecialidadByCodeQuery request, CancellationToken cancellationToken)
        {
            var especialidad = await _uow.Rrhh.Catalogos.EspecialidadesTrabajador.Query()
                .AsNoTracking()
                .Where(e => e.Code == request.Code)
                .Select(e => new EspecialidadTrabajadorResponse
                {
                    Code = e.Code,
                    Name = e.Name,
                    IsActive = e.IsActive,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    RowVersion = e.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (especialidad is null)
            {
                throw new KeyNotFoundException($"Especialidad {request.Code} no encontrada.");
            }

            return especialidad;
        }
    }
}