using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContables.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContables.CreateTrabajadorContable
{
    public class CreateTrabajadorContableCommandHandler : IRequestHandler<CreateTrabajadorContableCommand, TrabajadorContableResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorContableCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorContableResponse> Handle(CreateTrabajadorContableCommand request, CancellationToken cancellationToken)
        {
            var entidad = new TrabajadorContable
            {
                TrabajadorCode = request.TrabajadorCode,
                NumeroItem = request.NumeroItem,
                CuentaContable = string.IsNullOrWhiteSpace(request.CuentaContable) ? null : request.CuentaContable!.Trim(),
                Tipo = string.IsNullOrWhiteSpace(request.Tipo) ? null : request.Tipo!.Trim(),
                Porcentaje = request.Porcentaje,
                IsActive = true
            };

            await _uow.Rrhh.Maestros.TrabajadorContables.AddAsync(entidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entidad);
        }

        internal static TrabajadorContableResponse Map(TrabajadorContable e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            NumeroItem = e.NumeroItem,
            CuentaContable = e.CuentaContable,
            Tipo = e.Tipo,
            Porcentaje = e.Porcentaje,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            RowVersion = e.RowVersion
        };
    }
}
