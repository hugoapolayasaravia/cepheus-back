using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSeguros.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSeguros.UpdateTrabajadorSeguro
{
    public class UpdateTrabajadorSeguroCommandHandler : IRequestHandler<UpdateTrabajadorSeguroCommand, TrabajadorSeguroResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorSeguroCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorSeguroResponse> Handle(UpdateTrabajadorSeguroCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Maestros.TrabajadorSeguros.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"TrabajadorSeguro {request.Id} no encontrado.");
            }

            var entidad = new TrabajadorSeguro
            {
                Id = request.Id,
                TrabajadorCode = current.TrabajadorCode,
                EpsCode = request.EpsCode,
                SituacionEpsCode = request.SituacionEpsCode,
                NumeroSeguro = string.IsNullOrWhiteSpace(request.NumeroSeguro) ? null : request.NumeroSeguro!.Trim(),
                SctrTipoCode = request.SctrTipoCode,
                SctrSaludCode = request.SctrSaludCode,
                SctrPensionCode = request.SctrPensionCode,
                EpsActivo = request.EspsActivo,
                SeguroMedico = request.SeguroMedico,
                EssaludVida = request.EssaludVida,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Maestros.TrabajadorSeguros.Update(entidad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTrabajadorSeguro.CreateTrabajadorSeguroCommandHandler.Map(entidad);
        }
    }
}
