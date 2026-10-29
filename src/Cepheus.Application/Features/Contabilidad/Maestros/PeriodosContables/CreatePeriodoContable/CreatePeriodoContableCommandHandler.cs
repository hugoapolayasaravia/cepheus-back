using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.CreatePeriodoContable
{
    public class CreatePeriodoContableCommandHandler
        : IRequestHandler<CreatePeriodoContableCommand, PeriodoContableResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreatePeriodoContableCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PeriodoContableResponse> Handle(
            CreatePeriodoContableCommand request, CancellationToken cancellationToken)
        {
            var entity = new PeriodoContable
            {
                Code = $"{request.Year:0000}{request.Month:00}",
                Year = request.Year,
                Month = request.Month,
                Status = EstadoPeriodoContable.Abierto
            };

            await _uow.Contabilidad.Maestros.PeriodosContables.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return PeriodoContableMapper.Map(entity);
        }
    }
}
