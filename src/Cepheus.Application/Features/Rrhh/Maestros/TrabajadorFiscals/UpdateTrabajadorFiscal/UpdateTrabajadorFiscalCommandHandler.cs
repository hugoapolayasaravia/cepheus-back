using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFiscals.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFiscals.UpdateTrabajadorFiscal
{
    public class UpdateTrabajadorFiscalCommandHandler : IRequestHandler<UpdateTrabajadorFiscalCommand, TrabajadorFiscalResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorFiscalCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorFiscalResponse> Handle(UpdateTrabajadorFiscalCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Maestros.TrabajadorFiscals.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"TrabajadorFiscal {request.Id} no encontrado.");
            }

            var entidad = new TrabajadorFiscal
            {
                Id = request.Id,
                TrabajadorCode = current.TrabajadorCode,
                ConInmTrabajador = request.ConInmTrabajador,
                Domiciliado = request.Domiciliado,
                OtrosIngresosQuinta = request.OtrosIngresosQuinta,
                RentaQuintaExonerada = request.RentaQuintaExonerada,
                MadreResFamiliar = request.MadreResFamiliar,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Maestros.TrabajadorFiscals.Update(entidad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTrabajadorFiscal.CreateTrabajadorFiscalCommandHandler.Map(entidad);
        }
    }
}
