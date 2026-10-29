using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFormacions.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFormacions.CreateTrabajadorFormacion
{
    public class CreateTrabajadorFormacionCommandHandler : IRequestHandler<CreateTrabajadorFormacionCommand, TrabajadorFormacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorFormacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorFormacionResponse> Handle(CreateTrabajadorFormacionCommand request, CancellationToken cancellationToken)
        {
            var entidad = new TrabajadorFormacion
            {
                TrabajadorCode = request.TrabajadorCode,
                NivelEducativoCode = request.NivelEducativoCode,
                GradoInstruccionCode = request.GradoInstruccionCode,
                TituloCode = request.TituloCode,
                EspecialidadCode = request.EspecialidadCode,
                TipoCentroFormacionCode = request.TipoCentroFormacionCode,
                ModalidadFormativaCode = request.ModalidadFormativaCode,
                IsActive = true
            };

            await _uow.Rrhh.Maestros.TrabajadorFormacions.AddAsync(entidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entidad);
        }

        internal static TrabajadorFormacionResponse Map(TrabajadorFormacion e) => new()
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
            RowVersion = e.RowVersion
        };
    }
}
