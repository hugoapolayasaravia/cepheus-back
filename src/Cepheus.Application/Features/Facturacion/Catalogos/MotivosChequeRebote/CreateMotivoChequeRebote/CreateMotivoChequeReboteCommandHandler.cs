using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.CreateMotivoChequeRebote
{
    public class CreateMotivoChequeReboteCommandHandler
        : IRequestHandler<CreateMotivoChequeReboteCommand, MotivoChequeReboteResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateMotivoChequeReboteCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<MotivoChequeReboteResponse> Handle(
            CreateMotivoChequeReboteCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Catalogos.MotivosChequeRebote.Query().Select(t => t.Code),
                length: 2,
                entityLabel: "Motivos de Rebote de Cheque",
                cancellationToken);

            var motivo = new MotivoChequeRebote { Code = code, Name = request.Name.Trim(), IsActive = true };

            await _uow.Facturacion.Catalogos.MotivosChequeRebote.AddAsync(motivo, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return (await MotivoChequeReboteProjection.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
