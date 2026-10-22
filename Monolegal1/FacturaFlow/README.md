# FacturaFlow

Aplicación de gestión de recordatorios de facturas con ASP.NET Core Web
API (.NET 8), MongoDB, Angular y Mailpit. El entorno local se ejecuta
con Docker Compose.

## Requisitos

-   Docker Desktop instalado y en ejecución.
-   El proyecto completo descargado o clonado.

No necesitas instalar .NET, Node.js ni MongoDB para ejecutar el sistema
mediante Docker Compose. Para desarrollo o pruebas unitarias fuera de
los contenedores sí pueden ser necesarios.

## Iniciar la aplicación

Abre PowerShell en la carpeta raíz del proyecto, donde está
`docker-compose.yml`, y ejecuta:

``` powershell
docker compose up --build -d
```

Comprueba los servicios con:

``` powershell
docker compose ps
```

## Direcciones

  -------------------------------------------------------------------------------
  Servicio                Dirección                       Descripción
  ----------------------- ------------------------------- -----------------------
  Frontend                http://localhost:4200           Panel de FacturaFlow

  API                     http://localhost:5000           API REST

  Swagger                 http://localhost:5000/swagger   Documentación
                                                          interactiva

  Salud de la API         http://localhost:5000/health    Comprueba que el
                                                          backend responde

  Mailpit                 http://localhost:8025           Buzón de correos de
                                                          prueba

  SMTP                    `mailpit:1025` desde los        Captura de correos
                          contenedores                    

  MongoDB                 `localhost:27017` desde el      Base de datos
                          equipo                          
  -------------------------------------------------------------------------------

## Cargar datos de demostración

El script `scripts/seed.js` agrega seis facturas ficticias de tres
clientes. Solo si las facturas aún no aparecen, ejecuta desde PowerShell
en la raíz del proyecto:

``` powershell
Get-Content .\scripts\seed.js | docker compose exec -T mongodb mongosh "mongodb://localhost:27017/facturaflow"
```

Si las facturas ya aparecen en el panel, no necesitas ejecutar el script
nuevamente.

## Probar recordatorios

El entorno Docker configura el backend para enviar correos al servicio
Mailpit (`mailpit:1025`), con SSL desactivado y `Email__DryRun=false`.
Los mensajes quedan en el buzón de pruebas y no se entregan a clientes
reales.

1.  Abre http://localhost:8025.
2.  Procesa desde el panel o la API una factura en estado
    `primerrecordatorio` o `segundorecordatorio`.
3.  Comprueba el correo en Mailpit y el estado actualizado en el panel.

Transiciones esperadas: - `primerrecordatorio` →
`segundorecordatorio`. - `segundorecordatorio` → `desactivado`. - Una
factura `desactivado` no debe generar otro recordatorio.

Mailpit es para pruebas locales. No asumas que los mensajes se
conservarán permanentemente; guarda capturas si necesitas evidencia para
una entrega.

## Endpoints principales

  -------------------------------------------------------------------------------------
  Método                  Ruta                                  Función
  ----------------------- ------------------------------------- -----------------------
  `GET`                   `/api/facturas`                       Lista las facturas

  `POST`                  `/api/facturas/{id}/procesar`         Procesa una factura

  `POST`                  `/api/facturas/procesar-pendientes`   Procesa facturas
                                                                elegibles

  `GET`                   `/health`                             Verifica que la API
                                                                esté activa
  -------------------------------------------------------------------------------------

Consulta las facturas desde PowerShell:

``` powershell
Invoke-RestMethod http://localhost:5000/api/facturas
```

Para procesar una factura, sustituye `ID_DE_LA_FACTURA` por el ID real:

``` powershell
Invoke-RestMethod -Method Post -Uri "http://localhost:5000/api/facturas/ID_DE_LA_FACTURA/procesar"
```

## Detener y volver a iniciar

Detener los contenedores sin eliminar los datos:

``` powershell
docker compose down
```

Volver a iniciar:

``` powershell
docker compose up --build -d
```

MongoDB utiliza el volumen Docker `mongo_data`; sus datos persisten al
detener y recrear los contenedores mientras no se elimine ese volumen.

**No ejecutes `docker compose down -v` si necesitas conservar la base de
datos**, porque puede eliminar el volumen.

Ver logs del backend:

``` powershell
docker compose logs backend --tail 100
```

## Pruebas unitarias (opcional)

Con el SDK de .NET 8 instalado:

``` powershell
cd backend/FacturaFlow.Tests
dotnet test
```

## Estructura general

``` text
FacturaFlow/
├── backend/
│   ├── FacturaFlow.Api/
│   └── FacturaFlow.Tests/
├── frontend/
├── scripts/seed.js
├── docs/
├── docker-compose.yml
├── README.md
└── INICIO-RAPIDO.txt
```

## Seguridad y alcance

-   Los datos de demostración son ficticios.
-   No publiques contraseñas, tokens ni credenciales SMTP reales en el
    repositorio.
-   Mailpit es un buzón de pruebas, no un servicio de correo de
    producción.
-   Antes de exponer el sistema en Internet, evalúa autenticación,
    autorización, protección de secretos, validación, auditoría y
    controles de acceso.
-   El estado `desactivado` es informativo dentro de la aplicación; no
    desactiva automáticamente un servicio externo.
