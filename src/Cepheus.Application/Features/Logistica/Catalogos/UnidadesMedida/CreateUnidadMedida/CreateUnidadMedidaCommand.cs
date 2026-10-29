using Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.Common;
using MediatR;

public record CreateUnidadMedidaCommand(
    string Code,
    string Name,
    string IndDecimal 
) : IRequest<UnidadMedidaResponse>;