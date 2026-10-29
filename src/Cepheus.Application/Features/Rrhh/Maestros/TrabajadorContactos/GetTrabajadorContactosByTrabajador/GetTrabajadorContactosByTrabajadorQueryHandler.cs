using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.GetTrabajadorContactosByTrabajador
{
    public class GetTrabajadorContactosByTrabajadorQueryHandler
        : IRequestHandler<GetTrabajadorContactosByTrabajadorQuery, List<TrabajadorContactoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorContactosByTrabajadorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<TrabajadorContactoResponse>> Handle(
            GetTrabajadorContactosByTrabajadorQuery request, CancellationToken cancellationToken)
        {
            var trabajadorCode = request.TrabajadorCode.Trim().ToUpperInvariant();

            return await _uow.Rrhh.Maestros.TrabajadorContactos.Query()
                .AsNoTracking()
                .Where(c => c.TrabajadorCode == trabajadorCode)
                .OrderByDescending(c => c.IsPrimary)
                .ThenBy(c => c.Id)
                .Select(c => new TrabajadorContactoResponse
                {
                    Id = c.Id,
                    TrabajadorCode = c.TrabajadorCode,
                    Name = c.Name,
                    Phone = c.Phone,
                    ParentescoCode = c.ParentescoCode,
                    IsPrimary = c.IsPrimary,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    CreatedBy = c.CreatedBy,
                    UpdatedBy = c.UpdatedBy,
                })
                .ToListAsync(cancellationToken);
        }
    }
}