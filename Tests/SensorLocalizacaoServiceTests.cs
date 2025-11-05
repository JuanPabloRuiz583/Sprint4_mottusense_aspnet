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
    public class SensorLocalizacaoServiceTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private SensorLocalizacaoService GetService(AppDbContext context)
        {
            return new SensorLocalizacaoService(context);
        }

        private Moto CriarMoto(AppDbContext context)
        {
            var moto = new Moto
            {
                Placa = "ABC1234",
                Modelo = "CG 160",
                NumeroChassi = "CHASSI12345678901",
                Status = StatusMoto.DISPONIVEL,
                PatioId = 1,
                ClienteId = 1
            };
            context.Motos.Add(moto);
            context.SaveChanges();
            return moto;
        }

        [Fact]
        public void DeveRetornarTodosOsSensores()
        {
            var context = GetDbContext();
            var moto = CriarMoto(context);

            context.Sensores.Add(new SensorLocalizacao
            {
                Latitude = -23.5,
                Longitude = -46.6,
                TimeDaLocalizacao = DateTime.Now,
                MotoId = moto.Id
            });
            context.Sensores.Add(new SensorLocalizacao
            {
                Latitude = -22.9,
                Longitude = -43.2,
                TimeDaLocalizacao = DateTime.Now,
                MotoId = moto.Id
            });
            context.SaveChanges();

            var service = GetService(context);

            var sensores = service.GetAll().ToList();

            Assert.Equal(2, sensores.Count);
        }

        [Fact]
        public void DeveRetornarSensorPorIdQuandoExiste()
        {
            var context = GetDbContext();
            var moto = CriarMoto(context);

            var sensor = new SensorLocalizacao
            {
                Latitude = -23.5,
                Longitude = -46.6,
                TimeDaLocalizacao = DateTime.Now,
                MotoId = moto.Id
            };
            context.Sensores.Add(sensor);
            context.SaveChanges();

            var service = GetService(context);

            var resultado = service.GetById(sensor.Id);

            Assert.NotNull(resultado);
            Assert.Equal(sensor.Latitude, resultado.Latitude);
        }

        [Fact]
        public void DeveRetornarNuloQuandoSensorNaoExistePorId()
        {
            var context = GetDbContext();
            var service = GetService(context);

            var resultado = service.GetById(999);

            Assert.Null(resultado);
        }

        [Fact]
        public void DeveRetornarErroAoCriarSensorComMotoIdInvalido()
        {
            var context = GetDbContext();
            var service = GetService(context);

            var dto = new SensorLocalizacaoDTO
            {
                Latitude = -23.5,
                Longitude = -46.6,
                TimeDaLocalizacao = DateTime.Now,
                MotoId = 999
            };

            var (sensor, erro) = service.Create(dto);

            Assert.Null(sensor);
            Assert.Equal("id invalido. o id da moto nao existe", erro);
        }

        [Fact]
        public void DeveCriarSensorQuandoDadosSaoValidos()
        {
            var context = GetDbContext();
            var moto = CriarMoto(context);
            var service = GetService(context);

            var dto = new SensorLocalizacaoDTO
            {
                Latitude = -23.5,
                Longitude = -46.6,
                TimeDaLocalizacao = DateTime.Now,
                MotoId = moto.Id
            };

            var (sensor, erro) = service.Create(dto);

            Assert.NotNull(sensor);
            Assert.Null(erro);
            Assert.Equal(dto.Latitude, sensor.Latitude);
        }

        [Fact]
        public void DeveRetornarErroQuandoIdDoCorpoDiferenteDaUrl()
        {
            var context = GetDbContext();
            var moto = CriarMoto(context);
            var service = GetService(context);

            var dto = new SensorLocalizacaoDTO
            {
                Id = 2,
                Latitude = -23.5,
                Longitude = -46.6,
                TimeDaLocalizacao = DateTime.Now,
                MotoId = moto.Id
            };

            var (sensor, erro) = service.Update(1, dto);

            Assert.Null(sensor);
            Assert.Equal("ID do corpo não corresponde ao da URL", erro);
        }

        [Fact]
        public void DeveRetornarErroQuandoSensorNaoEncontradoParaAtualizar()
        {
            var context = GetDbContext();
            var moto = CriarMoto(context);
            var service = GetService(context);

            var dto = new SensorLocalizacaoDTO
            {
                Id = 1,
                Latitude = -23.5,
                Longitude = -46.6,
                TimeDaLocalizacao = DateTime.Now,
                MotoId = moto.Id
            };

            var (sensor, erro) = service.Update(1, dto);

            Assert.Null(sensor);
            Assert.Equal("Sensor de localização não encontrado", erro);
        }

        [Fact]
        public void DeveRetornarErroAoAtualizarSensorComMotoIdInvalido()
        {
            var context = GetDbContext();
            var moto = CriarMoto(context);
            var sensor = new SensorLocalizacao
            {
                Latitude = -23.5,
                Longitude = -46.6,
                TimeDaLocalizacao = DateTime.Now,
                MotoId = moto.Id
            };
            context.Sensores.Add(sensor);
            context.SaveChanges();

            var service = GetService(context);

            var dto = new SensorLocalizacaoDTO
            {
                Id = sensor.Id,
                Latitude = -22.9,
                Longitude = -43.2,
                TimeDaLocalizacao = DateTime.Now,
                MotoId = 999 // MotoId inválido
            };

            var (sensorAtualizado, erro) = service.Update(sensor.Id, dto);

            Assert.Null(sensorAtualizado);
            Assert.Equal("id invalido. o id da moto nao existe", erro);
        }

        [Fact]
        public void DeveAtualizarSensorQuandoDadosSaoValidos()
        {
            var context = GetDbContext();
            var moto = CriarMoto(context);
            var sensor = new SensorLocalizacao
            {
                Latitude = -23.5,
                Longitude = -46.6,
                TimeDaLocalizacao = DateTime.Now,
                MotoId = moto.Id
            };
            context.Sensores.Add(sensor);
            context.SaveChanges();

            var service = GetService(context);

            var dto = new SensorLocalizacaoDTO
            {
                Id = sensor.Id,
                Latitude = -22.9,
                Longitude = -43.2,
                TimeDaLocalizacao = DateTime.Now,
                MotoId = moto.Id
            };

            var (sensorAtualizado, erro) = service.Update(sensor.Id, dto);

            Assert.NotNull(sensorAtualizado);
            Assert.Null(erro);
            Assert.Equal(dto.Latitude, sensorAtualizado.Latitude);
            Assert.Equal(dto.Longitude, sensorAtualizado.Longitude);
        }

        [Fact]
        public void DeveRetornarFalsoAoTentarExcluirSensorInexistente()
        {
            var context = GetDbContext();
            var service = GetService(context);

            var resultado = service.Delete(999);

            Assert.False(resultado);
        }

        [Fact]
        public void DeveExcluirSensorQuandoExiste()
        {
            var context = GetDbContext();
            var moto = CriarMoto(context);
            var sensor = new SensorLocalizacao
            {
                Latitude = -23.5,
                Longitude = -46.6,
                TimeDaLocalizacao = DateTime.Now,
                MotoId = moto.Id
            };
            context.Sensores.Add(sensor);
            context.SaveChanges();

            var service = GetService(context);

            var resultado = service.Delete(sensor.Id);

            Assert.True(resultado);
            Assert.Null(context.Sensores.Find(sensor.Id));
        }
    }
}
