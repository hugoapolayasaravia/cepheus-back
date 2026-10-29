using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorBeneficios.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorBeneficios.CreateTrabajadorBeneficio
{
    public class CreateTrabajadorBeneficioCommandHandler : IRequestHandler<CreateTrabajadorBeneficioCommand, TrabajadorBeneficioResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorBeneficioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorBeneficioResponse> Handle(CreateTrabajadorBeneficioCommand request, CancellationToken cancellationToken)
        {
            var entidad = new TrabajadorBeneficio
            {
                TrabajadorCode = request.TrabajadorCode,
                Cts = request.Cts,
                Gratificacion = request.Gratificacion,
                Vacaciones = request.Vacaciones,
                MovilidadAntesEntrada = request.MovilidadAntesEntrada,
                MovilidadDespuesSalida = request.MovilidadDespuesSalida,
                Refrigerio = request.Refrigerio,
                Cena = request.Cena,
                Vale = request.Vale,
                IsActive = true
            };

            await _uow.Rrhh.Maestros.TrabajadorBeneficios.AddAsync(entidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entidad);
        }

        internal static TrabajadorBeneficioResponse Map(TrabajadorBeneficio e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            Cts = e.Cts,
            Gratificacion = e.Gratificacion,
            Vacaciones = e.Vacaciones,
            MovilidadAntesEntrada = e.MovilidadAntesEntrada,
            MovilidadDespuesSalida = e.MovilidadDespuesSalida,
            Refrigerio = e.Refrigerio,
            Cena = e.Cena,
            Vale = e.Vale,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            RowVersion = e.RowVersion
        };
    }
}
