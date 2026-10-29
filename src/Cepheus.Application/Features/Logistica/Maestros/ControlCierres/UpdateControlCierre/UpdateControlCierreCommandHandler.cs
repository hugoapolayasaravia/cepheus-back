using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ControlCierres.UpdateControlCierre
{
    public class UpdateControlCierreCommandHandler : IRequestHandler<UpdateControlCierreCommand, ControlCierreResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateControlCierreCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ControlCierreResponse> Handle(UpdateControlCierreCommand request, CancellationToken cancellationToken)
        {
            var control = await _uow.Logistica.Maestros.ControlCierres.Query()
                .FirstOrDefaultAsync(c => c.PlantaCode == request.PlantaCode && c.PeriodCode == request.PeriodCode, cancellationToken);

            if (control is null)
            {
                throw new KeyNotFoundException(
                    $"No hay cierre registrado para planta {request.PlantaCode} en el período {request.PeriodCode}.");
            }

            control.ClosureDate = request.ClosureDate;
            control.DifferenceAmount = request.DifferenceAmount;

            await _uow.SaveChangesAsync(cancellationToken);

            return CreateControlCierre.CreateControlCierreCommandHandler.Map(control);
        }
    }
}
