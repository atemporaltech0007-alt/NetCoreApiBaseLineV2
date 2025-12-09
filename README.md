# Task Management System - Backend API

API RESTful desarrollada con .NET 8 como parte de la prueba técnica para Desarrollador Semi Senior/Senior.

## Descripción

API completa para gestión de tareas con autenticación JWT, Entity Framework Core, arquitectura por capas, concurrencia optimista y buenas prácticas SOLID.

## Tecnologías Utilizadas

- **.NET 8**
- **Entity Framework Core** (Code-First)
- **SQL Server**
- **JWT Authentication**
- **FluentValidation** para validaciones
- **Swagger/OpenAPI** para documentación

## Arquitectura

El proyecto sigue una arquitectura por capas limpia (Clean Architecture):

```
ApiGenerico.WebAPI/              # Capa de Presentación
├── Controllers/
│   ├── TokenController.cs       # Autenticación JWT
│   ├── TasksController.cs       # CRUD de tareas
│   └── StatesController.cs      # CRUD de estados
├── Validators/                  # FluentValidation
├── Middleware/                  # Manejo global de errores
└── Program.cs                   # Configuración

ApiGenerico.Application/         # Capa de Aplicación
├── Services/
│   ├── TaskService.cs           # Lógica de negocio tareas
│   └── StateService.cs          # Lógica de negocio estados
└── Interfaces/                  # Contratos de servicios

ApiGenerico.Domain/              # Capa de Dominio
├── Entities/
│   ├── TaskEntity.cs            # Entidad Task
│   └── StateEntity.cs           # Entidad State
└── Models/Dto/
    ├── TaskDto.cs               # DTOs de tareas
    ├── StateDto.cs              # DTOs de estados
    └── PagedResultDto.cs        # Paginación

ApiGenerico.Infrastructure/      # Capa de Infraestructura
├── Context/
│   └── ContextSql.cs            # DbContext con seeder
├── Repositories/
│   ├── TaskRepository.cs        # Repositorio tareas
│   └── StateRepository.cs       # Repositorio estados
└── Migrations/                  # Migraciones EF
```

### Decisiones Técnicas

1. **Arquitectura por Capas**: Separación clara de responsabilidades (Presentación, Aplicación, Dominio, Infraestructura).

2. **Repository Pattern**: Abstracción del acceso a datos para facilitar testing y mantenimiento.

3. **DTOs**: Transferencia de datos entre capas sin exponer entidades de dominio.

4. **Inyección de Dependencias**: Configuración en Program.cs para bajo acoplamiento.

5. **FluentValidation**: Validaciones declarativas y reutilizables separadas de la lógica de negocio.

6. **Manejo Global de Errores**: Middleware centralizado para respuestas consistentes.

7. **Concurrencia Optimista**: Columna `RowVersion` en tabla Task para detectar conflictos.

8. **Seeder de Datos**: Estados iniciales (Pendiente, En Progreso, Completado) creados automáticamente.

## Instalación y Configuración

### Prerrequisitos

- .NET 8 SDK
- SQL Server 2019 o superior
- Visual Studio 2022 o VS Code

### Instalación

```bash
# Clonar el repositorio
git clone <repository-url>
cd NetCoreApiBaseLineV2

# Restaurar dependencias
dotnet restore
```

### Configuración de Variables de Entorno

Editar `ApiGenerico.WebAPI/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "ConnetionGenerico": "Server=your_server;Database=TaskManagement;User Id=your_user;Password=your_password;Encrypt=false;"
  },
  "Authentication": {
    "SecretKey": "your_secret_key_min_32_characters_here",
    "Issuer": "https://localhost:44358/",
    "Audience": "https://localhost:44358/"
  },
  "SectionConfiguration": {
    "DuracionEnMinutosToken": 1440
  }
}
```

**Nota:** Reemplazar con valores reales en tu entorno. Las cadenas de conexión y claves pueden estar encriptadas.

### Crear Base de Datos

```bash
cd ApiGenerico.WebAPI

# Crear/actualizar base de datos
dotnet ef database update

# Si necesitas recrear desde cero
dotnet ef database drop --force
dotnet ef database update
```

La migración crea automáticamente:
- Base de datos `TaskManagement`
- Tabla `State` con 3 registros (Pendiente, En Progreso, Completado)
- Tabla `Task` con columna `RowVersion` para concurrencia
- Relaciones y constraints

### Ejecutar la API

```bash
dotnet run
```

API disponible en: **http://localhost:5016**

Swagger UI: **http://localhost:5016/swagger**

## Funcionalidades Implementadas

### Autenticación JWT
- Endpoint para obtener token
- Token con expiración configurable (24 horas por defecto)
- Todos los endpoints protegidos con [Authorize]

### Gestión de Tareas (TasksController)
- **GET /api/tasks** - Lista paginada con filtros (estado, título, fechas)
- **GET /api/tasks/{id}** - Obtiene tarea específica
- **POST /api/tasks** - Crea nueva tarea
- **PUT /api/tasks/{id}** - Actualiza tarea existente
- **DELETE /api/tasks/{id}** - Elimina tarea
- **GET /api/tasks/states** - Lista estados disponibles

### Gestión de Estados (StatesController)
- **GET /api/states** - Lista todos los estados
- **GET /api/states/{id}** - Obtiene estado específico
- **POST /api/states** - Crea nuevo estado
- **PUT /api/states/{id}** - Actualiza estado
- **DELETE /api/states/{id}** - Elimina estado (valida que no tenga tareas)

### Características Avanzadas

#### 1. Paginación
Todos los listados de tareas incluyen:
- Número de página actual
- Tamaño de página
- Total de registros
- Total de páginas

#### 2. Filtros Avanzados
Las tareas se pueden filtrar por:
- Estado (stateId)
- Título (búsqueda parcial)
- Rango de fechas (dueDateFrom, dueDateTo)

#### 3. Concurrencia Optimista (Bonus Point)
Implementado con columna `RowVersion`:
- Detecta cuando dos usuarios editan la misma tarea
- Retorna error 409 (Conflict) si hay conflicto
- Cliente debe recargar datos actualizados

```csharp
if (!task.RowVersion.SequenceEqual(dto.RowVersion))
{
    throw new InvalidOperationException("Concurrency conflict detected.");
}
```

#### 4. Validaciones con FluentValidation
- Validación de DTOs antes de procesamiento
- Mensajes de error descriptivos
- Validaciones reutilizables

#### 5. Optimización de Consultas
- Uso de `.Include()` para evitar N+1
- Proyecciones con `Select` cuando es necesario
- Queries eficientes con Entity Framework

#### 6. Manejo Global de Errores
Middleware que captura excepciones y retorna:
```json
{
  "message": "Descripción del error",
  "statusCode": 400
}
```

## Endpoints Detallados

### Autenticación

#### POST /api/token/authentication
Obtiene token JWT.

**Request:**
```json
{
  "user": "encrypted_user_credential",
  "password": "encrypted_password_credential"
}
```

**Response:**
```json
{
  "token": "eyJhbGc...",
  "expiration": "2025-12-10T18:00:00Z"
}
```

### Tareas

#### GET /api/tasks
Lista tareas con filtros y paginación.

**Parámetros:**
- `page` (int, default: 1)
- `pageSize` (int, default: 10)
- `stateId` (int, opcional)
- `title` (string, opcional)
- `dueDateFrom` (datetime, opcional)
- `dueDateTo` (datetime, opcional)

**Response:**
```json
{
  "items": [...],
  "page": 1,
  "pageSize": 10,
  "totalCount": 45,
  "totalPages": 5
}
```

#### POST /api/tasks
Crea nueva tarea.

**Request:**
```json
{
  "title": "Implementar API",
  "description": "Crear endpoints REST",
  "dueDate": "2025-12-20T18:00:00",
  "stateId": 1
}
```

**Validaciones:**
- `title`: Requerido, máximo 200 caracteres
- `description`: Opcional, máximo 4000 caracteres
- `stateId`: Requerido, debe existir
- `dueDate`: Opcional, debe ser fecha futura

#### PUT /api/tasks/{id}
Actualiza tarea existente.

**Request:**
```json
{
  "title": "API Completada",
  "description": "Todos los endpoints funcionando",
  "dueDate": "2025-12-20T18:00:00",
  "stateId": 3,
  "rowVersion": "AAAAAAAACB4="
}
```

**Importante:** Incluir `rowVersion` para control de concurrencia.

### Estados

#### POST /api/states
Crea nuevo estado.

**Request:**
```json
{
  "name": "En Revisión"
}
```

**Validaciones:**
- `name`: Requerido, máximo 100 caracteres, único

#### DELETE /api/states/{id}
Elimina estado si no tiene tareas asociadas.

**Response:** `204 No Content` o `400 Bad Request`

## Uso con Swagger

1. Abrir http://localhost:5016/swagger
2. Ejecutar **POST /api/token/authentication**
3. Copiar el token de la respuesta
4. Hacer clic en **Authorize** (candado verde)
5. Ingresar: `Bearer TU_TOKEN_AQUI`
6. Probar cualquier endpoint

## Códigos de Respuesta HTTP

| Código | Significado |
|--------|-------------|
| 200 | OK - Operación exitosa |
| 201 | Created - Recurso creado |
| 204 | No Content - Eliminación exitosa |
| 400 | Bad Request - Datos inválidos |
| 401 | Unauthorized - Token inválido/faltante |
| 404 | Not Found - Recurso no encontrado |
| 409 | Conflict - Conflicto de concurrencia |
| 500 | Internal Server Error |

## Pruebas

### Prueba de Concurrencia

1. Obtener token JWT
2. GET /api/tasks/1 en dos clientes (copiar rowVersion)
3. PUT /api/tasks/1 desde cliente 1 (éxito)
4. PUT /api/tasks/1 desde cliente 2 con rowVersion antiguo (409 Conflict)

### Prueba de Validaciones

```bash
# Crear tarea sin título (400 Bad Request)
POST /api/tasks
{
  "description": "Sin título"
}

# Crear estado duplicado (400 Bad Request)
POST /api/states
{
  "name": "Pendiente"
}
```

## Solución de Problemas

### Error de conexión a BD
```bash
# Verificar cadena de conexión en appsettings.json
# Verificar que SQL Server esté corriendo
# Recrear base de datos
dotnet ef database drop --force
dotnet ef database update
```

### Error 401 en Swagger
- Obtener nuevo token con POST /api/token/authentication
- Hacer clic en "Authorize" e ingresar: `Bearer TOKEN`

### Error en migraciones
```bash
# Eliminar carpeta Migrations
# Crear nueva migración
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Cumplimiento de Requerimientos

### Requerimientos Obligatorios
- .NET 8 con arquitectura por capas
- Entity Framework Code-First con migraciones
- Seeder de datos iniciales
- Autenticación JWT en todos los endpoints
- DTOs para transferencia de datos
- FluentValidation para validaciones
- Inyección de dependencias
- Swagger/OpenAPI
- Variables de entorno seguras
- Optimización de consultas (.Include)
- Manejo global de excepciones

### Bonus Points Implementados
- **Concurrencia Optimista**: RowVersion en tabla Task con detección de conflictos

### Patrones de Diseño Aplicados
- Repository Pattern
- Dependency Injection
- DTO Pattern
- Unit of Work (implícito con EF DbContext)

### Principios SOLID
- **S**ingle Responsibility: Cada clase tiene una responsabilidad única
- **O**pen/Closed: Extensible sin modificar código existente
- **L**iskov Substitution: Interfaces bien definidas
- **I**nterface Segregation: Interfaces específicas por funcionalidad
- **D**ependency Inversion: Dependencias mediante abstracciones

## Scripts Disponibles

```bash
dotnet restore         # Restaurar dependencias
dotnet build          # Compilar proyecto
dotnet run            # Ejecutar API
dotnet test           # Ejecutar pruebas (si existen)
dotnet ef database update  # Aplicar migraciones
```

## Estructura de Base de Datos

### Tabla: State
- Id (int, PK, Identity)
- Name (nvarchar(100), unique)
- CreatedAt (datetime2)
- UpdatedAt (datetime2)

### Tabla: Task
- Id (int, PK, Identity)
- Title (nvarchar(200), not null)
- Description (nvarchar(4000), nullable)
- DueDate (datetime2, nullable)
- StateId (int, FK → State.Id)
- CreatedAt (datetime2)
- UpdatedAt (datetime2)
- RowVersion (rowversion, concurrencia optimista)

## Notas de Seguridad

- Cadenas de conexión encriptadas
- Claves JWT seguras en appsettings
- Validación en todas las entradas
- Protección contra SQL Injection (EF parametrizado)
- CORS configurado
- Token con expiración

## Autor

Desarrollado como parte de la prueba técnica para Desarrollador Semi Senior/Senior.

## Licencia

Este proyecto es de uso educativo y de evaluación técnica.
