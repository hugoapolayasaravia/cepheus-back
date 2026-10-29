using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFormacions.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFormacions.GetTrabajadorFormacionsByTrabajador
{
    public class GetTrabajadorFormacionsByTrabajadorQueryHandler : IRequestHandler<GetTrabajadorFormacionsByTrabajadorQuery, List<TrabajadorFormacionResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorFormacionsByTrabajadorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<TrabajadorFormacionResponse>> Handle(GetTrabajadorFormacionsByTrabajadorQuery request, CancellationToken cancellationToken)
        {
            return await _uow.Rrhh.Maestros.TrabajadorFormacions.Query()
                .AsNoTracking()
                .Where(e => e.TrabajadorCode == request.TrabajadorCode)
                .OrderBy(e => e.Id)
                .Select(e => Map(e))
                .ToListAsync(cancellationToken);
        }

        private static TrabajadorFormacionResponse Map(TrabajadorFormacion e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            NivelEducativoCode = e.NivelEducativoCode,
            GradoInstruccionCode = e.GradoInstruccionCode,
            TituloCode = e.TituloCode,
            EspecialidadCode = e.EspecialidadCode,
            TipoCentroFormacionCode = e.TipoCentroFormacionCode,
            ModalidadFormativaCode = e.ModalidadFormativaCode,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
