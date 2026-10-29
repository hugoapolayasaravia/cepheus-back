using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.CreateSctrPension
{
    public class CreateSctrPensionCommandHandler : IRequestHandler<CreateSctrPensionCommand, SctrPensionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateSctrPensionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SctrPensionResponse> Handle(CreateSctrPensionCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.SctrsPension.Query().Select(s => s.Code), length: 3, entityLabel: "SctrPension", cancellationToken);

            var sctrPension = new SctrPension
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.SctrsPension.AddAsync(sctrPension, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(sctrPension);
        }

        internal static SctrPensionResponse Map(SctrPension sctrPension) => new()
        {
            Code = sctrPension.Code,
            Name = sctrPension.Name,
            IsActive = sctrPension.IsActive,
            CreatedAt = sctrPension.CreatedAt,
            UpdatedAt = sctrPension.UpdatedAt,
            RowVersion = sctrPension.RowVersion
        };
    }
}