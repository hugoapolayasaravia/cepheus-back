using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.CreateArticuloAnexo
{
    public class CreateArticuloAnexoCommandHandler : IRequestHandler<CreateArticuloAnexoCommand, ArticuloAnexoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateArticuloAnexoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ArticuloAnexoResponse> Handle(
            CreateArticuloAnexoCommand request, CancellationToken cancellationToken)
        {
            var entity = new ArticuloAnexo
            {
                SubCentroCostoCode = request.SubCentroCostoCode.Trim().ToUpperInvariant(),
                ArticuloCode = request.ArticuloCode.Trim().ToUpperInvariant(),
                AccountingAccountCode = request.AccountingAccountCode.Trim().ToUpperInvariant(),
                AccountingAttachmentTypeCode = ArticuloAnexoMapper.Clean(request.AccountingAttachmentTypeCode),
                AccountingAttachmentCode = ArticuloAnexoMapper.Clean(request.AccountingAttachmentCode)
            };

            await _uow.Logistica.Maestros.ArticuloAnexos.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return ArticuloAnexoMapper.Map(entity);
        }
    }
}
