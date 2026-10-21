# Arquitectura y criterios técnicos

## Capas

1. **Presentación (Angular):** panel de indicadores, filtros, tabla, estados visibles y diálogo de resultados. No contiene reglas de transición.
2. **API (ASP.NET Core):** `FacturasController` expone endpoints REST y delega la lógica en `IReminderService`.
3. **Aplicación / dominio:** `ReminderService` valida estados, construye el mensaje, solicita el envío y solo entonces solicita persistir el cambio.
4. **Infraestructura:** `MongoFacturaRepository` encapsula consultas/actualizaciones MongoDB; `SmtpEmailSender` encapsula SMTP y el modo de simulación.

## SOLID y Clean Code

- **SRP:** el controlador atiende HTTP, el servicio coordina el caso de uso, el repositorio persiste y el remitente envía correo.
- **OCP:** los servicios dependen de interfaces; el remitente puede sustituirse por otro proveedor sin cambiar el caso de uso.
- **LSP / ISP:** interfaces pequeñas (`IFacturaRepository`, `IEmailSender`, `IReminderService`) con contratos concretos.
- **DIP:** el servicio de recordatorios depende de abstracciones inyectadas por el contenedor de ASP.NET Core.
- Nombres explícitos, métodos acotados, configuración externa y manejo explícito de errores.

## Secuencia de procesamiento

```mermaid
sequenceDiagram
    actor Usuario
    participant UI as Angular
    participant API as ASP.NET Core
    participant Servicio as ReminderService
    participant SMTP as IEmailSender
    participant DB as MongoDB
    Usuario->>UI: Procesar factura
    UI->>API: POST /api/facturas/{id}/procesar
    API->>Servicio: ProcessAsync(id)
    Servicio->>DB: Leer factura
    DB-->>Servicio: Estado y datos del cliente
    Servicio->>Servicio: Validar estado y decidir transición
    Servicio->>SMTP: Enviar correo
    alt Envío correcto
      SMTP-->>Servicio: Confirmación
      Servicio->>DB: Actualizar estado si sigue igual
      DB-->>Servicio: Resultado de actualización
    else Error SMTP
      SMTP-->>Servicio: Excepción
      Servicio-->>API: Error sin cambio de estado
    end
    Servicio-->>API: Resultado
    API-->>UI: JSON de resultado
    UI-->>Usuario: Resumen y datos actualizados
```

## Matriz de pruebas

| Caso | Resultado esperado |
|---|---|
| Primer recordatorio y correo exitoso | `segundorecordatorio` |
| Segundo recordatorio y correo exitoso | `desactivado` |
| Error SMTP | Estado original sin cambios |
| Factura desactivada | No se envía correo |
| Factura inexistente | Resultado de no encontrado |
| Cambio concurrente de estado | No sobrescribir el estado cambiado |
