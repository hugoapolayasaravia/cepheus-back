using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.CreateSctrSalud
{
    public class CreateSctrSaludCommandHandler : IRequestHandler<CreateSctrSaludCommand, SctrSaludResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateSctrSaludCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SctrSaludResponse> Handle(CreateSctrSaludCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.SctrsSalud.Query().Select(s => s.Code), length: 3, entityLabel: "SctrSalud", cancellationToken);

            var sctrSalud = new SctrSalud
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.SctrsSalud.AddAsync(sctrSalud, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(sctrSalud);
        }

        internal static SctrSaludResponse Map(SctrSalud sctrSalud) => new()
        {
            Code = sctrSalud.Code,
            Name = sctrSalud.Name,
            IsActive = sctrSalud.IsActive,
            CreatedAt = sctrSalud.CreatedAt,
            UpdatedAt = sctrSalud.UpdatedAt,
            RowVersion = sctrSalud.RowVersion
        };
    }
}