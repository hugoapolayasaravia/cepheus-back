using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDependientes.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDependientes.GetTrabajadorDependientesByTrabajador
{
    public class GetTrabajadorDependientesByTrabajadorQueryHandler : IRequestHandler<GetTrabajadorDependientesByTrabajadorQuery, List<TrabajadorDependienteResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorDependientesByTrabajadorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<TrabajadorDependienteResponse>> Handle(GetTrabajadorDependientesByTrabajadorQuery request, CancellationToken cancellationToken)
        {
            return await _uow.Rrhh.Maestros.TrabajadorDependientes.Query()
                .AsNoTracking()
                .Where(e => e.TrabajadorCode == request.TrabajadorCode)
                .OrderBy(e => e.Id)
                .Select(e => Map(e))
                .ToListAsync(cancellationToken);
        }

        private static TrabajadorDependienteResponse Map(TrabajadorDependiente e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            Nombre = e.Nombre,
            ParentescoCode = e.ParentescoCode,
            FechaNacimiento = e.FechaNacimiento,
            Documento = e.Documento,
            Asegurado = e.Asegurado,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
