using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSindicatos.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSindicatos.CreateTrabajadorSindicato
{
    public class CreateTrabajadorSindicatoCommandHandler : IRequestHandler<CreateTrabajadorSindicatoCommand, TrabajadorSindicatoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorSindicatoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorSindicatoResponse> Handle(CreateTrabajadorSindicatoCommand request, CancellationToken cancellationToken)
        {
            var entidad = new TrabajadorSindicato
            {
                TrabajadorCode = request.TrabajadorCode,
                Afiliado = request.Afiliado,
                IsActive = true
            };

            await _uow.Rrhh.Maestros.TrabajadorSindicatos.AddAsync(entidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entidad);
        }

        internal static TrabajadorSindicatoResponse Map(TrabajadorSindicato e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            Afiliado = e.Afiliado,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            RowVersion = e.RowVersion
        };
    }
}
