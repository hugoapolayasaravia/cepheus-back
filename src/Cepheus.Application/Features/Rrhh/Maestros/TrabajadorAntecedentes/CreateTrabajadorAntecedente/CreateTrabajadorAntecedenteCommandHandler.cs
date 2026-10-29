using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorAntecedentes.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorAntecedentes.CreateTrabajadorAntecedente
{
    public class CreateTrabajadorAntecedenteCommandHandler : IRequestHandler<CreateTrabajadorAntecedenteCommand, TrabajadorAntecedenteResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorAntecedenteCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorAntecedenteResponse> Handle(CreateTrabajadorAntecedenteCommand request, CancellationToken cancellationToken)
        {
            var entidad = new TrabajadorAntecedente
            {
                TrabajadorCode = request.TrabajadorCode,
                TieneAntecedentes = request.TieneAntecedentes,
                Descripcion = string.IsNullOrWhiteSpace(request.Descripcion) ? null : request.Descripcion!.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Maestros.TrabajadorAntecedentes.AddAsync(entidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entidad);
        }

        internal static TrabajadorAntecedenteResponse Map(TrabajadorAntecedente e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            TieneAntecedentes = e.TieneAntecedentes,
            Descripcion = e.Descripcion,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            RowVersion = e.RowVersion
        };
    }
}
