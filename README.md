# Servidor de Agenda de eventos

Guardar eventos de calendario.

## Requisitos Previos

Asegúrate de tener los siguientes requisitos previos instalados en tu sistema:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server/)

## Configuración

1. Clona este repositorio a tu máquina local:

   ```shell
   git clone https://github.com/LINServices/LIN.Calendar.git
   ```

2. Abre el proyecto en tu entorno de desarrollo favorito, como Visual Studio o Visual Studio Code.

3. Configura la cadena de conexión a la base de datos en `appsettings.json`:

   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Server=tu-servidor-sql;Database=AgendaContactos;User=usuario;Password=contrasena;"
   }
   ```

   Reemplaza `tu-servidor-sql`, `usuario`, y `contrasena` con la información de tu instancia de SQL Server.
El servidor estará en funcionamiento y escuchando en el puerto especificado en `appsettings.json` (por defecto, el puerto 5000).

## Importante

Recuerda que este servicio forma parte de LIN y usa para la autenticar, LIN Identity.
