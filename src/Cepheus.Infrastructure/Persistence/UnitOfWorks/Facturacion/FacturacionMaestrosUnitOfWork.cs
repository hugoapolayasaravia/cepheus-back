using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Maestros;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Cepheus.Infrastructure.Persistence.Repositories;

namespace Cepheus.Infrastructure.Persistence.UnitOfWorks.Facturacion
{
    public sealed class FacturacionMaestrosUnitOfWork
        : IFacturacionMaestrosUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public FacturacionMaestrosUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }


        private IRepository<TransportistaVenta>? _transportistas;

        public IRepository<TransportistaVenta> Transportistas => _transportistas ??= new Repository<TransportistaVenta>(_context);

        private IRepository<ChoferVenta>? _choferes;

        public IRepository<ChoferVenta> Choferes =>  _choferes ??= new Repository<ChoferVenta>(_context);

        private IRepository<VehiculoVenta>? _vehiculos;

        public IRepository<VehiculoVenta> Vehiculos => _vehiculos ??= new Repository<VehiculoVenta>(_context);

        private IRepository<Cliente>? _clientes;
        public IRepository<Cliente> Clientes => _clientes ??= new Repository<Cliente>(_context);

        private IRepository<Obra>? _obras;
        public IRepository<Obra> Obras => _obras ??= new Repository<Obra>(_context);

        private IRepository<Vendedor>? _vendedores;
        public IRepository<Vendedor> Vendedores => _vendedores ??= new Repository<Vendedor>(_context);

        private IRepository<Cobrador>? _cobradores;
        public IRepository<Cobrador> Cobradores => _cobradores ??= new Repository<Cobrador>(_context);

        private IRepository<Producto>? _productos;
        public IRepository<Producto> Productos => _productos ??= new Repository<Producto>(_context);

        private IRepository<Tecnico>? _tecnicos;
        public IRepository<Tecnico> Tecnicos => _tecnicos ??= new Repository<Tecnico>(_context);

        private IRepository<UsuarioSolicitudNotaCredito>? _usuariosSolicitudNotaCredito;
        public IRepository<UsuarioSolicitudNotaCredito> UsuariosSolicitudNotaCredito => _usuariosSolicitudNotaCredito ??= new Repository<UsuarioSolicitudNotaCredito>(_context);
    }
}