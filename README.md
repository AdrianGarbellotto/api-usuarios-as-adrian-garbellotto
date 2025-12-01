# API de Gerenciamento de Usuários

## Descrição

Este projeto consiste no desenvolvimento de uma API REST completa para
gerenciamento de usuários, utilizando **ASP.NET Core 8 (Minimal APIs)**
e seguindo rigorosamente os princípios da **Clean Architecture**. A
aplicação foi criada com foco em escalabilidade, boas práticas de
desenvolvimento, separação de responsabilidades e aplicação de padrões
de projeto amplamente utilizados no mercado.

A API permite realizar operações CRUD completas, incluindo criação,
listagem, consulta individual, atualização e remoção (soft delete) de
usuários. Também implementa validações de negócio e validações
estruturais utilizando **FluentValidation**, além de persistir os dados
com **Entity Framework Core** e **SQLite**.

## Tecnologias Utilizadas

-   .NET 8.0\
-   ASP.NET Core Minimal APIs\
-   Entity Framework Core 8\
-   SQLite\
-   FluentValidation\
-   Clean Architecture\
-   Repository Pattern\
-   Service Pattern\
-   DTO Pattern\
-   Swagger

## Padrões de Projeto Implementados

-   **Repository Pattern**\
-   **Service Pattern**\
-   **DTO Pattern**\
-   **Dependency Injection**

## Como Executar o Projeto

## Pré-requisitos

-   .NET SDK 8.0 ou superior

## Passos

1.  Clone o repositório:

        git clone https://github.com/SEU-USUARIO/api-usuarios-as-adrian.git

2.  Entre na pasta do projeto:

        cd api-usuarios-as-adrian/APIUsuarios

3.  Execute as migrations:

        dotnet ef database update

4.  Execute a aplicação:

        dotnet run

5.  Acesse o Swagger:

        http://localhost:5000/swagger

## Exemplos de Requisições

## POST /usuarios

``` json
{
  "nome": "Adrian Santos",
  "email": "adrian@example.com",
  "senha": "123456",
  "dataNascimento": "2000-01-01",
  "telefone": "(11) 98765-4321"
}
```

## PUT /usuarios/1

``` json
{
  "nome": "Adrian Atualizado",
  "email": "adrian@example.com",
  "dataNascimento": "2000-01-01",
  "telefone": "(11) 99999-9999",
  "ativo": true
}
```

## Estrutura do Projeto

    APIUsuarios/
    ├── Domain/
    │   └── Entities/
    │       └── Usuario.cs
    │
    ├── Application/
    │   ├── DTOs/
    │   ├── Interfaces/
    │   ├── Services/
    │   └── Validators/
    │
    ├── Infrastructure/
    │   ├── Persistence/
    │   └── Repositories/
    │
    ├── Migrations/
    ├── Program.cs
    ├── appsettings.json
    └── APIUsuarios.csproj

## Autor

**Adrian dos Santos**\
Faculdade: **Análise e Desenvolvimento de Sistemas**

Link do video:

