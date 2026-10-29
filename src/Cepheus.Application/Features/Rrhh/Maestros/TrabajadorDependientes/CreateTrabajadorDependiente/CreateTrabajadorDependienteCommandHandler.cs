using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDependientes.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDependientes.CreateTrabajadorDependiente
{
    public class CreateTrabajadorDependienteCommandHandler : IRequestHandler<CreateTrabajadorDependienteCommand, TrabajadorDependienteResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorDependienteCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorDependienteResponse> Handle(CreateTrabajadorDependienteCommand request, CancellationToken cancellationToken)
        {
            var entidad = new TrabajadorDependiente
            {
                TrabajadorCode = request.TrabajadorCode,
                Nombre = request.Nombre.Trim(),
                ParentescoCode = request.ParentescoCode,
                FechaNacimiento = request.FechaNacimiento,
                Documento = string.IsNullOrWhiteSpace(request.Documento) ? null : request.Documento!.Trim(),
                Asegurado = request.Asegurado,
                IsActive = true
            };

            await _uow.Rrhh.Maestros.TrabajadorDependientes.AddAsync(entidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entidad);
        }

        internal static TrabajadorDependienteResponse Map(TrabajadorDependiente e) => new()
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
            RowVersion = e.RowVersion
        };
    }
}
