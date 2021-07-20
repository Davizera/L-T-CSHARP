using System;
using System.Collections.Generic;
using System.Text;
using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Services.Handlers;
using Xunit;

namespace Alura.CoisasAFazer.Testes
{
	public class CadastraTarefaHandlerExecute
	{
		[Fact]
		public void CadastroDeTarefasValidasInsereNoDB()
		{
			//arrange
			var comando = new CadastraTarefa("Estudar xUnit", new Categoria("Estudo"), DateTime.Now);
			var handler = new Services.Handlers.CadastraTarefaHandler();

			//act
			handler.Execute(comando);

			//assert
			Assert.True(true);
		}

		[Fact]
		public void PassingTest()
		{
			Assert.Equal(4, Add(2, 2));
		}

		[Fact]
		public void FailingTest()
		{
			Assert.Equal(5, Add(2, 2));
		}

		public int Add(int x, int y)
		{
			return x + y;
		}
	}
}
