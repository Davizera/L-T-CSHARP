# Curso de sobre Testes de Integfra��o com C#:

Aqui seguir�o anota��es de conceitos aprendidos no curso.

### System under test (SUT)
Termo usado para se referir � parte do sistema que est� sendo testada, sendo ele um m�todo/classe, entre outros.

### Depend�ncia ou morte
Com a implementa��o de testes n�s podemos enxegar quando uma classe est� muito acoplada, ou seja, as "coisas" nela est�o muito amarradas, ela depende de muitas coisas para rodar por si s�. Por isso, buscamos sempre aumentar a coes�o e diminuir o acomplamento entre classes e afins.  
No caso de nosso teste, havia um depend�ncia enorme com um recurso que � caro, o banco de dados, por isso foi usado a [invers�o de controle](#inversion-of-control) para contornar esse problema, tornando a classe que chamava o banco de dados mais flex�vel e f�cil tanto de manusear quando de reusar.  
Para isso implementamos um reposit�rio falso que simulava o banco de dados, isso porque implementamos a interface que era usada pela classe que fazia o uso ativo do banco de dados. Fizemos isso s� para o teste, porque o que queremos testas � o m�todo e n�o o banco de dados, neste momento.  
Saiba mais sobre [Coes�o e Acomplamento](https://pt.stackoverflow.com/questions/81314/o-que-s%C3%A3o-os-conceitos-de-coes%C3%A3o-e-acoplamento).


### Inversion of control (IOC)
Quando invertermos o controle, isto �, no momento em que o tiramos do programador o controle de algo, tornando assim essa determinada coisa mais flex�vel.
Podemos notar isso claramente, na mudan�a do `CadastraTarefaHandler` que teve seu c�digo modificado para receber o reposit�rio onde acessa as informa��es ao inv�s de deixar a implementa��o amarrada a esse Handler.
Para saber mais um pouco sobre visite [IOC e DI](https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/hands-on-labs/aspnet-mvc-4-dependency-injection#:~:text=Inversion%20of%20Control%20(IoC)%20means,rely%20to%20do%20their%20work.&text=Dependency%20Injection%20(DI)%20means%20that,constructor%20parameters%20and%20set%20properties.)

