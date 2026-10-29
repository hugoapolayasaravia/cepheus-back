using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.GetTrabajadorContactoById
{
    public class GetTrabajadorContactoByIdQueryHandler
        : IRequestHandler<GetTrabajadorContactoByIdQuery, TrabajadorContactoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorContactoByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorContactoResponse> Handle(GetTrabajadorContactoByIdQuery request, CancellationToken cancellationToken)
        {
            var contacto = await _uow.Rrhh.Maestros.TrabajadorContactos.Query()
                .AsNoTracking()
                .Where(c => c.Id == request.Id)
                .Select(c => new TrabajadorContactoResponse
                {
                    Id = c.Id,
                    TrabajadorCode = c.TrabajadorCode,
                    Name = c.Name,
                    Phone = c.Phone,
                    ParentescoCode = c.ParentescoCode,
                    IsPrimary = c.IsPrimary,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (contacto is null)
            {
                throw new KeyNotFoundException($"Contacto {request.Id} no encontrado.");
            }

            return contacto;
        }
    }
}