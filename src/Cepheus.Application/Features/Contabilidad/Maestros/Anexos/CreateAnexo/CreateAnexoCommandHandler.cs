using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.Anexos.Common;
using Cepheus.Domain.Contabilidad.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.Anexos.CreateAnexo
{
    public class CreateAnexoCommandHandler : IRequestHandler<CreateAnexoCommand, AnexoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateAnexoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AnexoResponse> Handle(CreateAnexoCommand request, CancellationToken cancellationToken)
        {
            var tipo = request.TipoAnexoCode.Trim();

            // Correlativo de 5 dígitos dentro de cada tipo de anexo.
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Contabilidad.Maestros.Anexos.Query()
                    .Where(a => a.TipoAnexoCode == tipo)
                    .Select(a => a.Code),
                length: 5,
                entityLabel: "Anexos",
                cancellationToken);

            var entity = new Anexo
            {
                TipoAnexoCode = request.TipoAnexoCode.Trim(),
                Code = code,
                Name = string.IsNullOrWhiteSpace(request.Name) ? null : request.Name.Trim(),
                LinkAccount1Code = (request.LinkAccount1Code?.Trim() ?? string.Empty),
                LinkAccount2Code = (request.LinkAccount2Code?.Trim() ?? string.Empty),
                Flag = string.IsNullOrWhiteSpace(request.Flag) ? null : request.Flag.Trim(),
                IsActive = true
            };

            await _uow.Contabilidad.Maestros.Anexos.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entity);
        }

        internal static AnexoResponse Map(Anexo e) => new()
        {
            TipoAnexoCode = e.TipoAnexoCode.TrimEnd(),
            Code = e.Code.TrimEnd(),
            Name = e.Name?.TrimEnd(),
            LinkAccount1Code = e.LinkAccount1Code.TrimEnd(),
            LinkAccount2Code = e.LinkAccount2Code.TrimEnd(),
            Flag = e.Flag?.TrimEnd(),
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
