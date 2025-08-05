# TC.CloudGames.User.API - Configura��o do Marten

Este projeto foi configurado para usar o **Marten** como Event Store e Document Database. Aqui est�o as principais configura��es implementadas:

## ? Depend�ncias Configuradas

- **Marten 8.5.0** - Event Store e Document Database
- **Swagger** - Para documenta��o e teste da API
- **ASP.NET Core 9.0** - Framework web moderno

## ?? Estrutura do Projeto

### Domain Layer (`TC.CloudGames.User.Domain`)
- **UserAggregate.cs** - Agregado principal do usu�rio com Event Sourcing
- **Eventos de Dom�nio**:
  - `UserCreatedEvent`
  - `UserUpdatedEvent` 
  - `UserDeactivatedEvent`

### Application Layer (`TC.CloudGames.User.Application`)
- **IUserRepository.cs** - Interface do reposit�rio

### Infrastructure Layer (`TC.CloudGames.User.Infrastructure`)
- **UserRepository.cs** - Implementa��o do reposit�rio usando Marten
- **UserProjection.cs** - Proje��o para consultas eficientes
- **UserProjectionHandler.cs** - Handler para atualizar proje��es

### API Layer (`TC.CloudGames.User.API`)
- **Program.cs** - Configura��o principal com endpoints m�nimos
- **ServiceCollectionExtensions.cs** - Configura��es do Marten

## ??? Configura��o do Banco de Dados

O projeto est� configurado para usar PostgreSQL. Configure a connection string no `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=tc_cloudgames_user;Username=postgres;Password=postgres"
  }
}
```

## ?? Event Sourcing com Marten

### Configura��es Principais:
- **Events Schema**: `events` - onde os eventos s�o armazenados
- **Documents Schema**: `documents` - onde as proje��es s�o armazenadas
- **Auto Create Schema**: Habilitado em desenvolvimento
- **Proje��es**: Configuradas para manter consultas eficientes

### Como Funciona:

1. **Cria��o de Eventos**: Quando uma opera��o � realizada no agregado, eventos s�o gerados
2. **Persist�ncia**: Os eventos s�o salvos no event store do Marten
3. **Reconstru��o**: O estado do agregado pode ser reconstru�do a partir dos eventos
4. **Proje��es**: Views otimizadas para consultas r�pidas

## ?? Endpoints da API

A API foi configurada com os seguintes endpoints usando **ASP.NET Core Minimal APIs**:

- `POST /api/users` - Criar usu�rio
- `GET /api/users/{id}` - Obter usu�rio por ID
- `GET /api/users` - Listar todos os usu�rios
- `PUT /api/users/{id}` - Atualizar usu�rio
- `DELETE /api/users/{id}` - Desativar usu�rio
- `GET /health` - Health check

## ????? Como Executar

### 1. Configure o PostgreSQL:
```bash
docker run --name postgres-marten \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=tc_cloudgames_user \
  -p 5432:5432 \
  -d postgres:15
```

### 2. Execute o projeto:
```bash
cd user-services/src/TC.CloudGames.User.API
dotnet run
```

### 3. Acesse a documenta��o:
- **Swagger UI**: `https://localhost:5001/swagger`
- **Health Check**: `https://localhost:5001/health`

## ?? Comandos de Exemplo

### Criar Usu�rio:
```bash
curl -X POST "https://localhost:5001/api/users" \
  -H "Content-Type: application/json" \
  -d '{"email": "user@example.com", "username": "testuser"}'
```

### Obter Usu�rio:
```bash
curl "https://localhost:5001/api/users/{user-id}"
```

### Atualizar Usu�rio:
```bash
curl -X PUT "https://localhost:5001/api/users/{user-id}" \
  -H "Content-Type: application/json" \
  -d '{"email": "updated@example.com", "username": "updateduser"}'
```

### Listar Usu�rios:
```bash
curl "https://localhost:5001/api/users"
```

## ?? Pr�ximos Passos - Adicionando Wolverine

Para adicionar o **WolverineFx** posteriormente e aproveitar Message Bus e handlers autom�ticos:

### 1. Adicionar pacotes:
```bash
dotnet add package WolverineFx
dotnet add package WolverineFx.Http
dotnet add package WolverineFx.Marten
```

### 2. Configurar no `ServiceCollectionExtensions.cs`:
```csharp
services.AddWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
    opts.Discovery.IncludeAssembly(typeof(CreateUserCommand).Assembly);
})
.IntegrateWithWolverine(); // Para integra��o com Marten

services.AddWolverineHttp();
```

### 3. Migrar endpoints para Wolverine HTTP:
- Criar comandos e queries
- Implementar handlers autom�ticos
- Usar atributos `[WolverinePost]`, `[WolverineGet]`, etc.

### 4. Benef�cios do Wolverine:
- **Message Bus** interno para desacoplamento
- **Handlers autom�ticos** por conven��o
- **Integra��o nativa** com Marten
- **Transactional Outbox** pattern
- **Retry policies** autom�ticas

## ?? Event Store - Vantagens

- **Auditoria Completa**: Todo hist�rico de mudan�as � preservado
- **Debugging Avan�ado**: Facilita investiga��o de problemas
- **Proje��es M�ltiplas**: Mesmos eventos podem gerar diferentes views
- **Temporal Queries**: Consultar estado em qualquer momento do passado
- **Escalabilidade**: Separa��o entre escrita (eventos) e leitura (proje��es)
- **Compliance**: Atende requisitos de auditoria e conformidade

## ??? Configura��es Adicionais

### Vari�veis de Ambiente (.env):
```env
CONNECTION_STRING=Host=localhost;Database=tc_cloudgames_user;Username=postgres;Password=postgres
ASPNETCORE_ENVIRONMENT=Development
```

### Docker Compose (opcional):
```yaml
version: '3.8'
services:
  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: tc_cloudgames_user
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:
```

## ?? Monitoramento

O Marten automaticamente cria as seguintes tabelas no PostgreSQL:
- `events.mt_events` - Armazena todos os eventos
- `events.mt_streams` - Metadados dos streams
- `documents.*` - Tabelas de proje��es

Voc� pode consultar diretamente estas tabelas para debugging e an�lise.