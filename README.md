# Repositories

Esse projeto � uma Web API RESTful desenvolvida utilizando o [C#](https://docs.microsoft.com/pt-br/dotnet/csharp/) e o [ASP.NET Core](https://docs.microsoft.com/pt-br/aspnet/core/?view=aspnetcore-3.1).

Tem como finalidade armazenar dados de Desenvolvedores e de seus Projetos -- � quase um GitHub :laughing:

## Rotas dispon�veis

![]()

## Tecnologias e feramentas utilizadas

- **Persist�ncia dos dados:** foi utilizado o [SQLServer](https://www.microsoft.com/pt-br/sql-server/sql-server-2019) em conjunto com o [Entity Framework Core](https://docs.microsoft.com/pt-br/ef/core/).
- **Testes automatizados:** foi utilizado o [xUnit](https://xunit.net/) para elabora��o dos testes de integra��o.
- **Documenta��o e testes manuais:** foi utilizado o [Swagger](https://swagger.io/).
- **Controle de vers�es:**, foi utilizado o [Git Flow](https://danielkummer.github.io/git-flow-cheatsheet/index.pt_BR.html).

## Estrat�gias de solu��o

- Foi utilizado a t�cnica de [Engenharia Reversa](https://docs.microsoft.com/pt-br/ef/core/managing-schemas/scaffolding?tabs=dotnet-core-cli), que � o processo (baseado no esquema do banco de dados) de obten��o das classes de entidades e contexto.
- Foi utilizado classes DTO (Data Transfer Object) para restringir que dados sens�veis (tal como `Password`) sejam enviados na requisi��o ou resposta de alguns verbos HTTP;
- Foi realizado uma valida��o dos dados provindos da requisi��o dos verbos PUT e POST, ao qual:
  - `Name`, `Bio`, `ProjectName`, `Description` and `Languages` n�o podem ser nulos nem vazios;
  - `Email` deve ser um email v�lido (email@example.com);
  - `Password` deve conter pelo menos 1 letra, 1 digito, 1 s�mbolo e ter 8 ou mais caracteres.

***Observa��o:** Em rela��o aos testes de integra��o, a segunda vez que o teste de sucesso na rota *`POST: api/Developers`* for executado, este ir� falhar devido ao email informado ser �nico. Para tal, modifique o email antes de execut�-lo novamente.*
