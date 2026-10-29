using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.ReopenPeriodoContable
{
    public class ReopenPeriodoContableCommandHandler
        : IRequestHandler<ReopenPeriodoContableCommand, PeriodoContableResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;

        public ReopenPeriodoContableCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<PeriodoContableResponse> Handle(
            ReopenPeriodoContableCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var entity = await _uow.Contabilidad.Maestros.PeriodosContables.Query()
                .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Período contable {code} no encontrado.");
            }

            if (entity.Status == EstadoPeriodoContable.Abierto)
            {
                throw new InvalidOperationException($"El período contable {code} ya está abierto.");
            }

            entity.Status = EstadoPeriodoContable.Abierto;
            entity.ReopenedAt = DateTime.UtcNow;
            entity.ReopenedBy = _currentUser.FullName;

            await _uow.SaveChangesAsync(cancellationToken);

            return PeriodoContableMapper.Map(entity);
        }
    }
}
