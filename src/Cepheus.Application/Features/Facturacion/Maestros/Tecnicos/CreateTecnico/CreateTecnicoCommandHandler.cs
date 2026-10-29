using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Tecnicos.Common;
using Cepheus.Domain.Facturacion.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Tecnicos.CreateTecnico
{
    public class CreateTecnicoCommandHandler : IRequestHandler<CreateTecnicoCommand, TecnicoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTecnicoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TecnicoResponse> Handle(CreateTecnicoCommand request, CancellationToken cancellationToken)
        {
            var tecnico = new Tecnico
            {
                TrabajadorCode = request.TrabajadorCode,
                IsActive = true
            };

            await _uow.Facturacion.Maestros.Tecnicos.AddAsync(tecnico, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return new TecnicoResponse
            {
                TrabajadorCode = tecnico.TrabajadorCode,
                IsActive = tecnico.IsActive,
                CreatedAt = tecnico.CreatedAt,
                UpdatedAt = tecnico.UpdatedAt,
                RowVersion = tecnico.RowVersion
            };
        }
    }
}
