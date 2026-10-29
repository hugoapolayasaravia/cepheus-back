using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.GetEspecialidadByCode
{
    public class GetEspecialidadByCodeQueryHandler
        : IRequestHandler<GetEspecialidadByCodeQuery, EspecialidadResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetEspecialidadByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EspecialidadResponse> Handle(
            GetEspecialidadByCodeQuery request,
            CancellationToken cancellationToken)
        {
            var especialidad = await _uow.Mantenimiento.Catalogos.Especialidades.Query()
                .AsNoTracking()
                .Where(e => e.Code == request.Code)
                .Select(e => new EspecialidadResponse
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
