using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSeguros.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSeguros.GetTrabajadorSegurosByTrabajador
{
    public class GetTrabajadorSegurosByTrabajadorQueryHandler : IRequestHandler<GetTrabajadorSegurosByTrabajadorQuery, TrabajadorSeguroResponse?>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorSegurosByTrabajadorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorSeguroResponse?> Handle(GetTrabajadorSegurosByTrabajadorQuery request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorSeguros.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.TrabajadorCode == request.TrabajadorCode, cancellationToken);

            return entidad is null ? null : Map(entidad);
        }

        private static TrabajadorSeguroResponse Map(TrabajadorSeguro e) => new()
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
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
