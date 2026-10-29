using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Tecnicos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Tecnicos.GetTecnicoByCode
{
    public class GetTecnicoByCodeQueryHandler : IRequestHandler<GetTecnicoByCodeQuery, TecnicoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTecnicoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TecnicoResponse> Handle(GetTecnicoByCodeQuery request, CancellationToken cancellationToken)
        {
            var tecnico = await _uow.Facturacion.Maestros.Tecnicos.Query()
                .AsNoTracking()
                .Where(t => t.TrabajadorCode == request.TrabajadorCode)
                .Select(t => new TecnicoResponse
                {
                    TrabajadorCode = t.TrabajadorCode,
                    TrabajadorNombre = t.Trabajador.FirstNames + " " + t.Trabajador.PaternalSurname,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tecnico is null)
            {
                throw new KeyNotFoundException($"Técnico {request.TrabajadorCode} no encontrado.");
            }

            return tecnico;
        }
    }
}
