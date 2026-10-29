using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.CreateTipoPension
{
    public class CreateTipoPensionCommandHandler : IRequestHandler<CreateTipoPensionCommand, TipoPensionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoPensionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoPensionResponse> Handle(CreateTipoPensionCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.TiposPension.Query().Select(t => t.Code), length: 3, entityLabel: "TiposPension", cancellationToken);

            var tipoPension = new TipoPension
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.TiposPension.AddAsync(tipoPension, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoPension);
        }

        internal static TipoPensionResponse Map(TipoPension tipoPension) => new()
        {
            Code = tipoPension.Code,
            Name = tipoPension.Name,
            IsActive = tipoPension.IsActive,
            CreatedAt = tipoPension.CreatedAt,
            UpdatedAt = tipoPension.UpdatedAt,
            RowVersion = tipoPension.RowVersion
        };
    }
}