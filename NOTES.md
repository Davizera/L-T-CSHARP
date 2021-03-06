# Curso de sobre Testes de Integfra??o com C#

> Aqui seguir?o anota??es de conceitos aprendidos no curso.

## Aula 1 - Testes de integra??o

### System under test (SUT)
Termo usado para se referir ? parte do sistema que est? sendo testada, sendo ele um m?todo/classe, entre outros.

### Depend?ncia ou morte
Com a implementa??o de testes n?s podemos enxegar quando uma classe est? muito acoplada, ou seja, as "coisas" nela est?o muito amarradas, ela depende de muitas coisas para rodar por si s?. Por isso, buscamos sempre aumentar a coes?o e diminuir o acomplamento entre classes e afins.  
No caso de nosso teste, havia um depend?ncia enorme com um recurso que ? caro, o banco de dados, por isso foi usado a [invers?o de controle](#inversion-of-control-ioc) para contornar esse problema, tornando a classe que chamava o banco de dados mais flex?vel e f?cil tanto de manusear quando de reusar.  
O ideal quando se tem o uso de recursos caros/lentos ? explicitar essa necessidade para que quem for utiliz?-la saiba disso de maneira a se adequar ? essa necessidade, um meio de facilitar isso ? atrav?s de inje??o de depend?ncia. J? que a depend?ncia foi explicitada, n?s podemos trabalhar de acordo com isso e contornar essa depend?ncia simulando o seu comportamento, no caso do nosso projeto n?s fizamos isso atrav?s da simula??o do reposit?rio.Para isso implementamos um reposit?rio falso que simulava o banco de dados, isso porque implementamos a interface que era usada pela classe que fazia o uso ativo do banco de dados.  
 
```csharp
//Implementa??o real
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

//Implementa??o do fake
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
Fizemos isso s? para o teste, porque o que queremos testas ? o m?todo e n?o o banco de dados, neste momento.  
Saiba mais sobre [Coes?o e Acomplamento](https://pt.stackoverflow.com/questions/81314/o-que-s%C3%A3o-os-conceitos-de-coes%C3%A3o-e-acoplamento).


### Inversion of control (IOC)
Quando invertermos o controle, isto ?, no momento em que o tiramos do programador o controle de algo, tornando assim essa determinada coisa mais flex?vel.
Podemos notar isso claramente, na mudan?a do `CadastraTarefaHandler` que teve seu c?digo modificado para receber o reposit?rio onde acessa as informa??es ao inv?s de deixar a implementa??o amarrada a esse Handler.
Para saber mais um pouco sobre visite [IOC e DI](https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/hands-on-labs/aspnet-mvc-4-dependency-injection#:~:text=Inversion%20of%20Control%20(IoC)%20means,rely%20to%20do%20their%20work.&text=Dependency%20Injection%20(DI)%20means%20that,constructor%20parameters%20and%20set%20properties.)

```csharp
//Antes da invers?o
public CadastraTarefaHandler()
        {
            _repo = new RepositorioTarefas();;
            _logger = new LoggerFactory().CreateLogger<CadastraTarefaHandler>();
        }

//Explicitando depend?ncia, utilizamos aqui o tipo mais comum de inje??o de depend?ncia
//Invertemos o controle, agora o controle est? na m?o de quem usa a classe e n?o mais a classe que tem que cuidar da chamada ao reposit?rio
public CadastraTarefaHandler(IRepositorioTarefas repositorio)
        {
            _repo = repositorio;
            _logger = new LoggerFactory().CreateLogger<CadastraTarefaHandler>();
        }

```

## Aula 2 - Depend?ncias mais leve e mais r?pidas

### InMemoryDatabase

Para mais informa??es sobre abordagem desse problema visitie o [Testando com o EF In-Memory Database](https://docs.microsoft.com/en-us/ef/core/testing/in-memory).

> [!WARNING]  
> Caso o teste do banco de dados seja relevante o pior modo de testar ? usando o InMemoryDatabase, voc? pode contornar isso atrav?s do uso do SQLite, por exemplo.

Nesta aula aprendendo um pouco sobre como trabalhar fazendo uso da abordagem InMemoryDatabase, para substituir o uso do banco de dados. Um dos pontos positivos de usar essa abordagem ? a facilidade de sua implementa??o j? que n?o precisamos clonar o comportamento de algo como fizemos na aula anterior, [Aula - 1](#aula-1---testes-de-integra??o).  
Para isso, a gente precisou refatorar alguns trecos de c?digo. Novamente fizemos a invers?od e controle, mas agora no `RepositorioTarefas` para que ao inv?s da pr?pria classe criar o `Contexto`, esse foi injetado nela, sendo assim, fica claro para quem quiser utilizar essa determinada que para us?-la se faz necess?rio a inje??o dessa determinada depend?ncia.  
Veja abaixo o trecho de c?digo que mostra a mudan?a do c?digo:
```csharp

//Antes da mudan?a, a classe criava o contexto independentemente.
//Assim ficamos engessados, dificultando os testes e flexibilidade da classe.
public RepositorioTarefa()
	{
		_ctx = new DbTarefasContext ();
	}
//Ap?s mudan?a, classe mais flex?vel e mais test?vel. 
//Agora quem tem o controle ? quem faz o uso dessa classe.
public RepositorioTarefa(DbTarefasContext dbTarefasContext)
	{
		_ctx = dbTarefasContext;
	}
```

Com essa mudan?a, foi necess?rio fazer algumas adapta??es para usar o `InMemoryDatabase` em nossa classe que realiza os testes.
Primeiro a gente teve que criar um objeto que define as configura??es que ser?o usados pelo nosso contexto, nessas configura??es a gente consegue definir o uso do `InMemoryDatabase` e passamos essas configura??es a diante para a classe que do nosso contexto, o `DbContextTarefas`, para ai sim criarmos uma inst?ncia para o nosso reposit?rio as coisas criados ? nossa maneira, j? que no invertemos o controle de como as coisas eram criadas.
Segue abaixo o trecho de c?digo que define as configura??es:
```csharp
//Criar o objeto que define as configura??es para usarmos no nosso Context.
var options = new DbContextOptionsBuilder<DbTarefasContext>()
				.UseInMemoryDatabase("DbTarefas")
				.Options;
//Cria o Context com a configura??es que definimos.
var context = new DbTarefasContext(options);

//Cria o reposit?rio com o Context que criarmos configurado ? nossa maneira.
var repo = new RepositorioTarefa(context);
			
var handler = new CadastraTarefaHandler(repo);
```

### Massa de dados

Aqui vimos o caso em que a gente precisa preencher o banco com uma "grande" quantidade de dados, mas para fazer isso num banco de teste ou um banco em que v?rios devs usam pode ser problem?tico, mas por qu?? Isso porque h? a grande chance de a base ser alterada diversas vezes entre os teste e, consequentemente te atrapalhando. Sendo assim, para que isso n?o ocorrra a gente faz o uso o InMemoryDatabase para poder simular esse comportamento, criando o contexto e afins.

> [!WARNING]
> Lembrando que o InMemoryDatabase n?o ? recomendado para fazer teste do banco de dados em sim, ele ? recomendado para quando o tipo/banco de dados **n?o importa!**
> Isso foi citado anteriormente na se??o que fala sobre o [InMemoryDatabse](#InMemoryDatabase) em si. 

#### Refer?ncias:
https://jimmybogard.com/avoid-in-memory-databases-for-tests/

## Aula 3 - Injetando dados para cen?rios complexos

Em alguns casos a gente precisa injetar/definir alguns comportamentos e sem mudar as partes do c?digo de produ??o. Nessa aula aprendemos como definir que uma exce??o seja lan?ada durante a execu??o de um determinado m?todo. Para facilitar esse processo fizemos o uso do framework Moq para definir essas configura??es.
Uma maneira de definir que uma exece??o seja lan?ada durante um determinado m?todo segue no trecho de c?digo abaixo:

```csharp
//Cria um mock do objeto que queremos
var mock = new Mock<IRepositorioTarefas>();
//Faz o setup do comportamento que queremos
//Aqui a gente definiu que para qualquer array de Tarefa uma exece??o deve ser lan?ada
mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))
	.Throws(new Exception("Houve um erro ao tentar incluir tarefa(s)"));
var repo = mock.Object;

```

Aqui a gente est? lindando com o tipo de dubl? chamado **stub**, veja mais no listagem dos [tipos de dubl?](#tipos-de-duble)

#### Refer?ncias:
https://documentation.help/
https://martinfowler.com/bliki/TestDouble.html
https://blog.pragmatists.com/test-doubles-fakes-mocks-and-stubs-1a7491dfa3da

## Aula 4 - Testando o comportamento do seu sistema

Em determinados momentos dos nossos teste no iremos querer saber se um determinado foi chamada, se ele foi chamado o tanto de vezes que deveria e coisas semelhantes, mas n?o podemos apelar para o bom e velho Console.WriteLine para ficar nos informando determinado foi chamado ou n?o n?? Afinal, o c?digo em um certo momento ir? para o ambiente de produ??o e nele queremos o c?digo com absolutamente mais nada al?m do necess?rio.  
Para esse cen?rio a gente pode fazer uso novamente da biblioteca de teste Moq, configurando o nosso c?digo da seguinte maneira:

```csharp
//C?digo que n?o importa...
//Definimos o mock do objeto que queremos usar
var mock = new Mock<IRepositorioTarefas>();
//Aqui configuramos, para qualquer chamada para o ObtemTarefas ele retorne uma lista predefinida de tarefas
mock.Setup(r => r.ObtemTarefas(It.IsAny<Func<Tarefa, bool>>()))
	.Returns(tarefas);
//C?digo que n?o importa...
//Aqui colocamos a verifica??o que queremos, e nela procuramos sabe se houve apenas uma chamada  
//ao m?todo AtualizarTarefas para qualquer array de tarefas
mock.Verify(r => r.AtualizarTarefas(It.IsAny<Tarefa[]>()), Times.Once());
```

Nessa aula a gente aprendeu sobre o dubl? chamado mock, na [listagem dos dubl?s](#tipos-de-dubles) tem uma explica??o de como ele ? usado e em quais cen?rios.

## Aula 5 - Verificando efeitos colaterais




## Tipos de dubl?s: 
1. Dummy Object (objetos sem uso)  
	O dummy object ? como pr?prio nome diz, burro. Esse objeto n?o tem utilizada alguma para gente a n?o ser preencher algo que ? obrigat?rio para que o teste n?o quebre (isso porque esse n?o est? sendo o foco do teste no momento).
2. Fake Object (objeto criado para simular determinado comportamento)
	O fake object j? ? algo mais complicado, dependendo ? claro do objeto que voc? esteja tentando simular. Um exemplo de implementa??o do fake object foi o repositorio que criamos s? para imitar alguns comportamentos implementados pelo reposit?rio verdadeiro. Em alguns casos n?o deve valer a pena utilizar esse tipo de dubl? por ser algum objeto/entidade muito complexa, nesse caso provavelmente o ideal ser? recorrer ao pr?ximo t?pico.
3. InMemoryDatabase
	Por elimina??o esse dubl? ? o ideal para quando n?o se vale a pena tentar criar um fake object ou at? mesmo fazer uso do dummy object (mas pode ser que mesmo com esse dubl? voc? fa?a uso do dummy), esse tipo de dubl? ? interassante por facilitar o seu trabalho na hora de testar, mas se lembre o diabo mora nos detalhes!
4. Stub, que ? um objeto que guarda dados predefinios e usa isso para responder as chamadas durante o teste. Ele ? muito usando quando a gente n?o pode ou n?o quer envolver objetos que ir?o responder com dados reais podendo causas efeitos n?o desejados na nossa base de dados, entre outros.  
5. Mocks, s?o objetos que registram as chamadas que eles recebem. Quando estamos testando algo s?o os mock que nos ajudam a saber de uma deteminado m?todo foi executado ou n?o. Ele pode ser usado para verificar se um m?todo que envia emails foi chamado, entre outre outras coisas.
