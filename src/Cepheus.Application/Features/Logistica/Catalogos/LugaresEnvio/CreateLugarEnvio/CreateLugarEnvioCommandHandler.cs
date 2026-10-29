using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.LugaresEnvio.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.LugaresEnvio.CreateLugarEnvio
{
    public class CreateLugarEnvioCommandHandler : IRequestHandler<CreateLugarEnvioCommand, LugarEnvioResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateLugarEnvioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<LugarEnvioResponse> Handle(CreateLugarEnvioCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Logistica.Catalogos.LugaresEnvio.Query().Select(f => f.Code), length: 3, entityLabel: "Lugares de Envío", cancellationToken);

            var lugar = new LugarEnvio
            {
                Code = code,
                Name = request.Name.Trim(),
                Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
                IsActive = true
            };

            await _uow.Logistica.Catalogos.LugaresEnvio.AddAsync(lugar, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(lugar);
        }

        internal static LugarEnvioResponse Map(LugarEnvio lugar) => new()
        {
            Code = lugar.Code,
            Name = lugar.Name,
            Address = lugar.Address,
            IsActive = lugar.IsActive,
            CreatedAt = lugar.CreatedAt,
            UpdatedAt = lugar.UpdatedAt,
            CreatedBy = lugar.CreatedBy,
            UpdatedBy = lugar.UpdatedBy,
            RowVersion = lugar.RowVersion
        };
    }
}