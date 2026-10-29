using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.Compradores.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Compradores.CreateComprador
{
    public class CreateCompradorCommandHandler : IRequestHandler<CreateCompradorCommand, CompradorResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateCompradorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CompradorResponse> Handle(CreateCompradorCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Logistica.Catalogos.Compradores.Query().Select(f => f.Code), length: 3, entityLabel: "Compradores", cancellationToken);


            var comprador = new Comprador
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Logistica.Catalogos.Compradores.AddAsync(comprador, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(comprador);
        }

        internal static CompradorResponse Map(Comprador comprador) => new()
        {
            Code = comprador.Code,
            Name = comprador.Name,
            IsActive = comprador.IsActive,
            CreatedAt = comprador.CreatedAt,
            UpdatedAt = comprador.UpdatedAt,
            CreatedBy = comprador.CreatedBy,
            UpdatedBy = comprador.UpdatedBy,
            RowVersion = comprador.RowVersion
        };
    }
}