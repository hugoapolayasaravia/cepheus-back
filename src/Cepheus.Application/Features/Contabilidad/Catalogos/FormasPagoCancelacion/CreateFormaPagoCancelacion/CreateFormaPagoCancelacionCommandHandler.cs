using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.Common;
using Cepheus.Domain.Contabilidad.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.CreateFormaPagoCancelacion
{
    public class CreateFormaPagoCancelacionCommandHandler
        : IRequestHandler<CreateFormaPagoCancelacionCommand, FormaPagoCancelacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateFormaPagoCancelacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<FormaPagoCancelacionResponse> Handle(
            CreateFormaPagoCancelacionCommand request,
            CancellationToken cancellationToken)
        {
            var entity = new FormaPagoCancelacion
            {
                Code = request.Code.Trim(),
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Contabilidad.Catalogos.FormasPagoCancelacion.AddAsync(
                entity,
                cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entity);
        }

        internal static FormaPagoCancelacionResponse Map(FormaPagoCancelacion e) => new()
        {
            Code = e.Code,
            Name = e.Name,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
