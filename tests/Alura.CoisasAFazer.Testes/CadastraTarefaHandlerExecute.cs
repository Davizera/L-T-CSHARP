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

namespace Alura.CoisasAFazer.Testes
{
	public class CadastraTarefaHandlerExecute
	{
		[Fact]
		public void CadastroDeTarefasValidasInsereNoDB()
		{
			//arrange
			var comando = new CadastraTarefa("Estudar xUnit", new Categoria("Estudo"), DateTime.Now);
			
			var options = new DbContextOptionsBuilder<DbTarefasContext>()
				.UseInMemoryDatabase("DbTarefas")
				.Options;
			var context = new DbTarefasContext(options);
			var repo = new RepositorioTarefa(context);
			
			var handler = new CadastraTarefaHandler(repo);

			//act
			handler.Execute(comando);

			//assert
			var tarefa = repo.ObtemTarefa((t) => t.Titulo == "Estudar xUnit");
			Assert.NotNull(tarefa);
		}
	}
}
