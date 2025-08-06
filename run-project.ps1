# Script para ejecutar el proyecto de Balanceo de Carga con Nginx y MySQL Master-Slave
# Autor: Sistema de Alta Disponibilidad
# Fecha: 2024

Write-Host "=== Sistema de Balanceo de Carga con Nginx y MySQL Master-Slave ===" -ForegroundColor Green
Write-Host ""

# Verificar si Docker esta instalado y ejecutandose
Write-Host "Verificando Docker..." -ForegroundColor Yellow
try {
    docker --version | Out-Null
    Write-Host "Docker esta instalado" -ForegroundColor Green
}
catch {
    Write-Host "Docker no esta instalado o no esta ejecutandose" -ForegroundColor Red
    Write-Host "Por favor, instala Docker Desktop desde: https://www.docker.com/products/docker-desktop" -ForegroundColor Yellow
    exit 1
}

# Verificar si .NET esta instalado
Write-Host "Verificando .NET..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host ".NET $dotnetVersion esta instalado" -ForegroundColor Green
}
catch {
    Write-Host ".NET no esta instalado" -ForegroundColor Red
    Write-Host "Por favor, instala .NET 9.0 desde: https://dotnet.microsoft.com/download/dotnet/9.0" -ForegroundColor Yellow
    exit 1
}

# Verificar si dotnet-ef esta instalado
Write-Host "Verificando Entity Framework Tools..." -ForegroundColor Yellow
try {
    dotnet ef --version | Out-Null
    Write-Host "Entity Framework Tools esta instalado" -ForegroundColor Green
}
catch {
    Write-Host "Instalando Entity Framework Tools..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
    Write-Host "Entity Framework Tools instalado" -ForegroundColor Green
}

Write-Host ""
Write-Host "=== Opciones de Ejecucion ===" -ForegroundColor Cyan
Write-Host "1. Ejecutar con Docker Compose (Recomendado)"
Write-Host "2. Ejecutar solo la aplicacion (.NET)"
Write-Host "3. Ejecutar solo las bases de datos (Docker)"
Write-Host "4. Verificar estado de servicios"
Write-Host "5. Detener todos los servicios"
Write-Host "6. Ver logs"
Write-Host "7. Limpiar y reiniciar"
Write-Host ""

$opcion = Read-Host "Selecciona una opcion (1-7)"

switch ($opcion) {
    "1" {
        Write-Host "Iniciando todos los servicios con Docker Compose..." -ForegroundColor Green
        docker-compose up -d
        Write-Host ""
        Write-Host "Servicios iniciados correctamente" -ForegroundColor Green
        Write-Host "API Principal: http://localhost:80" -ForegroundColor Cyan
        Write-Host "Swagger UI: http://localhost:80/swagger" -ForegroundColor Cyan
        Write-Host "Servicio 1: http://localhost:5001" -ForegroundColor Cyan
        Write-Host "Servicio 2: http://localhost:5002" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Para verificar el estado: docker-compose ps" -ForegroundColor Yellow
        break
    }
    "2" {
        Write-Host "Compilando la aplicacion..." -ForegroundColor Yellow
        dotnet build
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Compilacion exitosa" -ForegroundColor Green
            Write-Host "Ejecutando la aplicacion..." -ForegroundColor Yellow
            dotnet run
        } else {
            Write-Host "Error en la compilacion" -ForegroundColor Red
        }
        break
    }
    "3" {
        Write-Host "Iniciando solo las bases de datos..." -ForegroundColor Yellow
        docker-compose up -d mysql-master mysql-master2 mysqlslave
        Write-Host "Bases de datos iniciadas" -ForegroundColor Green
        Write-Host "MySQL Master 1: localhost:3309" -ForegroundColor Cyan
        Write-Host "MySQL Master 2: localhost:3308" -ForegroundColor Cyan
        Write-Host "MySQL Slave: localhost:3307" -ForegroundColor Cyan
        break
    }
    "4" {
        Write-Host "Estado de los servicios:" -ForegroundColor Yellow
        docker-compose ps
        break
    }
    "5" {
        Write-Host "Deteniendo todos los servicios..." -ForegroundColor Yellow
        docker-compose down
        Write-Host "Servicios detenidos" -ForegroundColor Green
        break
    }
    "6" {
        Write-Host "Mostrando logs de todos los servicios..." -ForegroundColor Yellow
        docker-compose logs
        break
    }
    "7" {
        Write-Host "Limpiando y reiniciando..." -ForegroundColor Yellow
        docker-compose down
        docker-compose up -d
        Write-Host "Servicios reiniciados" -ForegroundColor Green
        break
    }
    default {
        Write-Host "Opcion no valida" -ForegroundColor Red
        break
    }
}

Write-Host ""
Write-Host "=== Comandos Utiles ===" -ForegroundColor Cyan
Write-Host "Verificar estado: docker-compose ps" -ForegroundColor White
Write-Host "Ver logs: docker-compose logs [servicio]" -ForegroundColor White
Write-Host "Detener: docker-compose down" -ForegroundColor White
Write-Host "Reiniciar: docker-compose restart" -ForegroundColor White
Write-Host "Ejecutar migraciones: dotnet ef database update" -ForegroundColor White
Write-Host ""

Write-Host "Presiona cualquier tecla para salir..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")