using Microsoft.EntityFrameworkCore;
using Sprint.Data;
using Sprint.Dtos;
using Sprint.Models;
using Sprint.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class ClienteServiceTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "ClienteServiceTestDb")
                .Options;
            return new AppDbContext(options);
        }

        private ClienteService GetService(AppDbContext context)
        {
            return new ClienteService(context);
        }

        [Fact]
        public void DeveRetornarTodosOsClientes()
        {
            var context = GetDbContext();
            context.Clientes.Add(new Cliente { Nome = "João", Email = "joao@email.com", Senha = "12345678" });
            context.Clientes.Add(new Cliente { Nome = "Maria", Email = "maria@email.com", Senha = "87654321" });
            context.SaveChanges();

            var service = GetService(context);

            var clientes = service.GetAll().ToList();

            Assert.Equal(2, clientes.Count);
            Assert.Contains(clientes, c => c.Nome == "João");
            Assert.Contains(clientes, c => c.Nome == "Maria");
        }

        [Fact]
        public void DeveRetornarClientePorIdQuandoExiste()
        {
            var context = GetDbContext();
            var cliente = new Cliente { Nome = "Carlos", Email = "carlos@email.com", Senha = "senha123" };
            context.Clientes.Add(cliente);
            context.SaveChanges();

            var service = GetService(context);

            var result = service.GetById(cliente.Id);

            Assert.NotNull(result);
            Assert.Equal("Carlos", result.Nome);
        }

        [Fact]
        public void DeveRetornarNuloQuandoClienteNaoExistePorId()
        {
            var context = GetDbContext();
            var service = GetService(context);

            var result = service.GetById(999);

            Assert.Null(result);
        }

        [Fact]
        public void DeveRetornarErroQuandoEmailEhObrigatorio()
        {
            var context = GetDbContext();
            var service = GetService(context);

            var dto = new ClienteDTO { Nome = "Teste", Email = "", Senha = "12345678" };
            var (cliente, error) = service.Create(dto);

            Assert.Null(cliente);
            Assert.Equal("Email é obrigatório", error);
        }

        [Fact]
        public void DeveRetornarErroQuandoEmailJaExiste()
        {
            var context = GetDbContext();
            context.Clientes.Add(new Cliente { Nome = "Ana", Email = "ana@email.com", Senha = "senha123" });
            context.SaveChanges();

            var service = GetService(context);

            var dto = new ClienteDTO { Nome = "Ana2", Email = "ana@email.com", Senha = "senha456" };
            var (cliente, error) = service.Create(dto);

            Assert.Null(cliente);
            Assert.Equal("um cliente com esse email ja existe", error);
        }

        [Fact]
        public void DeveCriarClienteQuandoDadosSaoValidos()
        {
            var context = GetDbContext();
            var service = GetService(context);

            var dto = new ClienteDTO { Nome = "Pedro", Email = "pedro@email.com", Senha = "senha123" };
            var (cliente, error) = service.Create(dto);

            Assert.NotNull(cliente);
            Assert.Null(error);
            Assert.Equal("Pedro", cliente.Nome);
        }

        [Fact]
        public void DeveRetornarErroQuandoIdDoCorpoDiferenteDaUrl()
        {
            var context = GetDbContext();
            var service = GetService(context);

            var dto = new ClienteDTO { Id = 2, Nome = "Novo", Email = "novo@email.com", Senha = "senha123" };
            var (cliente, error) = service.Update(1, dto);

            Assert.Null(cliente);
            Assert.Equal("ID do corpo não corresponde ao da URL", error);
        }

        

        [Fact]
        public void DeveAtualizarClienteQuandoDadosSaoValidos()
        {
            var context = GetDbContext();
            var cliente = new Cliente { Nome = "Lucas", Email = "lucas@email.com", Senha = "senha123" };
            context.Clientes.Add(cliente);
            context.SaveChanges();

            var service = GetService(context);

            var dto = new ClienteDTO { Id = cliente.Id, Nome = "Lucas Atualizado", Email = "lucas@email.com", Senha = "novaSenha123" };
            var (updatedCliente, error) = service.Update(cliente.Id, dto);

            Assert.NotNull(updatedCliente);
            Assert.Null(error);
            Assert.Equal("Lucas Atualizado", updatedCliente.Nome);
            Assert.Equal("novaSenha123", updatedCliente.Senha);
        }

        [Fact]
        public void DeveRetornarFalsoAoTentarExcluirClienteInexistente()
        {
            var context = GetDbContext();
            var service = GetService(context);

            var result = service.Delete(999);

            Assert.False(result);
        }

        [Fact]
        public void DeveExcluirClienteQuandoExiste()
        {
            var context = GetDbContext();
            var cliente = new Cliente { Nome = "Paulo", Email = "paulo@email.com", Senha = "senha123" };
            context.Clientes.Add(cliente);
            context.SaveChanges();

            var service = GetService(context);

            var result = service.Delete(cliente.Id);

            Assert.True(result);
            Assert.Null(context.Clientes.Find(cliente.Id));
        }

        [Fact]
        public void DeveAutenticarClienteComCredenciaisCorretas()
        {
            var context = GetDbContext();
            var cliente = new Cliente { Nome = "Rafa", Email = "rafa@email.com", Senha = "senha123" };
            context.Clientes.Add(cliente);
            context.SaveChanges();

            var service = GetService(context);

            var result = service.Authenticate("rafa@email.com", "senha123");

            Assert.NotNull(result);
            Assert.Equal("Rafa", result.Nome);
        }

        [Fact]
        public void DeveRetornarNuloAoAutenticarComCredenciaisIncorretas()
        {
            var context = GetDbContext();
            var cliente = new Cliente { Nome = "Rafa", Email = "rafa@email.com", Senha = "senha123" };
            context.Clientes.Add(cliente);
            context.SaveChanges();

            var service = GetService(context);

            var result = service.Authenticate("rafa@email.com", "senhaErrada");

            Assert.Null(result);
        }
    }
}
