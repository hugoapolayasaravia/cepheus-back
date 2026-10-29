using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.Common;
using Cepheus.Domain.Mantenimiento.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.CreateObjetoActividad
{
    public class CreateObjetoActividadCommandHandler
        : IRequestHandler<CreateObjetoActividadCommand, ObjetoActividadResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateObjetoActividadCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ObjetoActividadResponse> Handle(
            CreateObjetoActividadCommand request,
            CancellationToken cancellationToken)
        {
            var objetoActividad = new ObjetoActividad
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Mantenimiento.Maestros.ObjetosActividad.AddAsync(objetoActividad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(objetoActividad);
        }

        internal static ObjetoActividadResponse Map(ObjetoActividad objetoActividad) => new()
        {
            Code = objetoActividad.Code,
            Name = objetoActividad.Name,
            IsActive = objetoActividad.IsActive,
            CreatedAt = objetoActividad.CreatedAt,
            UpdatedAt = objetoActividad.UpdatedAt,
            RowVersion = objetoActividad.RowVersion
        };
    }
}
