using System;
using System.Collections.Generic;
using System.Linq;
using Alura.CoisasAFazer.Core.Models;

namespace Alura.CoisasAFazer.Infrastructure
{
	public class RepositorioTarefa : IRepositorioTarefas
	{
		DbTarefasContext _ctx;

		public RepositorioTarefa(DbTarefasContext dbTarefasContext)
		{
			_ctx = dbTarefasContext;
		}

		public void AtualizarTarefas(params Tarefa[] tarefas)
		{
			_ctx.Tarefas.UpdateRange(tarefas);
			_ctx.SaveChanges();
		}

		public void ExcluirTarefas(params Tarefa[] tarefas)
		{
			_ctx.Tarefas.RemoveRange(tarefas);
			_ctx.SaveChanges();
		}

		public void IncluirTarefas(params Tarefa[] tarefas)
		{
			_ctx.Tarefas.AddRange(tarefas);
			_ctx.SaveChanges();
		}

		public Categoria ObtemCategoriaPorId(int id)
		{
			return _ctx.Categorias.FirstOrDefault(c => c.Id == id);
		}

		public IEnumerable<Tarefa> ObtemTarefas(Func<Tarefa, bool> filtro = null)
		{
			if (filtro == null) return _ctx.Tarefas;
			return _ctx.Tarefas.Where(filtro);
		}

		public Tarefa ObtemTarefa(Func<Tarefa, bool> filtro)
		{
			return _ctx.Tarefas.Where(filtro).FirstOrDefault();
		}
	}
}
