using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.GetGradoInstruccionByCode
{
    public class GetGradoInstruccionByCodeQueryHandler : IRequestHandler<GetGradoInstruccionByCodeQuery, GradoInstruccionResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetGradoInstruccionByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<GradoInstruccionResponse> Handle(GetGradoInstruccionByCodeQuery request, CancellationToken cancellationToken)
        {
            var gradoInstruccion = await _uow.Rrhh.Catalogos.GradosInstruccion.Query()
                .AsNoTracking()
                .Where(g => g.Code == request.Code)
                .Select(g => new GradoInstruccionResponse
                {
                    Code = g.Code,
                    Name = g.Name,
                    IsActive = g.IsActive,
                    CreatedAt = g.CreatedAt,
                    UpdatedAt = g.UpdatedAt,
                    RowVersion = g.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (gradoInstruccion is null)
            {
                throw new KeyNotFoundException($"Grado de instrucción {request.Code} no encontrado.");
            }

            return gradoInstruccion;
        }
    }
}