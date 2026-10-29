using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.GetPeriodoContableByCode
{
    public class GetPeriodoContableByCodeQueryHandler
        : IRequestHandler<GetPeriodoContableByCodeQuery, PeriodoContableResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetPeriodoContableByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PeriodoContableResponse> Handle(
            GetPeriodoContableByCodeQuery request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var entity = await _uow.Contabilidad.Maestros.PeriodosContables.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Período contable {code} no encontrado.");
            }

            return PeriodoContableMapper.Map(entity);
        }
    }
}
