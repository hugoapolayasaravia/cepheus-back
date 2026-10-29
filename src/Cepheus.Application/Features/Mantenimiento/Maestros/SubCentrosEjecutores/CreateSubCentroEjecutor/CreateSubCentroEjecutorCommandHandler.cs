using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.Common;
using Cepheus.Domain.Mantenimiento.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.CreateSubCentroEjecutor
{
    public class CreateSubCentroEjecutorCommandHandler
        : IRequestHandler<CreateSubCentroEjecutorCommand, SubCentroEjecutorResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateSubCentroEjecutorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SubCentroEjecutorResponse> Handle(
            CreateSubCentroEjecutorCommand request,
            CancellationToken cancellationToken)
        {
            var centroEjecutorCode = string.IsNullOrWhiteSpace(request.CentroEjecutorCode)
                ? null
                : request.CentroEjecutorCode.Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(centroEjecutorCode))
            {
                throw new ArgumentException(
                    "El Centro Ejecutor es obligatorio para crear un SubCentro Ejecutor.");
            }

            var code = await SequentialCodeGenerator.NextChildAsync(
                _uow.Mantenimiento.Maestros.SubCentrosEjecutores.Query().Select(s => s.Code),
                centroEjecutorCode,
                suffixLength: 2,
                entityLabel: "SubCentros Ejecutores",
                cancellationToken);

            var subCentroEjecutor = new SubCentroEjecutor
            {
                Code = code,
                Name = request.Name.Trim(),
                CentroEjecutorCode = centroEjecutorCode,
                IsActive = true
            };

            await _uow.Mantenimiento.Maestros.SubCentrosEjecutores
                .AddAsync(subCentroEjecutor, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            return Map(subCentroEjecutor);
        }

        internal static SubCentroEjecutorResponse Map(
            SubCentroEjecutor subCentroEjecutor) => new()
            {
                Code = subCentroEjecutor.Code,
                Name = subCentroEjecutor.Name,
                CentroEjecutorCode = subCentroEjecutor.CentroEjecutorCode,
                IsActive = subCentroEjecutor.IsActive,
                CreatedAt = subCentroEjecutor.CreatedAt,
                UpdatedAt = subCentroEjecutor.UpdatedAt,
                RowVersion = subCentroEjecutor.RowVersion
            };
    }
}