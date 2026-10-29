using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSeguros.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSeguros.CreateTrabajadorSeguro
{
    public class CreateTrabajadorSeguroCommandHandler : IRequestHandler<CreateTrabajadorSeguroCommand, TrabajadorSeguroResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorSeguroCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorSeguroResponse> Handle(CreateTrabajadorSeguroCommand request, CancellationToken cancellationToken)
        {
            var entidad = new TrabajadorSeguro
            {
                TrabajadorCode = request.TrabajadorCode,
                EpsCode = request.EpsCode,
                SituacionEpsCode = request.SituacionEpsCode,
                NumeroSeguro = string.IsNullOrWhiteSpace(request.NumeroSeguro) ? null : request.NumeroSeguro!.Trim(),
                SctrTipoCode = request.SctrTipoCode,
                SctrSaludCode = request.SctrSaludCode,
                SctrPensionCode = request.SctrPensionCode,
                EpsActivo = request.EpsActivo,
                SeguroMedico = request.SeguroMedico,
                EssaludVida = request.EssaludVida,
                IsActive = true
            };

            await _uow.Rrhh.Maestros.TrabajadorSeguros.AddAsync(entidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entidad);
        }

        internal static TrabajadorSeguroResponse Map(TrabajadorSeguro e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            EpsCode = e.EpsCode,
            SituacionEpsCode = e.SituacionEpsCode,
            NumeroSeguro = e.NumeroSeguro,
            SctrTipoCode = e.SctrTipoCode,
            SctrSaludCode = e.SctrSaludCode,
            SctrPensionCode = e.SctrPensionCode,
            EpsActivo = e.EpsActivo,
            SeguroMedico = e.SeguroMedico,
            EssaludVida = e.EssaludVida,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            RowVersion = e.RowVersion
        };
    }
}
