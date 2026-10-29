

using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Application.Comun.Interfaces.UnitOfWork
{
    public interface IFacturacionMaestrosUnitOfWork
    {

        IRepository<TransportistaVenta> Transportistas { get; }
        IRepository<ChoferVenta> Choferes { get; }
        IRepository<VehiculoVenta> Vehiculos { get; }

        IRepository<Cliente> Clientes { get; }
        IRepository<Obra> Obras { get; }

        IRepository<Vendedor> Vendedores { get; }
        IRepository<Cobrador> Cobradores { get; }

        IRepository<Producto> Productos { get; }

        IRepository<Tecnico> Tecnicos { get; }
        IRepository<UsuarioSolicitudNotaCredito> UsuariosSolicitudNotaCredito { get; }

    }
}