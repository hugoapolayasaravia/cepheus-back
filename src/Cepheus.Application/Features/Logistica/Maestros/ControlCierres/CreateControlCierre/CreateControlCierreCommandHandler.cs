using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ControlCierres.CreateControlCierre
{
    public class CreateControlCierreCommandHandler : IRequestHandler<CreateControlCierreCommand, ControlCierreResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateControlCierreCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ControlCierreResponse> Handle(CreateControlCierreCommand request, CancellationToken cancellationToken)
        {
            var control = new ControlCierre
            {
                PlantaCode = request.PlantaCode.Trim().ToUpperInvariant(),
                PeriodCode = request.PeriodCode.Trim(),
                ClosureDate = request.ClosureDate,
                DifferenceAmount = request.DifferenceAmount
            };

            await _uow.Logistica.Maestros.ControlCierres.AddAsync(control, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(control);
        }

        internal static ControlCierreResponse Map(ControlCierre control) => new()
        {
            PlantaCode = control.PlantaCode,
            PeriodCode = control.PeriodCode,
            ClosureDate = control.ClosureDate,
            DifferenceAmount = control.DifferenceAmount,
            CreatedAt = control.CreatedAt,
            UpdatedAt = control.UpdatedAt
        };
    }
}
