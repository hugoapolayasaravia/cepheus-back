using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContables.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContables.GetTrabajadorContablesByTrabajador
{
    public class GetTrabajadorContablesByTrabajadorQueryHandler : IRequestHandler<GetTrabajadorContablesByTrabajadorQuery, List<TrabajadorContableResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorContablesByTrabajadorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<TrabajadorContableResponse>> Handle(GetTrabajadorContablesByTrabajadorQuery request, CancellationToken cancellationToken)
        {
            return await _uow.Rrhh.Maestros.TrabajadorContables.Query()
                .AsNoTracking()
                .Where(e => e.TrabajadorCode == request.TrabajadorCode)
                .OrderBy(e => e.Id)
                .Select(e => Map(e))
                .ToListAsync(cancellationToken);
        }

        private static TrabajadorContableResponse Map(TrabajadorContable e) => new()
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
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
