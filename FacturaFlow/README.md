# FacturaFlow

Aplicación de gestión de recordatorios de facturas con **ASP.NET Core Web API / C#**, **MongoDB** y **Angular**. Incluye datos de demostración para tres clientes, envío SMTP configurable, historial por factura, filtros, resumen de cartera y pruebas unitarias.

## Requisitos

- .NET SDK 8.0
- Node.js 20.19+ (o versión compatible con Angular 18) y npm
- Docker Desktop (recomendado) o MongoDB instalado
- Visual Studio 2022 con la carga de trabajo **ASP.NET y desarrollo web** (backend)
- Angular CLI se puede ejecutar desde npm sin instalación global

## 1. Iniciar MongoDB y el buzón de pruebas

Desde la carpeta raíz del proyecto:

```bash
docker compose up -d
```

Esto inicia MongoDB en `localhost:27017` y Mailpit en `localhost:8025`. Mailpit permite revisar correos sin enviarlos a clientes reales. Si no tienes Docker, instala MongoDB Community y asegúrate de que escuche en el puerto 27017.

## 2. Cargar las facturas de demostración

Abre una terminal en la carpeta raíz del proyecto y ejecuta:

```bash
mongosh "mongodb://localhost:27017/facturaflow" --file scripts/seed.js
```

Se crean seis facturas asociadas a tres clientes de prueba. Los correos `example.com` son datos ficticios; con Mailpit no se entregan al destinatario real.

## 3. Ejecutar el backend en Visual Studio

1. Abre `backend/FacturaFlow.Api/FacturaFlow.Api.csproj` con Visual Studio 2022.
2. Comprueba que `appsettings.json` contiene la conexión local de MongoDB y `Email:DryRun` está en `true` para una primera ejecución segura.
3. Selecciona el perfil `FacturaFlow.Api` y pulsa **F5** o ejecuta desde una terminal:

```bash
cd backend/FacturaFlow.Api
dotnet restore
dotnet run
```

La API queda disponible en `http://localhost:5000`. Documentación interactiva: `http://localhost:5000/swagger`. Comprobación: `http://localhost:5000/health`.

### Probar el envío sin SMTP real

Por defecto `Email:DryRun` está activado: los mensajes se simulan y quedan registrados en los logs. Esto permite comprobar el flujo y actualizar estados sin enviar correo externo.

### Probar correos en Mailpit (recomendado)

Edita `backend/FacturaFlow.Api/appsettings.json` y configura:

```json
"Email": {
  "Host": "localhost",
  "Port": 1025,
  "Username": "",
  "Password": "",
  "From": "recordatorios@facturaflow.local",
  "EnableSsl": false,
  "DryRun": false
}
```

Reinicia la API. Al procesar una factura, abre `http://localhost:8025` para ver el correo capturado. Mailpit no lo entrega a la dirección real.

### Usar un proveedor SMTP real

Configura host, puerto, remitente, TLS/SSL y credenciales válidas. En producción, **no guardes credenciales en `appsettings.json` ni en Git**; utiliza User Secrets, variables de entorno o un gestor de secretos. Solo establece `DryRun=false` después de probar con una cuenta controlada.

## 4. Ejecutar el frontend Angular

En una segunda terminal:

```bash
cd frontend
npm install
npm start
```

Abre `http://localhost:4200`. El frontend llama a la API en `http://localhost:5000`. El origen permitido se configura en `FrontendOrigin` del backend. Para desplegar en otra dirección, actualiza la URL base del `HttpClient` en `src/app/app.component.ts` y la política CORS del backend.

## 5. Ejecutar pruebas unitarias

```bash
cd backend/FacturaFlow.Tests
dotnet test
```

Las pruebas verifican las dos transiciones válidas, que un fallo de correo no actualice el estado y que una factura desactivada no vuelva a notificarse.

## Endpoints

| Método | Ruta | Función |
|---|---|---|
| `GET` | `/api/facturas` | Lista todas las facturas |
| `POST` | `/api/facturas/{id}/procesar` | Procesa una factura |
| `POST` | `/api/facturas/procesar-pendientes` | Procesa las facturas en primer o segundo recordatorio |
| `GET` | `/health` | Verifica que la API está activa |

## Regla de negocio

- `primerrecordatorio` → envía aviso de segundo recordatorio → `segundorecordatorio`.
- `segundorecordatorio` → envía aviso de posible desactivación → `desactivado`.
- `desactivado` → no envía otro aviso.
- Si el correo falla, el estado no se cambia.
- La actualización MongoDB filtra por ID y estado anterior para evitar sobrescribir silenciosamente una transición concurrente.

**Nota de consistencia:** SMTP y MongoDB no comparten una transacción distribuida. En caso de que el SMTP acepte el correo y la actualización de MongoDB falle por una modificación concurrente, el endpoint informa la discrepancia para revisión. Para producción de alto volumen conviene implementar patrón Outbox, idempotencia y una cola de reintentos.

## Estructura del proyecto

```text
FacturaFlow/
├── backend/
│   ├── FacturaFlow.Api/       # API, modelos, repositorio, servicios y configuración
│   └── FacturaFlow.Tests/     # Pruebas unitarias
├── frontend/                  # Aplicación Angular standalone y estilos responsive
├── scripts/seed.js            # Datos ficticios de tres clientes
├── docs/                      # Arquitectura y criterios de evaluación
├── docker-compose.yml         # MongoDB y Mailpit para pruebas
└── README.md
```

## Seguridad y alcance

- No se incluyen contraseñas ni credenciales reales.
- El modo inicial de simulación evita envíos accidentales.
- El panel está pensado para entorno local de evaluación. Antes de exponerlo públicamente deben añadirse autenticación/autorización, protección de secretos, validación reforzada, auditoría y controles de acceso.
- La frase de desactivación es informativa; en este alcance no existe una integración real con un sistema que desactive servicios.
