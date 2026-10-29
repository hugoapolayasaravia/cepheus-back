using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.CreateTipoSangre
{
    public class CreateTipoSangreCommandHandler : IRequestHandler<CreateTipoSangreCommand, TipoSangreResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoSangreCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoSangreResponse> Handle(CreateTipoSangreCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.TiposSangre.Query().Select(t => t.Code), length: 3, entityLabel: "TiposSangre", cancellationToken);

            var tipoSangre = new TipoSangre
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.TiposSangre.AddAsync(tipoSangre, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoSangre);
        }

        internal static TipoSangreResponse Map(TipoSangre tipoSangre) => new()
        {
            Code = tipoSangre.Code,
            Name = tipoSangre.Name,
            IsActive = tipoSangre.IsActive,
            CreatedAt = tipoSangre.CreatedAt,
            UpdatedAt = tipoSangre.UpdatedAt,
            RowVersion = tipoSangre.RowVersion
        };
    }
}