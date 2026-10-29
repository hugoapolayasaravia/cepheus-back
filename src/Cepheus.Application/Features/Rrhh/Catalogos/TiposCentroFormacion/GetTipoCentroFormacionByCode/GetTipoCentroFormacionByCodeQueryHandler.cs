using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.GetTipoCentroFormacionByCode
{
    public class GetTipoCentroFormacionByCodeQueryHandler
        : IRequestHandler<GetTipoCentroFormacionByCodeQuery, TipoCentroFormacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoCentroFormacionByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoCentroFormacionResponse> Handle(GetTipoCentroFormacionByCodeQuery request, CancellationToken cancellationToken)
        {
            var tipoCentroFormacion = await _uow.Rrhh.Catalogos.TiposCentroFormacion.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TipoCentroFormacionResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoCentroFormacion is null)
            {
                throw new KeyNotFoundException($"Tipo de centro de formación {request.Code} no encontrado.");
            }

            return tipoCentroFormacion;
        }
    }
}