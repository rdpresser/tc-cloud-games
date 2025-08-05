# TC.CloudGames.User.API - Configuração do Marten

Este projeto foi configurado para usar o **Marten** como Event Store e Document Database. Aqui estão as principais configurações implementadas:

## ? Dependências Configuradas

- **Marten 8.5.0** - Event Store e Document Database
- **Swagger** - Para documentação e teste da API
- **ASP.NET Core 9.0** - Framework web moderno

## ?? Estrutura do Projeto

### Domain Layer (`TC.CloudGames.User.Domain`)
- **UserAggregate.cs** - Agregado principal do usuário com Event Sourcing
- **Eventos de Domínio**:
  - `UserCreatedEvent`
  - `UserUpdatedEvent` 
  - `UserDeactivatedEvent`

### Application Layer (`TC.CloudGames.User.Application`)
- **IUserRepository.cs** - Interface do repositório

### Infrastructure Layer (`TC.CloudGames.User.Infrastructure`)
- **UserRepository.cs** - Implementação do repositório usando Marten
- **UserProjection.cs** - Projeção para consultas eficientes
- **UserProjectionHandler.cs** - Handler para atualizar projeções

### API Layer (`TC.CloudGames.User.API`)
- **Program.cs** - Configuração principal com endpoints mínimos
- **ServiceCollectionExtensions.cs** - Configurações do Marten

## ??? Configuração do Banco de Dados

O projeto está configurado para usar PostgreSQL. Configure a connection string no `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=tc_cloudgames_user;Username=postgres;Password=postgres"
  }
}
```

## ?? Event Sourcing com Marten

### Configurações Principais:
- **Events Schema**: `events` - onde os eventos são armazenados
- **Documents Schema**: `documents` - onde as projeções são armazenadas
- **Auto Create Schema**: Habilitado em desenvolvimento
- **Projeções**: Configuradas para manter consultas eficientes

### Como Funciona:

1. **Criação de Eventos**: Quando uma operação é realizada no agregado, eventos são gerados
2. **Persistência**: Os eventos são salvos no event store do Marten
3. **Reconstrução**: O estado do agregado pode ser reconstruído a partir dos eventos
4. **Projeções**: Views otimizadas para consultas rápidas

## ?? Endpoints da API

A API foi configurada com os seguintes endpoints usando **ASP.NET Core Minimal APIs**:

- `POST /api/users` - Criar usuário
- `GET /api/users/{id}` - Obter usuário por ID
- `GET /api/users` - Listar todos os usuários
- `PUT /api/users/{id}` - Atualizar usuário
- `DELETE /api/users/{id}` - Desativar usuário
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

### 3. Acesse a documentação:
- **Swagger UI**: `https://localhost:5001/swagger`
- **Health Check**: `https://localhost:5001/health`

## ?? Comandos de Exemplo

### Criar Usuário:
```bash
curl -X POST "https://localhost:5001/api/users" \
  -H "Content-Type: application/json" \
  -d '{"email": "user@example.com", "username": "testuser"}'
```

### Obter Usuário:
```bash
curl "https://localhost:5001/api/users/{user-id}"
```

### Atualizar Usuário:
```bash
curl -X PUT "https://localhost:5001/api/users/{user-id}" \
  -H "Content-Type: application/json" \
  -d '{"email": "updated@example.com", "username": "updateduser"}'
```

### Listar Usuários:
```bash
curl "https://localhost:5001/api/users"
```

## ?? Próximos Passos - Adicionando Wolverine

Para adicionar o **WolverineFx** posteriormente e aproveitar Message Bus e handlers automáticos:

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
.IntegrateWithWolverine(); // Para integração com Marten

services.AddWolverineHttp();
```

### 3. Migrar endpoints para Wolverine HTTP:
- Criar comandos e queries
- Implementar handlers automáticos
- Usar atributos `[WolverinePost]`, `[WolverineGet]`, etc.

### 4. Benefícios do Wolverine:
- **Message Bus** interno para desacoplamento
- **Handlers automáticos** por convenção
- **Integração nativa** com Marten
- **Transactional Outbox** pattern
- **Retry policies** automáticas

## ?? Event Store - Vantagens

- **Auditoria Completa**: Todo histórico de mudanças é preservado
- **Debugging Avançado**: Facilita investigação de problemas
- **Projeções Múltiplas**: Mesmos eventos podem gerar diferentes views
- **Temporal Queries**: Consultar estado em qualquer momento do passado
- **Escalabilidade**: Separação entre escrita (eventos) e leitura (projeções)
- **Compliance**: Atende requisitos de auditoria e conformidade

## ??? Configurações Adicionais

### Variáveis de Ambiente (.env):
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
- `documents.*` - Tabelas de projeções

Você pode consultar diretamente estas tabelas para debugging e análise.