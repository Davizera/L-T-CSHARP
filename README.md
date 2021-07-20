# Curso de sobre Testes de Integfração com C#:

Aqui seguirão anotações de conceitos aprendidos no curso.

### System under test (SUT)
Termo usado para se referir à parte do sistema que está sendo testada, sendo ele um método/classe, entre outros.

### Dependência ou morte
Com a implementação de testes nós podemos enxegar quando uma classe está muito acoplada, ou seja, as "coisas" nela estão muito amarradas, ela depende de muitas coisas para rodar por si só. Por isso, buscamos sempre aumentar a coesão e diminuir o acomplamento entre classes e afins.  
No caso de nosso teste, havia um dependência enorme com um recurso que é caro, o banco de dados, por isso foi usado a [inversão de controle](#inversion-of-control) para contornar esse problema, tornando a classe que chamava o banco de dados mais flexível e fácil tanto de manusear quando de reusar.  
Para isso implementamos um repositório falso que simulava o banco de dados, isso porque implementamos a interface que era usada pela classe que fazia o uso ativo do banco de dados. Fizemos isso só para o teste, porque o que queremos testas é o método e não o banco de dados, neste momento.  
Saiba mais sobre [Coesão e Acomplamento](https://pt.stackoverflow.com/questions/81314/o-que-s%C3%A3o-os-conceitos-de-coes%C3%A3o-e-acoplamento).


### Inversion of control (IOC)
Quando invertermos o controle, isto é, no momento em que o tiramos do programador o controle de algo, tornando assim essa determinada coisa mais flexível.
Podemos notar isso claramente, na mudança do `CadastraTarefaHandler` que teve seu código modificado para receber o repositório onde acessa as informações ao invés de deixar a implementação amarrada a esse Handler.
Para saber mais um pouco sobre visite [IOC e DI](https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/hands-on-labs/aspnet-mvc-4-dependency-injection#:~:text=Inversion%20of%20Control%20(IoC)%20means,rely%20to%20do%20their%20work.&text=Dependency%20Injection%20(DI)%20means%20that,constructor%20parameters%20and%20set%20properties.)

