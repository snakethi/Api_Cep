# ApiCep

API REST desenvolvida em .NET 8 para gerenciamento de usuários e endereços, com autenticação JWT, integração com o ViaCEP e exportação de dados em CSV.

A solução foi construída para atender ao desafio técnico de Desenvolvedor Sênior, priorizando separação de responsabilidades, segurança, resiliência, clareza do código e facilidade de execução.

## Funcionalidades

- Cadastro, consulta, atualização e exclusão lógica de usuários
- Login com autenticação JWT
- Cadastro, consulta, atualização e exclusão lógica de endereços
- Consulta de endereços pelo ViaCEP
- Paginação, busca e ordenação de usuários
- Exportação de usuários e endereços em CSV, de forma geral ou filtrada por usuário
- Documentação da API com Swagger/OpenAPI
- Logs estruturados em JSON
- Rate Limiting
- Versionamento pela URL
- Health Checks de aplicação, SQL Server e ViaCEP

## Requisitos atendidos

| Categoria | Implementação |
|---|---|
| Login | Endpoint de autenticação com e-mail e senha |
| CRUD de usuários | Criação, consulta, listagem, atualização e exclusão lógica |
| CRUD de endereços | Criação, consulta, listagem, atualização e exclusão lógica |
| ViaCEP | Consulta direta e preenchimento de endereço pelo CEP |
| Exportação CSV | Exportação geral ou filtrada por usuário, mantendo usuários sem endereço |
| JWT | Geração, assinatura e validação de Bearer Token |
| Swagger/OpenAPI | Documentação disponível em ambiente de desenvolvimento |
| Logs estruturados | Saída JSON e pipeline de logging do MediatR |
| Paginação, ordenação e filtro | Disponíveis na listagem de usuários |
| Cache do ViaCEP | Cache principal e cache de fallback |
| Resiliência | Timeout, retry, limite total e fallback |
| Health Checks | Liveness e readiness |
| Clean Architecture | Separação entre API, Application, Domain e Infrastructure |
| CQRS | Commands e Queries com MediatR |
| FluentValidation | Validação automática pelo pipeline do MediatR |
| Rate Limiting | Políticas distintas para login e demais endpoints |
| Versionamento | Rotas no formato `/api/v1/...` |

## Arquitetura adotada

A solução utiliza uma abordagem de Clean Architecture pragmática. As regras centrais não dependem de ASP.NET Core, Entity Framework, SQL Server, JWT ou serviços externos.

```text
ApiCep.Api
    │
    ├──────────────► ApiCep.Application ──────────────► ApiCep.Domain
    │                        ▲
    └──────────────► ApiCep.Infrastructure ───────────► ApiCep.Domain
                             │
                             └────────────────────────► ApiCep.Application
```

### ApiCep.Domain

Contém as entidades `User` e `Address` e suas regras de estado.

As entidades possuem propriedades com setters privados e métodos como `Update` e `Deactivate`, evitando que alterações relevantes sejam realizadas de forma inconsistente fora do domínio. Também são responsáveis por normalizações e pelo controle de datas de criação, atualização e exclusão lógica.

O projeto de domínio não depende das demais camadas.

### ApiCep.Application

Contém os casos de uso organizados em Commands e Queries, além das interfaces utilizadas para persistência, autenticação, exportação e comunicação com serviços externos.

Exemplos:

- `CreateUserCommand`
- `UpdateUserCommand`
- `LoginCommand`
- `CreateAddressCommand`
- `GetUserByIdQuery`
- `ListUsersQuery`
- `GetAddressByZipCodeQuery`
- `ExportUsersCsvQuery`

Os handlers dependem de interfaces, e não das implementações concretas da infraestrutura. A camada também concentra os validators, behaviors do MediatR, exceções de aplicação e modelos de resposta.

### ApiCep.Infrastructure

Implementa os contratos definidos pela Application.

Principais responsabilidades:

- Persistência com Entity Framework Core e SQL Server
- Repositórios de usuários, endereços e exportação
- Geração do JWT
- Hash e verificação de senha
- Comunicação HTTP com o ViaCEP
- Cache em memória
- Políticas de resiliência
- Geração de CSV
- Health Checks técnicos
- Migrations do banco

Os registros de dependência foram separados por responsabilidade: persistência, segurança, exportação, ViaCEP e Health Checks.

### ApiCep.Api

É a camada de entrada da aplicação e o Composition Root.

Contém:

- Controllers
- Configuração da autenticação JWT
- Swagger/OpenAPI
- Versionamento
- Rate Limiting
- Tratamento global de exceções
- Mapeamento dos Health Checks
- Inicialização dos módulos da Application e Infrastructure

Os controllers são finos: recebem a requisição HTTP, criam o Command ou Query correspondente e enviam a operação ao MediatR.

### ApiCep.Tests

Contém testes unitários das entidades, handlers, behaviors e serviços de infraestrutura que possuem regras ou comportamento relevante.

## Estrutura da solução

```text
Api_Cep/
├── .github/
│   └── workflows/
│       └── ci.yml
├── ApiCep/
│   ├── Authentication/
│   ├── Controllers/
│   ├── ExceptionHandling/
│   ├── HealthChecks/
│   ├── RateLimiting/
│   ├── Swagger/
│   ├── Versioning/
│   ├── Properties/
│   ├── ApiCep.Api.csproj
│   ├── ApiCep.sln
│   ├── Program.cs
│   ├── Startup.cs
│   └── appsettings.json
├── ApiCep.Application/
│   ├── Address/
│   │   ├── Commands/
│   │   ├── Models/
│   │   └── Queries/
│   ├── Authentication/
│   ├── Common/
│   ├── Exports/
│   ├── Interfaces/
│   ├── User/
│   ├── DependencyInjection.cs
│   └── ApiCep.Application.csproj
├── ApiCep.Domain/
│   ├── Entities/
│   │   ├── Address.cs
│   │   └── User.cs
│   └── ApiCep.Domain.csproj
├── ApiCep.Infrastructure/
│   ├── Authentication/
│   ├── Data/
│   │   ├── Configurations/
│   │   ├── Migrations/
│   │   └── ApplicationDbContext.cs
│   ├── DependencyInjection/
│   ├── Exports/
│   ├── ExternalServices/
│   ├── HealthChecks/
│   ├── Repositories/
│   ├── Security/
│   ├── DependencyInjection.cs
│   └── ApiCep.Infrastructure.csproj
├── ApiCep.Tests/
│   ├── Application/
│   ├── Domain/
│   ├── Infrastructure/
│   └── ApiCep.Tests.csproj
├── .gitignore
└── README.md
```

## Principais decisões técnicas

### Clean Architecture pragmática

A separação em projetos mantém o domínio independente e impede que detalhes de infraestrutura sejam levados para os casos de uso. A solução não adiciona abstrações sem uma necessidade concreta; por exemplo, não foram introduzidos Event Sourcing, Domain Events ou bancos separados apenas para aumentar a complexidade arquitetural.

### CQRS com MediatR

Commands representam operações que alteram o estado, enquanto Queries representam operações de leitura. Cada caso de uso possui um handler próprio.

O mesmo banco é utilizado para leitura e escrita. Essa decisão mantém os benefícios de separação de responsabilidades do CQRS sem introduzir sincronização, consistência eventual ou infraestrutura adicional incompatível com o escopo do projeto.

### Pipeline de behaviors

Validação e logging são preocupações transversais implementadas no pipeline do MediatR.

Fluxo simplificado:

```text
Controller
    → MediatR
        → LoggingBehavior
            → ValidationBehavior
                → Handler
```

Isso evita repetição de validações e logs em cada controller ou handler.

### FluentValidation

Os Commands e Queries que recebem dados possuem validators específicos. Falhas geram `ValidationException`, tratada globalmente como `400 Bad Request` no formato `ValidationProblemDetails`.

Regras estruturais permanecem nos validators. Regras que dependem do estado do sistema, como e-mail duplicado ou usuário inexistente, permanecem nos handlers. Invariantes das entidades permanecem no Domain.

### Autenticação JWT

O login valida a senha utilizando um serviço baseado no `PasswordHasher` do ASP.NET Core. Após a autenticação, é emitido um JWT contendo o identificador, nome e e-mail do usuário.

A validação do token verifica:

- Assinatura
- Emissor
- Audiência
- Tempo de expiração

A chave JWT não é versionada. Em desenvolvimento, deve ser configurada com User Secrets ou variável de ambiente.

O cadastro de usuário é anônimo para permitir a criação da primeira conta. Consulta, atualização, exclusão e demais recursos protegidos exigem JWT.

### Persistência e soft delete

O acesso ao SQL Server é realizado pelo Entity Framework Core. As configurações das entidades ficam separadas do `ApplicationDbContext`.

Usuários e endereços utilizam exclusão lógica. Os repositórios filtram registros com `DeletedAtUtc` preenchido, preservando o histórico e evitando exclusão física imediata.

### Integração com ViaCEP

O CEP é normalizado para oito dígitos antes de ser consultado ou utilizado como chave de cache. Isso evita entradas diferentes para valores como `01310-100` e `01310100`.

A integração utiliza `HttpClient` tipado e o resilience handler do .NET:

- Timeout por tentativa: 3 segundos
- Timeout total: 10 segundos
- Retry: 2 novas tentativas além da tentativa inicial
- Cache principal: 6 horas
- Cache de fallback: 1 dia

Quando o ViaCEP está indisponível e existe uma resposta válida anterior para o mesmo CEP, o serviço devolve o valor armazenado no cache de fallback. O fallback nunca cria ou inventa informações de endereço.

### Logs estruturados

Os logs são escritos em JSON por meio de `Microsoft.Extensions.Logging`.

O `LoggingBehavior` registra o nome e o tempo de execução dos Commands e Queries. As requisições completas não são serializadas para evitar exposição de senhas, tokens ou outros dados sensíveis.

O tratamento global de exceções utiliza o `traceId` da requisição, permitindo correlacionar a resposta HTTP com os logs internos.

### Tratamento de erros

As exceções são tratadas em um ponto central e convertidas para `ProblemDetails` ou `ValidationProblemDetails`.

Mapeamentos principais:

| Exceção | Status |
|---|---:|
| `ValidationException` | 400 |
| `ArgumentException` | 400 |
| `UnauthorizedException` | 401 |
| `NotFoundException` | 404 |
| `ConflictException` | 409 |
| Erro inesperado | 500 |

Detalhes internos de exceções inesperadas não são enviados ao cliente.

### Rate Limiting

Foram definidas duas políticas:

- Login: 5 requisições por minuto, particionadas por IP
- API: 100 requisições por minuto, particionadas pelo ID do usuário autenticado ou pelo IP

Ao exceder o limite, a API retorna `429 Too Many Requests`, `ProblemDetails`, `traceId` e o cabeçalho `Retry-After`.

### Versionamento

A versão faz parte da URL:

```text
/api/v1/users
/api/v1/auth/login
```

A escolha por segmento de URL torna o contrato explícito, facilita testes e documentação e permite introduzir uma futura versão sem alterar silenciosamente os clientes existentes.

### Health Checks

Foram separados dois tipos de verificação:

```text
GET /health/live
GET /health/ready
```

- `live`: informa se o processo da API está em execução
- `ready`: verifica SQL Server e ViaCEP

Os Health Checks são endpoints operacionais mapeados diretamente na aplicação e não aparecem no Swagger. Para testá-los, acesse as URLs diretamente pelo navegador, Postman ou outra ferramenta HTTP:

```text
https://localhost:7061/health/live
https://localhost:7061/health/ready
```

Substitua `7061` pela porta HTTPS exibida no terminal ao iniciar a API, caso seja diferente.

Os Health Checks não são versionados porque representam endpoints operacionais, e não contratos de negócio.

### Registros explícitos de dependência

As implementações da Infrastructure são registradas explicitamente. Isso facilita identificar o ciclo de vida e a implementação associada a cada interface.

O registro automático foi utilizado apenas onde existe uma convenção clara e segura, como validators e handlers localizados por assembly.

### Integração contínua

O repositório possui um workflow em `.github/workflows/ci.yml`. Em pushes e pull requests para `main` ou `master`, o GitHub Actions executa a restauração das dependências, o build em `Release` e todos os testes em um runner Linux com .NET 8.

A pipeline não executa migrations nem depende do SQL Server, da chave JWT ou dos User Secrets, porque os testes automatizados atuais são isolados de recursos externos. Isso permite validar a solução em uma máquina limpa sem expor configurações locais.

## Bibliotecas utilizadas

| Biblioteca ou recurso | Motivo |
|---|---|
| .NET 8 / ASP.NET Core | Plataforma da API e recursos nativos de autenticação, logging, DI e Rate Limiting |
| Entity Framework Core | Mapeamento das entidades, migrations e acesso ao SQL Server |
| Microsoft.EntityFrameworkCore.SqlServer | Provider do SQL Server |
| MediatR | Implementação dos Commands, Queries, handlers e pipeline behaviors |
| FluentValidation | Validação declarativa dos casos de uso |
| Asp.Versioning.Mvc | Versionamento da API |
| Asp.Versioning.Mvc.ApiExplorer | Integração das versões com o Swagger |
| Swashbuckle.AspNetCore | Geração da documentação OpenAPI e interface Swagger |
| Microsoft.Extensions.Http.Resilience | Timeout, retry e demais políticas do `HttpClient` |
| Microsoft.Extensions.Caching.Memory | Cache local das respostas do ViaCEP |
| Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore | Verificação de disponibilidade do `ApplicationDbContext` |
| CsvHelper | Geração consistente do arquivo CSV |
| xUnit | Framework de testes |
| NSubstitute | Substituição das dependências nos testes unitários |
| GitHub Actions | Validação automática de restore, build e testes a cada push ou pull request |

## Como executar

### Pré-requisitos

- .NET 8 SDK
- SQL Server acessível localmente
- Git
- `dotnet-ef` compatível com .NET 8
- Visual Studio ou outra IDE compatível, opcional

Caso o `dotnet-ef` ainda não esteja instalado:

```powershell
dotnet tool install --global dotnet-ef --version "8.*"
```

### 1. Clonar o repositório

```powershell
git clone https://github.com/snakethi/Api_Cep.git
cd Api_Cep
```

### 2. Configurar a conexão com o SQL Server

A configuração padrão está em `ApiCep/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ApiCepDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Ela utiliza autenticação integrada do Windows. Ajuste o servidor ou o modo de autenticação conforme o ambiente local.

Não versione credenciais reais.

### 3. Configurar a chave JWT

A chave deve possuir pelo menos 32 bytes e não deve ser adicionada ao `appsettings.json`.

Exemplo em PowerShell, executado na raiz do repositório:

```powershell
$key = [Guid]::NewGuid().ToString("N") + [Guid]::NewGuid().ToString("N") + [Guid]::NewGuid().ToString("N")
dotnet user-secrets set "Jwt:Key" "$key" --project .\ApiCep\ApiCep.Api.csproj
$key = $null
```

Como alternativa, configure a variável de ambiente:

```text
Jwt__Key
```

### 4. Restaurar e compilar

```powershell
dotnet restore .\ApiCep\ApiCep.sln
dotnet build .\ApiCep\ApiCep.sln
```

### 5. Criar ou atualizar o banco

A migration inicial já está incluída no repositório. Para aplicá-la pelo Visual Studio:

1. Abra a solução `ApiCep/ApiCep.sln`.
2. Defina `ApiCep.Api` como projeto de inicialização.
3. Acesse **Ferramentas → Gerenciador de Pacotes do NuGet → Console do Gerenciador de Pacotes**.
4. No campo **Projeto padrão**, selecione `ApiCep.Infrastructure`.
5. Execute:

```powershell
Update-Database
```

Como alternativa, pela linha de comando na raiz do repositório:

```powershell
dotnet ef database update --project .\ApiCep.Infrastructure\ApiCep.Infrastructure.csproj --startup-project .\ApiCep\ApiCep.Api.csproj
```

### 6. Executar a API

```powershell
dotnet run --project .\ApiCep\ApiCep.Api.csproj
```

Com o perfil atual de desenvolvimento, o Swagger pode ser acessado em:

```text
https://localhost:7061/swagger
```

Caso outra porta seja exibida no terminal, utilize a URL informada na inicialização.

O Swagger é habilitado somente no ambiente `Development`.

## Primeiro acesso

### 1. Cadastrar um usuário

O cadastro é o único recurso de usuário que não exige autenticação.

```http
POST /api/v1/users
Content-Type: application/json
```

```json
{
  "name": "Usuário Teste",
  "email": "usuario@teste.com",
  "password": "Teste@123"
}
```

### 2. Fazer login

```http
POST /api/v1/auth/login
Content-Type: application/json
```

```json
{
  "email": "usuario@teste.com",
  "password": "Teste@123"
}
```

A resposta contém `accessToken`, `tokenType`, `expiresAtUtc` e os dados básicos do usuário.

### 3. Autorizar no Swagger

Copie o valor de `accessToken`, clique em **Authorize** no Swagger e informe o token. Depois disso, os endpoints protegidos poderão ser executados pela própria interface.

## Endpoints principais

| Método | Rota | Autenticação | Descrição |
|---|---|---|---|
| POST | `/api/v1/users` | Anônimo | Cadastra um usuário |
| GET | `/api/v1/users` | JWT | Lista usuários com paginação, filtro e ordenação |
| GET | `/api/v1/users/{id}` | JWT | Consulta um usuário |
| PUT | `/api/v1/users/{id}` | JWT | Atualiza um usuário |
| DELETE | `/api/v1/users/{id}` | JWT | Exclui logicamente um usuário |
| POST | `/api/v1/auth/login` | Anônimo | Realiza login e gera o JWT |
| POST | `/api/v1/users/{userId}/addresses` | JWT | Cadastra um endereço |
| GET | `/api/v1/users/{userId}/addresses` | JWT | Lista os endereços do usuário |
| GET | `/api/v1/users/{userId}/addresses/{addressId}` | JWT | Consulta um endereço |
| PUT | `/api/v1/users/{userId}/addresses/{addressId}` | JWT | Atualiza um endereço |
| DELETE | `/api/v1/users/{userId}/addresses/{addressId}` | JWT | Exclui logicamente um endereço |
| GET | `/api/v1/zipcodes/{zipCode}` | JWT | Consulta diretamente o ViaCEP |
| GET | `/api/v1/exports/users/csv` | JWT | Exporta todos os usuários e endereços ou filtra pelo `userId` |
| GET | `/health/live` | Anônimo | Liveness Check |
| GET | `/health/ready` | Anônimo | Readiness Check |

### Exemplo de paginação, filtro e ordenação

```http
GET /api/v1/users?page=1&pageSize=10&search=thiago&sortBy=name&sortDirection=asc
Authorization: Bearer {token}
```

O parâmetro `search` é aplicado ao nome e ao e-mail. A filtragem, ordenação e paginação são executadas no banco antes da materialização dos registros.

### Exportação CSV

Para exportar todos os usuários e seus endereços:

```http
GET /api/v1/exports/users/csv
Authorization: Bearer {token}
```

Para exportar somente um usuário e seus endereços:

```http
GET /api/v1/exports/users/csv?userId={guid}
Authorization: Bearer {token}
```

O filtro é aplicado no banco antes da materialização. A consulta utiliza `LEFT JOIN`, portanto um usuário sem endereço também é incluído no arquivo. Quando um `userId` informado não existe ou foi excluído logicamente, a API retorna `404 Not Found`.

## Testes

Para executar todos os testes:

```powershell
dotnet test .\ApiCep\ApiCep.sln
```

Última execução local:

```text
Total de testes: 85
Aprovados: 85
Falhas: 0
Ignorados: 0
Build: 0 erros e 0 avisos
```

Medição local de cobertura de linhas realizada durante o desenvolvimento:

```text
Solução: 55,0%
ApiCep.Domain: 85,8%
ApiCep.Application: 50,9%
```

A estratégia de testes priorizou entidades, regras de negócio, handlers, validação, autenticação, integração com cache e geração de CSV, em vez de aumentar artificialmente a cobertura por meio de arquivos puramente declarativos de configuração.

## Trade-offs considerados

### Cache local

O `IMemoryCache` é simples e adequado para uma única instância. Em várias instâncias, cada processo manteria seu próprio cache.

### Rate Limiting em memória

O limitador nativo do ASP.NET Core protege a aplicação em uma única instância. Em ambiente distribuído, o controle deveria ser aplicado em um API Gateway ou mecanismo compartilhado.

### CQRS com o mesmo banco

Commands e Queries são separados no código, mas utilizam o mesmo SQL Server e o mesmo modelo de persistência. Bancos ou modelos de leitura separados não se justificam para o volume e o escopo atuais.

### JWT sem refresh token

O projeto emite access tokens com expiração definida, mas não implementa refresh token, revogação ou rotação de chaves. A decisão reduz o escopo sem comprometer a demonstração do fluxo de autenticação solicitado.

### Cadastro público

A criação de usuário permanece anônima para permitir o primeiro acesso. Em um sistema real, o cadastro poderia exigir convite, confirmação de e-mail, CAPTCHA ou aprovação administrativa.

### Fallback com dado anterior

O fallback melhora a disponibilidade, mas pode devolver um endereço armazenado anteriormente durante a indisponibilidade do ViaCEP. O período foi limitado para reduzir o risco de dados excessivamente antigos.

### CSV em memória

O arquivo é gerado em memória, solução suficiente para o volume esperado no desafio. Para grandes volumes, a exportação deveria utilizar streaming ou processamento assíncrono.

### Registros explícitos de DI

Os serviços da infraestrutura são registrados manualmente. Isso gera algumas linhas adicionais, mas torna as implementações e seus ciclos de vida visíveis, evitando comportamento implícito em um projeto pequeno.

## Melhorias para produção

- Armazenar segredos em Azure Key Vault, AWS Secrets Manager ou solução equivalente
- Utilizar chaves assimétricas ou um provedor de identidade para emissão de tokens
- Implementar refresh token, revogação e rotação de chaves
- Adicionar confirmação de e-mail, política de bloqueio e proteção contra automação no cadastro
- Substituir o cache local por Redis em uma implantação distribuída
- Aplicar Rate Limiting no API Gateway ou em infraestrutura compartilhada
- Adicionar OpenTelemetry para traces, métricas e correlação distribuída
- Exportar logs para Application Insights, Grafana Loki, Elasticsearch ou serviço equivalente
- Mapear falhas indisponíveis do ViaCEP para `503 Service Unavailable`
- Adicionar testes de integração com SQL Server isolado ou Testcontainers
- Adicionar testes HTTP dos controllers e do tratamento global de erros
- Evoluir a pipeline atual com relatório de cobertura, análise estática, verificação de dependências e publicação controlada
- Empacotar a aplicação em container e definir configuração por ambiente
- Utilizar streaming ou fila para exportações CSV de grande volume
- Adicionar filtros adicionais à exportação, como período de criação e situação do usuário, caso exista necessidade de negócio
- Configurar políticas de backup, observabilidade e alertas do banco
- Adicionar paginação aos endereços caso o volume por usuário cresça

## Segurança

- A chave JWT não fica no repositório
- Senhas são armazenadas somente como hash
- Tokens, senhas e requisições completas não são registrados nos logs
- Endpoints protegidos utilizam `[Authorize]`
- O cadastro público continua protegido por validação, verificação de duplicidade e Rate Limiting
- Erros inesperados não expõem detalhes internos ao cliente
- Swagger é habilitado apenas em desenvolvimento
- Arquivos locais, segredos e resultados de cobertura estão protegidos pelo `.gitignore`

## Observações finais

A solução busca demonstrar decisões compatíveis com uma API corporativa sem introduzir infraestrutura ou padrões que não sejam necessários para o problema atual.

A arquitetura permite evoluir persistência, cache, autenticação, observabilidade e integrações externas com impacto reduzido sobre os casos de uso e o domínio.
