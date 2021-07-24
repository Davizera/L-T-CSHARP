# Curso de sobre Testes de Integfra��o com C#

> Aqui seguir�o anota��es de conceitos aprendidos no curso.

## Aula 1 - Testes de integra��o

### System under test (SUT)
Termo usado para se referir � parte do sistema que est� sendo testada, sendo ele um m�todo/classe, entre outros.

### Depend�ncia ou morte
Com a implementa��o de testes n�s podemos enxegar quando uma classe est� muito acoplada, ou seja, as "coisas" nela est�o muito amarradas, ela depende de muitas coisas para rodar por si s�. Por isso, buscamos sempre aumentar a coes�o e diminuir o acomplamento entre classes e afins.  
No caso de nosso teste, havia um depend�ncia enorme com um recurso que � caro, o banco de dados, por isso foi usado a [invers�o de controle](#inversion-of-control-ioc) para contornar esse problema, tornando a classe que chamava o banco de dados mais flex�vel e f�cil tanto de manusear quando de reusar.  
O ideal quando se tem o uso de recursos caros/lentos � explicitar essa necessidade para que quem for utiliz�-la saiba disso de maneira a se adequar � essa necessidade, um meio de facilitar isso � atrav�s de inje��o de depend�ncia. J� que a depend�ncia foi explicitada, n�s podemos trabalhar de acordo com isso e contornar essa depend�ncia simulando o seu comportamento, no caso do nosso projeto n�s fizamos isso atrav�s da simula��o do reposit�rio.Para isso implementamos um reposit�rio falso que simulava o banco de dados, isso porque implementamos a interface que era usada pela classe que fazia o uso ativo do banco de dados.  
 
```csharp
//Implementa��o real
public class RepositorioTarefa : IRepositorioTarefas
	{
		DbTarefasContext _ctx;

		public RepositorioTarefa()
		{
			_ctx = new DbTarefasContext();
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

		public IEnumerable<Tarefa> ObtemTarefas(Func<Tarefa, bool> filtro)
		{
			return _ctx.Tarefas.Where(filtro);
		}
	}

//Implementa��o do fake
public class RepoFakeTarefas : IRepositorioTarefas
	{
		List<Tarefa> lista = new List<Tarefa>();

		public void AtualizarTarefas(params Tarefa[] tarefas)
		{
			throw new NotImplementedException();
		}

		public void ExcluirTarefas(params Tarefa[] tarefas)
		{
			throw new NotImplementedException();
		}

		public void IncluirTarefas(params Tarefa[] tarefas)
		{
			tarefas.ToList().ForEach(t => lista.Add(t));
		}

		public Categoria ObtemCategoriaPorId(int id)
		{
			return new Categoria(id, string.Empty);
		}

		public IEnumerable<Tarefa> ObtemTarefas(Func<Tarefa, bool> filtro)
		{
			return lista.Where(filtro);
		}
		public Tarefa ObtemTarefa(Func<Tarefa, bool> filtro)
		{
			return lista.Where(filtro).FirstOrDefault();
		}
	}
```
Fizemos isso s� para o teste, porque o que queremos testas � o m�todo e n�o o banco de dados, neste momento.  
Saiba mais sobre [Coes�o e Acomplamento](https://pt.stackoverflow.com/questions/81314/o-que-s%C3%A3o-os-conceitos-de-coes%C3%A3o-e-acoplamento).


### Inversion of control (IOC)
Quando invertermos o controle, isto �, no momento em que o tiramos do programador o controle de algo, tornando assim essa determinada coisa mais flex�vel.
Podemos notar isso claramente, na mudan�a do `CadastraTarefaHandler` que teve seu c�digo modificado para receber o reposit�rio onde acessa as informa��es ao inv�s de deixar a implementa��o amarrada a esse Handler.
Para saber mais um pouco sobre visite [IOC e DI](https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/hands-on-labs/aspnet-mvc-4-dependency-injection#:~:text=Inversion%20of%20Control%20(IoC)%20means,rely%20to%20do%20their%20work.&text=Dependency%20Injection%20(DI)%20means%20that,constructor%20parameters%20and%20set%20properties.)

```csharp
//Antes da invers�o
public CadastraTarefaHandler()
        {
            _repo = new RepositorioTarefas();;
            _logger = new LoggerFactory().CreateLogger<CadastraTarefaHandler>();
        }

//Explicitando depend�ncia, utilizamos aqui o tipo mais comum de inje��o de depend�ncia
//Invertemos o controle, agora o controle est� na m�o de quem usa a classe e n�o mais a classe que tem que cuidar da chamada ao reposit�rio
public CadastraTarefaHandler(IRepositorioTarefas repositorio)
        {
            _repo = repositorio;
            _logger = new LoggerFactory().CreateLogger<CadastraTarefaHandler>();
        }

```

## Aula 2 - Depend�ncias mais leve e mais r�pidas

### InMemoryDatabase

Para mais informa��es sobre abordagem desse problema visitie o [Testando com o EF In-Memory Database](https://docs.microsoft.com/en-us/ef/core/testing/in-memory).

>[!WARNING]  
> Caso o teste do banco de dados seja relevante o pior modo de testar � usando o InMemoryDatabase, voc� pode contornar isso atrav�s do uso do SQLite, por exemplo.


