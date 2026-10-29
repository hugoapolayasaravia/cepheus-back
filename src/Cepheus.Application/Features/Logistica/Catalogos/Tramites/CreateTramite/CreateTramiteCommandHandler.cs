using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.Tramites.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Tramites.CreateTramite
{
    public class CreateTramiteCommandHandler : IRequestHandler<CreateTramiteCommand, TramiteResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTramiteCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TramiteResponse> Handle(CreateTramiteCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Logistica.Catalogos.Tramites.Query().Select(f => f.Code), length: 1, entityLabel: "Tramites", cancellationToken);


            var tramite = new Tramite
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Logistica.Catalogos.Tramites.AddAsync(tramite, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tramite);
        }

        internal static TramiteResponse Map(Tramite tramite) => new()
        {
            Code = tramite.Code,
            Name = tramite.Name,
            IsActive = tramite.IsActive,
            CreatedAt = tramite.CreatedAt,
            UpdatedAt = tramite.UpdatedAt,
            CreatedBy = tramite.CreatedBy,
            UpdatedBy = tramite.UpdatedBy,
            RowVersion = tramite.RowVersion
        };
    }
}