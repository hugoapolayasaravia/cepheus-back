using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.CreateOficina
{
    public class CreateOficinaCommandHandler : IRequestHandler<CreateOficinaCommand, OficinaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateOficinaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OficinaResponse> Handle(CreateOficinaCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.Oficinas.Query().Select(o => o.Code), length: 3, entityLabel: "Oficinas", cancellationToken);

            var oficina = new Oficina
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.Oficinas.AddAsync(oficina, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(oficina);
        }

        internal static OficinaResponse Map(Oficina oficina) => new()
        {
            Code = oficina.Code,
            Name = oficina.Name,
            IsActive = oficina.IsActive,
            CreatedAt = oficina.CreatedAt,
            UpdatedAt = oficina.UpdatedAt,
            RowVersion = oficina.RowVersion
        };
    }
}