using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.Common;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.CreateTipoCuentaContable;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.GetTipoCuentaContableById
{
    public class GetTipoCuentaContableByIdQueryHandler : IRequestHandler<GetTipoCuentaContableByIdQuery, TipoCuentaContableResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoCuentaContableByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoCuentaContableResponse> Handle(GetTipoCuentaContableByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Contabilidad.Catalogos.TiposCuentaContable.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Tipo de cuenta contable {request.Code} no encontrado.");
            }

            return CreateTipoCuentaContableCommandHandler.Map(entity);
        }
    }
}
