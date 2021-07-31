using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Services.Handlers;
using Alura.CoisasAFazer.Infrastructure;

namespace Alura.CoisasAFazer.Testes
{
	public class ObtemCategoriaPorIdHandlerExecute
	{
		[Fact]
		public void SeIdExisteObtemCategoriaPorIdDeveSerExecutado()
		{
			var idCategoria = 4;
			var mock = new Mock<IRepositorioTarefas>();
			var repo = mock.Object;
			var comando = new ObtemCategoriaPorId(idCategoria);
			var handler = new ObtemCategoriaPorIdHandler(repo);

			var categoria = handler.Execute(comando);

			mock.Verify(r => r.ObtemCategoriaPorId(idCategoria), Times.Once());
		}
	}
}
