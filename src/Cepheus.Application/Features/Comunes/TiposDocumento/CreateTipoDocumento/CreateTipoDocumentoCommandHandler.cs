using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.TiposDocumento.Common;
using Cepheus.Domain.Comunes;
using MediatR;

namespace Cepheus.Application.Features.Comunes.TiposDocumento.CreateTipoDocumento
{
    public class CreateTipoDocumentoCommandHandler : IRequestHandler<CreateTipoDocumentoCommand, TipoDocumentoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoDocumentoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoDocumentoResponse> Handle(CreateTipoDocumentoCommand request, CancellationToken cancellationToken)
        {
            var tipoDocumento = new TipoDocumento
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                ShortName = string.IsNullOrWhiteSpace(request.ShortName) ? null : request.ShortName.Trim().ToUpperInvariant(),
                SunatCode = string.IsNullOrWhiteSpace(request.SunatCode) ? null : request.SunatCode.Trim(),
                
                IsActive = true
            };

            await _uow.Comunes.TiposDocumento.AddAsync(tipoDocumento, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoDocumento);
        }

        internal static TipoDocumentoResponse Map(TipoDocumento tipoDocumento) => new()
        {
            Code = tipoDocumento.Code,
            Name = tipoDocumento.Name,
            ShortName = tipoDocumento.ShortName,
            SunatCode = tipoDocumento.SunatCode,
            
            IsActive = tipoDocumento.IsActive,
            CreatedAt = tipoDocumento.CreatedAt,
            UpdatedAt = tipoDocumento.UpdatedAt,
            CreatedBy = tipoDocumento.CreatedBy,
            UpdatedBy = tipoDocumento.UpdatedBy,
            RowVersion = tipoDocumento.RowVersion
        };
    }
}
