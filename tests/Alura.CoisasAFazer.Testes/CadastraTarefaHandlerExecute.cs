using System;
using System.Collections.Generic;
using System.Text;
using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Services.Handlers;
using Alura.CoisasAFazer.Testes.TestDubles;
using Xunit;
using System.Linq;

namespace Alura.CoisasAFazer.Testes
{
	public class CadastraTarefaHandlerExecute
	{
		[Fact]
		public void CadastroDeTarefasValidasInsereNoDB()
		{
			//arrange
			var comando = new CadastraTarefa("Estudar xUnit", new Categoria("Estudo"), DateTime.Now);
			var fakeRepo = new RepoFakeTarefas();
			var handler = new CadastraTarefaHandler(fakeRepo);

			//act
			handler.Execute(comando);

			//assert
			var tarefa = fakeRepo.ObtemTarefa((t) => t.Titulo == "Estudar xUnit");
			Assert.NotNull(tarefa);
		}
	}
}
