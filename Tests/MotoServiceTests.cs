using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sprint.Data;
using Sprint.Dtos;
using Sprint.Models;
using Sprint.Services;


namespace Tests
{
    public class MotoServiceTests
    {
        [Fact]
        public void Create_DeveRetornarMoto_QuandoDadosValidos()
        {
            // Arrange
            var service = new MotoServiceFake();
            var dto = new MotoDTO
            {
                Placa = "ABC1234",
                Modelo = "Honda CG 160",
                NumeroChassi = "9C2KC1670GR123456",
                Status = StatusMoto.DISPONIVEL, // Use o enum correto
                PatioId = 1,
                ClienteId = 1
            };

            // Act
            var (moto, error) = service.Create(dto);

            // Assert
            Assert.Equal("ABC1234", moto.Placa);
        }


        [Fact]
        public void Create_DeveRetornarErro_QuandoNumeroChassiJaExiste()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "MotoDb_ChassiDuplicado")
                .Options;

            using var context = new AppDbContext(options);
            var patio = new Patio { Id = 1, Nome = "Patio A", Endereco = "Rua 1" };
            var cliente = new Cliente { Id = 1, Nome = "Cliente A", Email = "a@email.com", Senha = "12345678" };
            context.Patios.Add(patio);
            context.Clientes.Add(cliente);
            context.Motos.Add(new Moto
            {
                Id = 1,
                Placa = "ABC1234",
                Modelo = "Modelo X",
                NumeroChassi = "CHASSI123",
                Status = StatusMoto.DISPONIVEL,
                PatioId = patio.Id,
                ClienteId = cliente.Id
            });
            context.SaveChanges();

            var service = new MotoService(context);
            var motoDto = new MotoDTO
            {
                Placa = "DEF5678",
                Modelo = "Modelo Y",
                NumeroChassi = "CHASSI123", // Chassi duplicado
                Status = StatusMoto.DISPONIVEL,
                PatioId = patio.Id,
                ClienteId = cliente.Id
            };

            // Act
            var (moto, error) = service.Create(motoDto);

            // Assert
            Assert.Null(moto);
            Assert.Equal("ja existe uma moto com esse numero de chassi", error);
        }


        [Fact]
        public void GetById_DeveRetornarNull_QuandoMotoNaoExiste()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "MotoDb_MotoNull")
                .Options;

            using var context = new AppDbContext(options);
            var service = new MotoService(context);

            // Act
            var moto = service.GetById(999); // Id inexistente

            // Assert
            Assert.Null(moto);
        }

        [Fact]
        public void GetAll_DeveRetornarTodasAsMotos()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "MotoDb_GetAll")
                .Options;

            using var context = new AppDbContext(options);
            var patio = new Patio { Id = 1, Nome = "Patio A", Endereco = "Rua 1" };
            var cliente = new Cliente { Id = 1, Nome = "Cliente A", Email = "a@email.com", Senha = "12345678" };
            context.Patios.Add(patio);
            context.Clientes.Add(cliente);
            context.Motos.Add(new Moto
            {
                Placa = "ABC1234",
                Modelo = "Modelo X",
                NumeroChassi = "CHASSI123",
                Status = StatusMoto.DISPONIVEL,
                PatioId = patio.Id,
                ClienteId = cliente.Id
            });
            context.Motos.Add(new Moto
            {
                Placa = "DEF5678",
                Modelo = "Modelo Y",
                NumeroChassi = "CHASSI456",
                Status = StatusMoto.DISPONIVEL,
                PatioId = patio.Id,
                ClienteId = cliente.Id
            });
            context.SaveChanges();

            var service = new MotoService(context);

            var motos = service.GetAll().ToList();

            Assert.Equal(2, motos.Count);
        }

        [Fact]
        public void Update_DeveRetornarErro_QuandoIdCorpoDiferenteDaUrl()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "MotoDb_UpdateIdCorpo")
                .Options;

            using var context = new AppDbContext(options);
            var service = new MotoService(context);

            var dto = new MotoDTO
            {
                Id = 2,
                Placa = "ABC1234",
                Modelo = "Modelo X",
                NumeroChassi = "CHASSI123",
                Status = StatusMoto.DISPONIVEL,
                PatioId = 1,
                ClienteId = 1
            };

            var (moto, error) = service.Update(1, dto);

            Assert.Null(moto);
            Assert.Equal("ID do corpo não corresponde ao da URL", error);
        }

        [Fact]
        public void Update_DeveRetornarErro_QuandoMotoNaoEncontrada()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "MotoDb_UpdateNaoEncontrada")
                .Options;

            using var context = new AppDbContext(options);
            var service = new MotoService(context);

            var dto = new MotoDTO
            {
                Id = 1,
                Placa = "ABC1234",
                Modelo = "Modelo X",
                NumeroChassi = "CHASSI123",
                Status = StatusMoto.DISPONIVEL,
                PatioId = 1,
                ClienteId = 1
            };

            var (moto, error) = service.Update(1, dto);

            Assert.Null(moto);
            Assert.Equal("Moto não encontrada", error);
        }

        [Fact]
        public void Update_DeveRetornarErro_QuandoChassiJaExisteEmOutraMoto()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "MotoDb_UpdateChassiDuplicado")
                .Options;

            using var context = new AppDbContext(options);
            var patio = new Patio { Id = 1, Nome = "Patio A", Endereco = "Rua 1" };
            var cliente = new Cliente { Id = 1, Nome = "Cliente A", Email = "a@email.com", Senha = "12345678" };
            context.Patios.Add(patio);
            context.Clientes.Add(cliente);
            var moto1 = new Moto
            {
                Id = 1,
                Placa = "ABC1234",
                Modelo = "Modelo X",
                NumeroChassi = "CHASSI123",
                Status = StatusMoto.DISPONIVEL,
                PatioId = patio.Id,
                ClienteId = cliente.Id
            };
            var moto2 = new Moto
            {
                Id = 2,
                Placa = "DEF5678",
                Modelo = "Modelo Y",
                NumeroChassi = "CHASSI456",
                Status = StatusMoto.DISPONIVEL,
                PatioId = patio.Id,
                ClienteId = cliente.Id
            };
            context.Motos.Add(moto1);
            context.Motos.Add(moto2);
            context.SaveChanges();

            var service = new MotoService(context);

            var dto = new MotoDTO
            {
                Id = moto2.Id,
                Placa = "DEF5678",
                Modelo = "Modelo Y",
                NumeroChassi = "CHASSI123", // Chassi já existe em moto1
                Status = StatusMoto.DISPONIVEL,
                PatioId = patio.Id,
                ClienteId = cliente.Id
            };

            var (moto, error) = service.Update(moto2.Id, dto);

            Assert.Null(moto);
            Assert.Equal("já existe uma moto com esse número de chassi", error);
        }

        [Fact]
        public void Delete_DeveRetornarFalso_QuandoMotoNaoExiste()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "MotoDb_DeleteNaoExiste")
                .Options;

            using var context = new AppDbContext(options);
            var service = new MotoService(context);

            var result = service.Delete(999);

            Assert.False(result);
        }

        [Fact]
        public void Delete_DeveExcluirMoto_QuandoExiste()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "MotoDb_DeleteExiste")
                .Options;

            using var context = new AppDbContext(options);
            var patio = new Patio { Id = 1, Nome = "Patio A", Endereco = "Rua 1" };
            var cliente = new Cliente { Id = 1, Nome = "Cliente A", Email = "a@email.com", Senha = "12345678" };
            context.Patios.Add(patio);
            context.Clientes.Add(cliente);
            var moto = new Moto
            {
                Id = 1,
                Placa = "ABC1234",
                Modelo = "Modelo X",
                NumeroChassi = "CHASSI123",
                Status = StatusMoto.DISPONIVEL,
                PatioId = patio.Id,
                ClienteId = cliente.Id
            };
            context.Motos.Add(moto);
            context.SaveChanges();

            var service = new MotoService(context);

            var result = service.Delete(moto.Id);

            Assert.True(result);
            Assert.Null(context.Motos.Find(moto.Id));
        }

    }







    // Implementação fake para teste unitário
    public class MotoServiceFake : IMotoService
    {
        public (Moto, string) Create(MotoDTO dto)
        {
            return (new Moto
            {
                Id = 1,
                Placa = dto.Placa,
                Modelo = dto.Modelo,
                NumeroChassi = dto.NumeroChassi,
                Status = dto.Status,
                PatioId = dto.PatioId ?? 0,
                ClienteId = dto.ClienteId ?? 0
            }, null);
        }

        public IEnumerable<Moto> GetAll() => throw new NotImplementedException();
        public Moto GetById(long id) => throw new NotImplementedException();
        public (Moto moto, string error) Update(long id, MotoDTO motoDto) => throw new NotImplementedException();
        public bool Delete(long id) => throw new NotImplementedException();
    }
}
