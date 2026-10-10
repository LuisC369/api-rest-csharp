# API REST en C# — CRUD con Entity Framework Core, MySQL y React

Web API desarrollada en **.NET 8** que expone endpoints para realizar operaciones **SELECT, INSERT, UPDATE y DELETE** sobre una base de datos MySQL (`db_empresa`), usando **Entity Framework Core** con el enfoque **Database First** (scaffolding).

Incluye además un **frontend en React (Vite)** que consume la API y permite administrar las tres tablas desde una interfaz web (desafío extra).

## 🎥 Video de demostración

▶️ **[Ver video en YouTube](PEGA_AQUI_EL_ENLACE_DEL_VIDEO)**

## Tecnologías

| Capa      | Tecnología |
|-----------|------------|
| Backend   | .NET 8, ASP.NET Core Web API, Entity Framework Core 8, Pomelo.EntityFrameworkCore.MySql 8.0.2, Swagger |
| Base de datos | MySQL 8 |
| Frontend  | React 19 + Vite |

## Estructura del repositorio

```
├── backend/                     # Web API en .NET
│   ├── Controllers/
│   │   ├── ClientesController.cs
│   │   ├── EmpleadosController.cs
│   │   └── PuestosController.cs
│   ├── Dtos/                    # Objetos que reciben los datos de INSERT y UPDATE
│   ├── Models/                  # Clases generadas con Scaffold-DbContext
│   │   ├── DbEmpresaContext.cs
│   │   ├── Cliente.cs
│   │   ├── Empleado.cs
│   │   └── Puesto.cs
│   ├── Program.cs               # DbContext, CORS y Swagger
│   ├── appsettings.json         # Cadena de conexión
│   └── MiPrimerApi.slnx
└── frontend/                    # Interfaz web en React
    └── src/
        ├── api.js               # Funciones para llamar a la API
        ├── App.jsx              # Pestañas Clientes / Empleados / Puestos
        └── components/Crud.jsx  # Tabla + formulario reutilizable
```

## Base de datos

Script para crear la base de datos y sus tablas. La tabla `puestos` se crea antes que `empleados` porque `empleados` tiene una llave foránea hacia ella.

```sql
CREATE DATABASE IF NOT EXISTS `db_empresa`
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_spanish_ci;

USE `db_empresa`;

CREATE TABLE `clientes` (
	`id_cliente` INT UNSIGNED NOT NULL AUTO_INCREMENT,
	`nombre` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_spanish_ci',
	`apellidos` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_spanish_ci',
	`telefono` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_spanish_ci',
	PRIMARY KEY (`id_cliente`) USING BTREE
)
COLLATE='utf8mb4_spanish_ci'
ENGINE=InnoDB
;

CREATE TABLE `puestos` (
	`id_puesto` INT UNSIGNED NOT NULL AUTO_INCREMENT,
	`nombre` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_spanish_ci',
	PRIMARY KEY (`id_puesto`) USING BTREE
)
COLLATE='utf8mb4_spanish_ci'
ENGINE=InnoDB
;

CREATE TABLE `empleados` (
	`id_empleado` INT UNSIGNED NOT NULL AUTO_INCREMENT,
	`id_puesto` INT UNSIGNED NOT NULL,
	`nombre` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_spanish_ci',
	`apellido` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_spanish_ci',
	PRIMARY KEY (`id_empleado`) USING BTREE,
	INDEX `FK_empleados_puestos` (`id_puesto`) USING BTREE,
	CONSTRAINT `FK_empleados_puestos` FOREIGN KEY (`id_puesto`) REFERENCES `puestos` (`id_puesto`) ON UPDATE NO ACTION ON DELETE NO ACTION
)
COLLATE='utf8mb4_spanish_ci'
ENGINE=InnoDB
;
```

Los modelos de C# se generaron a partir de esta base con el siguiente comando desde la Consola del Administrador de paquetes:

```powershell
Scaffold-DbContext "server=localhost;port=3306;user=root;password=TU_PASSWORD;database=db_empresa" Pomelo.EntityFrameworkCore.MySql -OutputDir Models
```

## Endpoints

### Clientes

| Método | Ruta                                | Operación |
|--------|-------------------------------------|-----------|
| GET    | `/Clientes/ListarClientes`          | SELECT de todos los clientes |
| GET    | `/Clientes/ObtenerCliente/{id}`     | SELECT de un cliente por id |
| POST   | `/Clientes/CrearCliente`            | INSERT de un cliente |
| PUT    | `/Clientes/ActualizarCliente/{id}`  | UPDATE de un cliente |
| DELETE | `/Clientes/EliminarCliente/{id}`    | DELETE de un cliente |

### Empleados

| Método | Ruta                                  | Operación |
|--------|---------------------------------------|-----------|
| GET    | `/Empleados/ListarEmpleados`          | SELECT de empleados con el nombre de su puesto (JOIN) |
| GET    | `/Empleados/ObtenerEmpleado/{id}`     | SELECT de un empleado por id |
| POST   | `/Empleados/CrearEmpleado`            | INSERT de un empleado |
| PUT    | `/Empleados/ActualizarEmpleado/{id}`  | UPDATE de un empleado |
| DELETE | `/Empleados/EliminarEmpleado/{id}`    | DELETE de un empleado |

### Puestos

| Método | Ruta                                | Operación |
|--------|-------------------------------------|-----------|
| GET    | `/Puestos/ListarPuestos`            | SELECT de todos los puestos |
| GET    | `/Puestos/ObtenerPuesto/{id}`       | SELECT de un puesto por id |
| POST   | `/Puestos/CrearPuesto`              | INSERT de un puesto |
| PUT    | `/Puestos/ActualizarPuesto/{id}`    | UPDATE de un puesto |
| DELETE | `/Puestos/EliminarPuesto/{id}`      | DELETE de un puesto (solo si no tiene empleados asignados) |

### Ejemplos de cuerpo (JSON)

Crear / actualizar cliente:

```json
{
  "nombre": "Ana",
  "apellidos": "Lopez",
  "telefono": "5555-1234"
}
```

Crear / actualizar empleado:

```json
{
  "idPuesto": 1,
  "nombre": "Carlos",
  "apellido": "Perez"
}
```

Crear / actualizar puesto:

```json
{
  "nombre": "Desarrollador"
}
```

## Cómo ejecutar

### Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download) (o superior)
- [Node.js 20](https://nodejs.org/) (o superior)
- MySQL 8 con la base `db_empresa` creada (ver script arriba)

### 1. Backend (Web API)

1. Ajustar la cadena de conexión en `backend/appsettings.json` con tu usuario y contraseña:
   ```json
   "ConnectionStrings": {
     "conexion": "server=localhost;port=3306;user=root;password=TU_PASSWORD;database=db_empresa"
   }
   ```
2. Abrir `backend/MiPrimerApi.slnx` en Visual Studio y ejecutar con el perfil **http** (F5), o desde la terminal:
   ```bash
   cd backend
   dotnet run --launch-profile http
   ```
3. La API queda en `http://localhost:5149` y Swagger en `http://localhost:5149/swagger`.

### 2. Frontend (React)

```bash
cd frontend
npm install
npm run dev
```

Abrir `http://localhost:5173`. La interfaz tiene una pestaña por tabla (Clientes, Empleados y Puestos) con su tabla de registros y un formulario para crear, editar y eliminar.

> Para registrar empleados primero debe existir al menos un puesto.
>
> Si la API corre en otra URL, se puede indicar con la variable `VITE_API_URL` (por ejemplo en un archivo `frontend/.env`).
