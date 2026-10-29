using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorRemuneracions.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorRemuneracions.CreateTrabajadorRemuneracion
{
    public class CreateTrabajadorRemuneracionCommandHandler : IRequestHandler<CreateTrabajadorRemuneracionCommand, TrabajadorRemuneracionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorRemuneracionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorRemuneracionResponse> Handle(CreateTrabajadorRemuneracionCommand request, CancellationToken cancellationToken)
        {
            var entidad = new TrabajadorRemuneracion
            {
                TrabajadorCode = request.TrabajadorCode,
                SueldoBasico = request.SueldoBasico,
                MonedaCode = string.IsNullOrWhiteSpace(request.MonedaCode) ? null : request.MonedaCode!.Trim(),
                ModoPagoCode = request.ModoPagoCode,
                IsActive = true
            };

            await _uow.Rrhh.Maestros.TrabajadorRemuneracions.AddAsync(entidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entidad);
        }

        internal static TrabajadorRemuneracionResponse Map(TrabajadorRemuneracion e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            SueldoBasico = e.SueldoBasico,
            MonedaCode = e.MonedaCode,
            ModoPagoCode = e.ModoPagoCode,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            RowVersion = e.RowVersion
        };
    }
}
