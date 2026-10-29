using Cepheus.Application.Features.Facturacion.Maestros.Tecnicos.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Tecnicos.CreateTecnico
{
    /// <summary>
    /// Habilita a un Trabajador de RRHH ya existente como Técnico de Facturación.
    /// No crea un Trabajador: solo agrega el registro de habilitación.
    /// </summary>
    public record CreateTecnicoCommand(string TrabajadorCode) : IRequest<TecnicoResponse>;
}
