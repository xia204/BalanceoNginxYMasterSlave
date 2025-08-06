# Sistema de Balanceo de Carga con Nginx y Replicación MySQL Master-Slave

## Descripción del Proyecto

Este proyecto implementa una **API REST con arquitectura de alta disponibilidad** que incluye:

- **Balanceo de carga** con Nginx entre 2 instancias de la aplicación
- **Replicación MySQL Master-Slave** para alta disponibilidad de datos
- **Patrón CQRS** con MediatR para separar comandos y consultas
- **Arquitectura limpia** con separación de capas
- **Docker Compose** para orquestación completa

## Arquitectura del Sistema

```
┌─────────────────┐    ┌─────────────────┐
│   Nginx (80)    │    │   Cliente       │
│   Load Balancer │◄──►│   (Browser/API) │
└─────────────────┘    └─────────────────┘
         │
         ▼
┌─────────────────┐    ┌─────────────────┐
│   Service 1     │    │   Service 2     │
│   (Port 5001)   │    │   (Port 5002)   │
└─────────────────┘    └─────────────────┘
         │                       │
         ▼                       ▼
┌─────────────────┐    ┌─────────────────┐
│   MySQL Master  │    │   MySQL Master2 │
│   (Port 3309)   │    │   (Port 3308)   │
└─────────────────┘    └─────────────────┘
         │                       │
         └───────────┬───────────┘
                     ▼
            ┌─────────────────┐
            │   MySQL Slave   │
            │   (Port 3307)   │
            └─────────────────┘
```

## Requisitos Previos

### Software Necesario:
1. **Docker Desktop** - Para ejecutar contenedores
2. **.NET 9.0 SDK** - Para desarrollo local
3. **Visual Studio 2022** o **VS Code** - IDE recomendado

### Instalación de Requisitos:

#### 1. Docker Desktop
```bash
# Descargar desde: https://www.docker.com/products/docker-desktop
# Instalar y reiniciar el sistema
```

#### 2. .NET 9.0 SDK
```bash
# Descargar desde: https://dotnet.microsoft.com/download/dotnet/9.0
# Verificar instalación:
dotnet --version
```

#### 3. Herramientas de Entity Framework
```bash
dotnet tool install --global dotnet-ef
```

## Configuración y Ejecución

### Opción 1: Ejecución Completa con Docker Compose (Recomendado)

1. **Clonar el repositorio:**
```bash
git clone <tu-repositorio>
cd BalanceoNginxYMasterSlave
```

2. **Ejecutar con Docker Compose:**
```bash
docker-compose up -d
```

3. **Verificar que todos los servicios estén corriendo:**
```bash
docker-compose ps
```

4. **Acceder a la aplicación:**
- **API Principal:** http://localhost:80
- **Swagger UI:** http://localhost:80/swagger
- **Servicio 1:** http://localhost:5001
- **Servicio 2:** http://localhost:5002

### Opción 2: Desarrollo Local

1. **Configurar bases de datos MySQL:**
```bash
# Instalar MySQL Server localmente o usar Docker
docker run --name mysql-master -e MYSQL_ROOT_PASSWORD=admin_123 -e MYSQL_DATABASE=libreria -p 3309:3306 -d mysql:8.0
docker run --name mysql-slave -e MYSQL_ROOT_PASSWORD=admin_123 -e MYSQL_DATABASE=libreria -p 3307:3306 -d mysql:8.0
```

2. **Actualizar appsettings.json:**
```json
{
  "ConnectionStrings": {
    "MasterConnection": "server=localhost;port=3309;database=libreria;user=root;password=admin_123;",
    "SlaveConnection": "server=localhost;port=3307;database=libreria;user=root;password=admin_123;"
  }
}
```

3. **Ejecutar migraciones:**
```bash
dotnet ef database update
```

4. **Ejecutar la aplicación:**
```bash
dotnet run
```

## Endpoints de la API

### 1. Crear Libro
```http
POST /api/LibroMaterial
Content-Type: application/json

{
  "titulo": "El Señor de los Anillos",
  "fechaPublicacion": "1954-07-29T00:00:00",
  "autorLibro": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### 2. Obtener Todos los Libros
```http
GET /api/LibroMaterial
```

### 3. Obtener Libro por ID
```http
GET /api/LibroMaterial/{id}
```

## Estructura del Proyecto

```
BalanceoNginxYMasterSlave/
├── Aplication/                    # Capa de aplicación (CQRS)
│   ├── Nuevo.cs                  # Comando para crear libros
│   ├── Consulta.cs               # Consulta para obtener libros
│   ├── ConsultaFiltro.cs         # Consulta filtrada por ID
│   ├── LibroMaterialDto.cs       # DTO para transferencia de datos
│   └── MappingPorfile.cs         # Configuración de AutoMapper
├── Controllers/                   # Controladores REST
│   └── LibroMaterialController.cs
├── Modelo/                       # Entidades del dominio
│   └── LibreriaMaterial.cs
├── Persistencia/                 # Capa de persistencia
│   ├── ContextoLibreria.cs       # Contexto para escritura (Master)
│   └── ContextoLibreriaReadOnly.cs # Contexto para lectura (Slave)
├── Extensiones/                  # Configuraciones de servicios
│   └── ServiceCollectionExtensions.cs
├── docker-compose.yaml           # Orquestación de contenedores
├── nginx.conf                    # Configuración de balanceo de carga
└── Program.cs                    # Punto de entrada de la aplicación
```

## Configuración de Base de Datos

### Replicación Master-Slave

El sistema utiliza **replicación MySQL** para alta disponibilidad:

- **Master 1 (Puerto 3309):** Para escrituras del Servicio 1
- **Master 2 (Puerto 3308):** Para escrituras del Servicio 2  
- **Slave (Puerto 3307):** Para lecturas de ambos servicios

### Configuración de Replicación

Los archivos de configuración MySQL están en:
- `mysql-master/custom.cnf` - Configuración del Master 1
- `mysql-master2/custom.cnf` - Configuración del Master 2
- `mysql-slave/custom.cnf` - Configuración del Slave

## Balanceo de Carga

Nginx distribuye las peticiones entre los dos servicios:
- **Servicio 1:** Puerto 5001
- **Servicio 2:** Puerto 5002

### Configuración de Nginx

```nginx
upstream backend {
    server uttt-micro-service1:5000;
    server uttt-micro-service2:5000;
}
```

## Monitoreo y Logs

### Ver logs de contenedores:
```bash
# Todos los servicios
docker-compose logs

# Servicio específico
docker-compose logs nginx
docker-compose logs uttt-micro-service1
docker-compose logs mysql-master
```

### Verificar estado de servicios:
```bash
docker-compose ps
```

## Troubleshooting

### Problemas Comunes:

1. **Puertos ocupados:**
```bash
# Verificar puertos en uso
netstat -an | findstr :80
netstat -an | findstr :3307
netstat -an | findstr :3308
netstat -an | findstr :3309
```

2. **Contenedores no inician:**
```bash
# Limpiar y reiniciar
docker-compose down
docker-compose up -d
```

3. **Base de datos no conecta:**
```bash
# Verificar logs de MySQL
docker-compose logs mysql-master
docker-compose logs mysql-slave
```

4. **Migraciones fallan:**
```bash
# Ejecutar migraciones manualmente
dotnet ef database update
```

## Desarrollo

### Agregar Nuevas Entidades:

1. **Crear modelo en `Modelo/`**
2. **Agregar DbSet en `ContextoLibreria.cs`**
3. **Crear migración:**
```bash
dotnet ef migrations add NombreMigracion
dotnet ef database update
```

### Agregar Nuevos Endpoints:

1. **Crear comando/consulta en `Aplication/`**
2. **Agregar endpoint en `Controllers/`**
3. **Configurar AutoMapper si es necesario**

## Tecnologías Utilizadas

- **.NET 9.0** - Framework de desarrollo
- **Entity Framework Core 8.0** - ORM
- **MySQL 8.0** - Base de datos
- **Nginx** - Balanceador de carga
- **Docker & Docker Compose** - Contenedores
- **MediatR** - Patrón CQRS
- **AutoMapper** - Mapeo de objetos
- **FluentValidation** - Validaciones
- **Swagger** - Documentación de API

## Contribución

1. Fork el proyecto
2. Crear rama feature (`git checkout -b feature/AmazingFeature`)
3. Commit cambios (`git commit -m 'Add AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abrir Pull Request

## Licencia

Este proyecto está bajo la Licencia MIT.

Para las masters crear el usuario replicador

CREATE USER 'replicador'@'%' IDENTIFIED WITH mysql_native_password BY 'admin_123';

GRANT REPLICATION SLAVE ON . TO 'replicador'@'%';

FLUSH PRIVILEGES;

SHOW MASTER STATUS;

Copiar el file que da y position

Para slave Cambiar los datos pra cada replica

CHANGE REPLICATION SOURCE TO SOURCE_HOST='mysql-master2', SOURCE_PORT=3306, SOURCE_USER='replicador', SOURCE_PASSWORD='admin_123', SOURCE_LOG_FILE='binlog.000002', SOURCE_LOG_POS=8170 FOR CHANNEL 'master2';

Revisar que la replica se realizo de forma correcta

START REPLICA FOR CHANNEL 'master2';

SHOW REPLICA STATUS FOR CHANNEL 'master2'\G

SHOW REPLICAS;

Buscar esos campos para verificar que esta funcionando

Campo	Significado esperado
Replica_IO_Running	Yes ✅
Replica_SQL_Running	Yes ✅
Last_IO_Error	(vacío o NULL) ✅
Last_SQL_Error	(vacío o NULL) ✅
Seconds_Behind_Source	0 o valor bajo 🟢
Por si algo sale mal

Para master

REVOKE REPLICATION SLAVE ON . FROM 'replicador'@'%';

DROP USER 'replicador'@'%';

SELECT user, host FROM mysql.user WHERE user = 'replicador';

Para slave

STOP REPLICA FOR CHANNEL 'master2';

RESET REPLICA FOR CHANNEL 'master2';
