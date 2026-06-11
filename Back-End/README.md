# Sabemi Back-End

API do projeto Sabemi para gestão de contratos e processamento de eventos de pagamento via webhook.

## Tecnologias

- .NET 10 (ASP.NET Core Web API)
- Entity Framework Core 10
- SQL Server
- MediatR
- AutoMapper
- Hangfire
- SignalR
- Scalar (documentação da API)

## Estrutura do projeto

- `Sabemi.Api`: camada de apresentação (controllers, configuração da API)
- `Sabemi.Application`: regras de aplicação (casos de uso)
- `Sabemi.Domain`: entidades e contratos de domínio
- `Sabemi.Infra`: persistência, serviços e integrações

## Pré-requisitos

- .NET SDK 10
- SQL Server (ex.: `SQLEXPRESS`)

## Configuração

As principais configurações ficam em `Sabemi.Api/appsettings.json`:

- `ConnectionStrings:Default`
- `Webhook:ApiKey`
- `Webhook:ApiSecret`

Exemplo:

```json
"ConnectionStrings": {
  "Default": "Server=localhost\\SQLEXPRESS; Initial Catalog=Sabemi; Integrated Security=True; TrustServerCertificate=True;"
},
"Webhook": {
  "ApiKey": "123",
  "ApiSecret": "123"
}
```

## Como rodar

No diretório `Back-End`:

1. Restaurar pacotes:
   - `dotnet restore`
2. Aplicar migrations no banco:
   - `dotnet ef database update --project Sabemi.Infra --startup-project Sabemi.Api`
3. Executar a API:
   - `dotnet run --project Sabemi.Api`

## Endpoints úteis em desenvolvimento

- Documentação interativa: `https://localhost:7004/docs`
- Dashboard de jobs (Hangfire): `https://localhost:7004/hangfire`
- Hub SignalR: `https://localhost:7004/hubs/notifications`
