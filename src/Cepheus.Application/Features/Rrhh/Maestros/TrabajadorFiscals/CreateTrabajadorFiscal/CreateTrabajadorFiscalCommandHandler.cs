using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFiscals.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFiscals.CreateTrabajadorFiscal
{
    public class CreateTrabajadorFiscalCommandHandler : IRequestHandler<CreateTrabajadorFiscalCommand, TrabajadorFiscalResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorFiscalCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorFiscalResponse> Handle(CreateTrabajadorFiscalCommand request, CancellationToken cancellationToken)
        {
            var entidad = new TrabajadorFiscal
            {
                TrabajadorCode = request.TrabajadorCode,
                ConInmTrabajador = request.ConInmTrabajador,
                Domiciliado = request.Domiciliado,
                OtrosIngresosQuinta = request.OtrosIngresosQuinta,
                RentaQuintaExonerada = request.RentaQuintaExonerada,
                MadreResFamiliar = request.MadreResFamiliar,
                IsActive = true
            };

            await _uow.Rrhh.Maestros.TrabajadorFiscals.AddAsync(entidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entidad);
        }

        internal static TrabajadorFiscalResponse Map(TrabajadorFiscal e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            ConInmTrabajador = e.ConInmTrabajador,
            Domiciliado = e.Domiciliado,
            OtrosIngresosQuinta = e.OtrosIngresosQuinta,
            RentaQuintaExonerada = e.RentaQuintaExonerada,
            MadreResFamiliar = e.MadreResFamiliar,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            RowVersion = e.RowVersion
        };
    }
}
