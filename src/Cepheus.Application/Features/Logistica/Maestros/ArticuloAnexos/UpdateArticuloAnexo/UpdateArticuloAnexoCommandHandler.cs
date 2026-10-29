using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.UpdateArticuloAnexo
{
    public class UpdateArticuloAnexoCommandHandler : IRequestHandler<UpdateArticuloAnexoCommand, ArticuloAnexoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateArticuloAnexoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ArticuloAnexoResponse> Handle(
            UpdateArticuloAnexoCommand request, CancellationToken cancellationToken)
        {
            var scc = request.SubCentroCostoCode.Trim().ToUpperInvariant();
            var art = request.ArticuloCode.Trim().ToUpperInvariant();

            var entity = await _uow.Logistica.Maestros.ArticuloAnexos.Query()
                .FirstOrDefaultAsync(a => a.SubCentroCostoCode == scc && a.ArticuloCode == art, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No existe el enlace de anexo del subcentro {scc} y el artículo {art}.");
            }

            entity.AccountingAccountCode = request.AccountingAccountCode.Trim().ToUpperInvariant();
            entity.AccountingAttachmentTypeCode = ArticuloAnexoMapper.Clean(request.AccountingAttachmentTypeCode);
            entity.AccountingAttachmentCode = ArticuloAnexoMapper.Clean(request.AccountingAttachmentCode);
            entity.RowVersion = request.RowVersion;

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El enlace fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return ArticuloAnexoMapper.Map(entity);
        }
    }
}
