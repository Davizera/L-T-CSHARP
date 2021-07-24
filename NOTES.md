# Curso de sobre Testes de Integfração com C#

> Aqui seguirão anotações de conceitos aprendidos no curso.

## Aula 1 - Testes de integração

### System under test (SUT)
Termo usado para se referir à parte do sistema que está sendo testada, sendo ele um método/classe, entre outros.

### Dependência ou morte
Com a implementação de testes nós podemos enxegar quando uma classe está muito acoplada, ou seja, as "coisas" nela estão muito amarradas, ela depende de muitas coisas para rodar por si só. Por isso, buscamos sempre aumentar a coesão e diminuir o acomplamento entre classes e afins.  
No caso de nosso teste, havia um dependência enorme com um recurso que é caro, o banco de dados, por isso foi usado a [inversão de controle](#inversion-of-control-ioc) para contornar esse problema, tornando a classe que chamava o banco de dados mais flexível e fácil tanto de manusear quando de reusar.  
O ideal quando se tem o uso de recursos caros/lentos é explicitar essa necessidade para que quem for utilizá-la saiba disso de maneira a se adequar à essa necessidade, um meio de facilitar isso é através de injeção de dependência. Já que a dependência foi explicitada, nós podemos trabalhar de acordo com isso e contornar essa dependência simulando o seu comportamento, no caso do nosso projeto nós fizamos isso através da simulação do repositório.Para isso implementamos um repositório falso que simulava o banco de dados, isso porque implementamos a interface que era usada pela classe que fazia o uso ativo do banco de dados.  
 
```csharp
//Implementação real
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

//Implementação do fake
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
Fizemos isso só para o teste, porque o que queremos testas é o método e não o banco de dados, neste momento.  
Saiba mais sobre [Coesão e Acomplamento](https://pt.stackoverflow.com/questions/81314/o-que-s%C3%A3o-os-conceitos-de-coes%C3%A3o-e-acoplamento).


### Inversion of control (IOC)
Quando invertermos o controle, isto é, no momento em que o tiramos do programador o controle de algo, tornando assim essa determinada coisa mais flexível.
Podemos notar isso claramente, na mudança do `CadastraTarefaHandler` que teve seu código modificado para receber o repositório onde acessa as informações ao invés de deixar a implementação amarrada a esse Handler.
Para saber mais um pouco sobre visite [IOC e DI](https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/hands-on-labs/aspnet-mvc-4-dependency-injection#:~:text=Inversion%20of%20Control%20(IoC)%20means,rely%20to%20do%20their%20work.&text=Dependency%20Injection%20(DI)%20means%20that,constructor%20parameters%20and%20set%20properties.)

```csharp
//Antes da inversão
public CadastraTarefaHandler()
        {
            _repo = new RepositorioTarefas();;
            _logger = new LoggerFactory().CreateLogger<CadastraTarefaHandler>();
        }

//Explicitando dependência, utilizamos aqui o tipo mais comum de injeção de dependência
//Invertemos o controle, agora o controle está na mão de quem usa a classe e não mais a classe que tem que cuidar da chamada ao repositório
public CadastraTarefaHandler(IRepositorioTarefas repositorio)
        {
            _repo = repositorio;
            _logger = new LoggerFactory().CreateLogger<CadastraTarefaHandler>();
        }

```

## Aula 2 - Dependências mais leve e mais rápidas

### InMemoryDatabase

Para mais informações sobre abordagem desse problema visitie o [Testando com o EF In-Memory Database](https://docs.microsoft.com/en-us/ef/core/testing/in-memory).

>[!WARNING]  
> Caso o teste do banco de dados seja relevante o pior modo de testar é usando o InMemoryDatabase, você pode contornar isso através do uso do SQLite, por exemplo.


