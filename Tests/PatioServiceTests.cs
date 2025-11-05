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
    public class PatioServiceTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private PatioService GetService(AppDbContext context)
        {
            return new PatioService(context);
        }

        [Fact]
        public void DeveRetornarTodosOsPatios()
        {
            var context = GetDbContext();
            context.Patios.Add(new Patio { Nome = "Pátio A", Endereco = "Rua 1" });
            context.Patios.Add(new Patio { Nome = "Pátio B", Endereco = "Rua 2" });
            context.SaveChanges();

            var service = GetService(context);

            var patios = service.GetAll().ToList();

            Assert.Equal(2, patios.Count);
            Assert.Contains(patios, p => p.Nome == "Pátio A");
            Assert.Contains(patios, p => p.Nome == "Pátio B");
        }

        [Fact]
        public void DeveRetornarPatioPorIdQuandoExiste()
        {
            var context = GetDbContext();
            var patio = new Patio { Nome = "Pátio Central", Endereco = "Av. Principal" };
            context.Patios.Add(patio);
            context.SaveChanges();

            var service = GetService(context);

            var resultado = service.GetById(patio.Id);

            Assert.NotNull(resultado);
            Assert.Equal("Pátio Central", resultado.Nome);
        }

        [Fact]
        public void DeveRetornarNuloQuandoPatioNaoExistePorId()
        {
            var context = GetDbContext();
            var service = GetService(context);

            var resultado = service.GetById(999);

            Assert.Null(resultado);
        }

        [Fact]
        public void DeveRetornarErroQuandoNomeEhObrigatorio()
        {
            var context = GetDbContext();
            var service = GetService(context);

            var dto = new PatioDTO { Nome = "", Endereco = "Rua X" };
            var (patio, erro) = service.Create(dto);

            Assert.Null(patio);
            Assert.Equal("Nome é obrigatório", erro);
        }

        [Fact]
        public void DeveCriarPatioQuandoDadosSaoValidos()
        {
            var context = GetDbContext();
            var service = GetService(context);

            var dto = new PatioDTO { Nome = "Novo Pátio", Endereco = "Rua Nova" };
            var (patio, erro) = service.Create(dto);

            Assert.NotNull(patio);
            Assert.Null(erro);
            Assert.Equal("Novo Pátio", patio.Nome);
        }

        [Fact]
        public void DeveRetornarErroQuandoIdDoCorpoDiferenteDaUrl()
        {
            var context = GetDbContext();
            var service = GetService(context);

            var dto = new PatioDTO { Id = 2, Nome = "Alterado", Endereco = "Rua Alterada" };
            var (patio, erro) = service.Update(1, dto);

            Assert.Null(patio);
            Assert.Equal("ID do corpo não corresponde ao da URL", erro);
        }

        [Fact]
        public void DeveRetornarErroQuandoPatioNaoEncontradoParaAtualizar()
        {
            var context = GetDbContext();
            var service = GetService(context);

            var dto = new PatioDTO { Id = 1, Nome = "Alterado", Endereco = "Rua Alterada" };
            var (patio, erro) = service.Update(1, dto);

            Assert.Null(patio);
            Assert.Equal("Pátio não encontrado", erro);
        }

        [Fact]
        public void DeveAtualizarPatioQuandoDadosSaoValidos()
        {
            var context = GetDbContext();
            var patio = new Patio { Nome = "Pátio Antigo", Endereco = "Rua Velha" };
            context.Patios.Add(patio);
            context.SaveChanges();

            var service = GetService(context);

            var dto = new PatioDTO { Id = patio.Id, Nome = "Pátio Atualizado", Endereco = "Rua Atualizada" };
            var (patioAtualizado, erro) = service.Update(patio.Id, dto);

            Assert.NotNull(patioAtualizado);
            Assert.Null(erro);
            Assert.Equal("Pátio Atualizado", patioAtualizado.Nome);
            Assert.Equal("Rua Atualizada", patioAtualizado.Endereco);
        }

        [Fact]
        public void DeveRetornarFalsoAoTentarExcluirPatioInexistente()
        {
            var context = GetDbContext();
            var service = GetService(context);

            var resultado = service.Delete(999);

            Assert.False(resultado);
        }

        [Fact]
        public void DeveExcluirPatioQuandoExiste()
        {
            var context = GetDbContext();
            var patio = new Patio { Nome = "Pátio Excluir", Endereco = "Rua Excluir" };
            context.Patios.Add(patio);
            context.SaveChanges();

            var service = GetService(context);

            var resultado = service.Delete(patio.Id);

            Assert.True(resultado);
            Assert.Null(context.Patios.Find(patio.Id));
        }
    }
}
