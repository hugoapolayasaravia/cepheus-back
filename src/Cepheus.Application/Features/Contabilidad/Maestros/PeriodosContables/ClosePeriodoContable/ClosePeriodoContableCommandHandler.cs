using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.ClosePeriodoContable
{
    public class ClosePeriodoContableCommandHandler
        : IRequestHandler<ClosePeriodoContableCommand, PeriodoContableResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;

        public ClosePeriodoContableCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<PeriodoContableResponse> Handle(
            ClosePeriodoContableCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var entity = await _uow.Contabilidad.Maestros.PeriodosContables.Query()
                .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Período contable {code} no encontrado.");
            }

            if (entity.Status == EstadoPeriodoContable.Cerrado)
            {
                throw new InvalidOperationException($"El período contable {code} ya está cerrado.");
            }

            entity.Status = EstadoPeriodoContable.Cerrado;
            entity.ClosedAt = DateTime.UtcNow;
            entity.ClosedBy = _currentUser.FullName;

            await _uow.SaveChangesAsync(cancellationToken);

            return PeriodoContableMapper.Map(entity);
        }
    }
}
