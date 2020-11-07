# Repositories

Esse projeto é uma Web API RESTful desenvolvida utilizando o [C#](https://docs.microsoft.com/pt-br/dotnet/csharp/) e o [ASP.NET Core](https://docs.microsoft.com/pt-br/aspnet/core/?view=aspnetcore-3.1).

Tem como finalidade armazenar dados de Desenvolvedores e de seus Projetos -- é quase um GitHub :laughing:

## Rotas disponíveis

![](https://github.com/Henrickqt/Repositories/blob/master/assets/end-points.jpg)

## Tecnologias e ferramentas utilizadas

- **Persistência dos dados:** foi utilizado o [SQLServer](https://www.microsoft.com/pt-br/sql-server/sql-server-2019) em conjunto com o [Entity Framework Core](https://docs.microsoft.com/pt-br/ef/core/).
- **Testes automatizados:** foi utilizado o [xUnit](https://xunit.net/) para elaboração dos testes de integração.
- **Documentação e testes manuais:** foi utilizado o [Swagger](https://swagger.io/).
- **Controle de versões:** foi utilizado o [Git Flow](https://danielkummer.github.io/git-flow-cheatsheet/index.pt_BR.html).

## Estratégias de solução

- Foi utilizado a técnica de [Engenharia Reversa](https://docs.microsoft.com/pt-br/ef/core/managing-schemas/scaffolding?tabs=dotnet-core-cli), que é o processo (baseado no esquema do banco de dados) de obtenção das classes de entidades e contexto.
- Foi utilizado classes DTO (Data Transfer Object) para restringir que dados sensíveis (tal como `Password`) sejam enviados na requisição ou resposta de alguns verbos HTTP;
- Foi realizado uma validação dos dados provindos da requisição dos verbos PUT e POST, ao qual:
  - `Name`, `Bio`, `ProjectName`, `Description` e `Languages` não podem ser nulos nem vazios;
  - `Email` deve ser um email válido (email@example.com);
  - `Password` deve conter pelo menos 1 letra, 1 digito, 1 símbolo e ter 8 ou mais caracteres.

***Observação:** Em relação aos testes de integração, a segunda vez que o teste de sucesso na rota* `POST: api/Developers` *for executado, este irá falhar devido ao email informado ser único. Para tal, modifique o email antes de executá-lo novamente.*
