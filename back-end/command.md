# Project Setup Guide

## This document provides step-by-step instructions to set up the Games API project with ASP.NET Core, Entity Framework Core, Swagger, and Serilog.

## Create the Project Structure:
- dotnet new sln -n Project1
- dotnet new webapi -n Games
- dotnet sln add ./Games/
- dotnet new xunit -n Games.Tests
- dotnet sln add ./Games.Tests/

## Add Dependencies:
In Games Project:
- dotnet add package Microsoft.EntityFrameworkCore.Tools
- dotnet add package Microsoft.EntityFrameworkCore.Design
- dotnet add package Microsoft.EntityFrameworkCore.SqlServer
- dotnet add package Microsoft.EntityFrameworkCore
- dotnet add package Swashbuckle.AspNetCore

## Add Serilog:
- dotnet add package Serilog.AspNetCore
- dotnet add package Serilog.Settings.Configuration
- dotnet add package Serilog.Sinks.Console

## In Games.Tests:
- dotnet add package Microsoft.EntityFrameworkCore.InMemory
- dotnet add package Xunit
- dotnet add package Xunit.runner.visualstudio
- dotnet add reference Games.Api
- dotnet add package FluentAssertions
- dotnet add package Moq
- dotnet add package coverlet.collector
- dotnet add package Microsoft.AspNetCore.Mvc.Testing
- dotnet test --collect:"XPlat Code Coverage"


## EF Core:
- dotnet ef migrations add InitialCreate -o Data/Migrations
- dotnet ef database update

## Run the API:
- dotnet watch run --project Games.Api
- Open Swagger UI in your browser: http://localhost:5182/swagger/index.html

## Install EF Core Tools Locally:
- dotnet new tool-manifest
- dotnet tool install --local dotnet-ef

## Scaffold DbContext from an Existing Database:
- dotnet ef dbcontext scaffold "Server=localhost,1433;Database=MyDatabase;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -o ./Models -c MyDatabaseContext

## Create Additional Migrations:
- dotnet ef migrations add <migration_name>
- dotnet ef database update

## Example JSONs
POST /games
```json
{
    "title": "Stardew Valley",
    "developer": "ConcernedApe",
    "releaseYear": 2016
}
```
```json
POST /platform
{
  "name": "Nintendo Switch 2",
  "manufacturer": "Nintendo",
  "releaseYear": 2025
}
```