using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.Common;
using Cepheus.Domain.Mantenimiento.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.CreateMaquina
{
    public class CreateMaquinaCommandHandler
        : IRequestHandler<CreateMaquinaCommand, MaquinaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateMaquinaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<MaquinaResponse> Handle(
            CreateMaquinaCommand request,
            CancellationToken cancellationToken)
        {
            var maquina = new Maquina
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Mantenimiento.Catalogos.Maquinas.AddAsync(maquina, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(maquina);
        }

        internal static MaquinaResponse Map(Maquina maquina) => new()
        {
            Code = maquina.Code,
            Name = maquina.Name,
            IsActive = maquina.IsActive,
            CreatedAt = maquina.CreatedAt,
            UpdatedAt = maquina.UpdatedAt,
            RowVersion = maquina.RowVersion
        };
    }
}
