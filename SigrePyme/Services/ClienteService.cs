using SigrePyme.Models;
using SigrePyme.Repositories;
using SigrePyme.Services;

namespace SigrePyme.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepo;

        public ClienteService(IClienteRepository clienteRepo)
        {
            _clienteRepo = clienteRepo;
        }

        public async Task<IEnumerable<Cliente>> ObtenerTodosAsync()
            => await _clienteRepo.ObtenerTodosAsync();

        public async Task<Cliente?> ObtenerPorIdAsync(int id)
            => await _clienteRepo.ObtenerPorIdAsync(id);

        public async Task<(bool Exito, string Mensaje)> CrearAsync(ClienteViewModel vm)
        {
            var existente = await _clienteRepo.ObtenerPorEmailAsync(vm.Email.Trim());
            if (existente != null)
                return (false, $"Ya existe un cliente registrado con el correo '{vm.Email}'.");

            var cliente = new Cliente
            {
                NombreCompleto = vm.NombreCompleto.Trim(),
                Email = vm.Email.Trim().ToLower(),
                Telefono = vm.Telefono?.Trim(),
                Direccion = vm.Direccion?.Trim(),
                Empresa = vm.Empresa?.Trim(),
                Activo = true,
                FechaRegistro = DateTime.Now
            };

            await _clienteRepo.AgregarAsync(cliente);
            await _clienteRepo.GuardarCambiosAsync();

            return (true, $"Cliente '{cliente.NombreCompleto}' registrado correctamente.");
        }

        public async Task<(bool Exito, string Mensaje)> EditarAsync(ClienteViewModel vm)
        {
            var cliente = await _clienteRepo.ObtenerPorIdAsync(vm.Id);
            if (cliente == null)
                return (false, "Cliente no encontrado.");

            var otro = await _clienteRepo.ObtenerPorEmailAsync(vm.Email.Trim());
            if (otro != null && otro.Id != vm.Id)
                return (false, $"El correo '{vm.Email}' ya está en uso por otro cliente.");

            cliente.NombreCompleto = vm.NombreCompleto.Trim();
            cliente.Email = vm.Email.Trim().ToLower();
            cliente.Telefono = vm.Telefono?.Trim();
            cliente.Direccion = vm.Direccion?.Trim();
            cliente.Empresa = vm.Empresa?.Trim();
            cliente.Activo = vm.Activo;

            _clienteRepo.Actualizar(cliente);
            await _clienteRepo.GuardarCambiosAsync();

            return (true, $"Cliente '{cliente.NombreCompleto}' actualizado correctamente.");
        }

        public async Task<(bool Exito, string Mensaje)> EliminarAsync(int id)
        {
            var cliente = await _clienteRepo.ObtenerPorIdAsync(id);
            if (cliente == null)
                return (false, "Cliente no encontrado.");

            _clienteRepo.Eliminar(cliente);
            await _clienteRepo.GuardarCambiosAsync();

            return (true, $"Cliente '{cliente.NombreCompleto}' desactivado correctamente.");
        }
    }
}