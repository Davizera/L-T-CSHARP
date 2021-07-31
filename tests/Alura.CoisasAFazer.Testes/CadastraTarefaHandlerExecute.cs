using System;
using System.Collections.Generic;
using System.Text;
using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Services.Handlers;
using Alura.CoisasAFazer.Testes.TestDubles;
using Xunit;
using System.Linq;
using Alura.CoisasAFazer.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace Alura.CoisasAFazer.Testes
{
	public class CadastraTarefaHandlerExecute
	{
		[Fact]
		public void CadastroDeTarefasValidasInsereNoDB()
		{
			//arrange
			var comando = new CadastraTarefa("Estudar xUnit", new Categoria("Estudo"), DateTime.Now);

			var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

			var options = new DbContextOptionsBuilder<DbTarefasContext>()
				.UseInMemoryDatabase("DbTarefas")
				.Options;
			var context = new DbTarefasContext(options);
			var repo = new RepositorioTarefa(context);

			var handler = new CadastraTarefaHandler(repo, mockLogger.Object);

			//act
			handler.Execute(comando);

			//assert
			var tarefa = repo.ObtemTarefa((t) => t.Titulo == "Estudar xUnit");
			Assert.NotNull(tarefa);
		}

		[Fact]
		public void QuandoExceptionForLancadaIsSuccessDeveSerFalse()
		{

			//arrange
			var comando = new CadastraTarefa("Estudar xUnit", new Categoria("Estudo"), DateTime.Now);

			var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

			var mock = new Mock<IRepositorioTarefas>();
			mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))
				.Throws(new Exception("Houve um erro ao tentar incluir tarefa(s)"));
			var repo = mock.Object;

			var handler = new CadastraTarefaHandler(repo, mockLogger.Object);

			//act
			var resultado = handler.Execute(comando);

			//assert
			Assert.False(resultado.IsSuccess);
		}

		[Fact]
		public void VerificaLogQuandoExcecaoLancada()
		{

			//arrange
			var msgErro = "Houve um erro ao tentar incluir tarefa(s)";
			var exceptionEsperada = new Exception(msgErro);

			var comando = new CadastraTarefa("Estudar xUnit", new Categoria("Estudo"), DateTime.Now);

			var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

			var mock = new Mock<IRepositorioTarefas>();
			mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))
				.Throws(exceptionEsperada);
			var repo = mock.Object;

			var handler = new CadastraTarefaHandler(repo, mockLogger.Object);

			//act
			var resultado = handler.Execute(comando);

			//assert
			mockLogger.Verify(l => 
				l.Log(
						LogLevel.Error,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => true),
						exceptionEsperada,
						It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
				Times.Once());
			//TODO: pesquisar o pq do It.Is...
		}
	}
}
