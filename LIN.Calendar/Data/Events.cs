namespace LIN.Calendar.Data;

public partial class Events
{

    /// <summary>
    /// Crea un evento.
    /// </summary>
    /// <param name="data">Modelo.</param>
    public static async Task<CreateResponse> Create(EventModel data)
    {

        // Contexto
        (var context, var connectionKey) = Conexión.GetOneConnection();

        // respuesta
        var response = await Create(data, context);

        context.CloseActions(connectionKey);

        return response;

    }


    /// <summary>
    /// Obtiene un evento.
    /// </summary>
    /// <param name="id">ID del evento</param>
    public static async Task<ReadOneResponse<EventModel>> Read(int id)
    {

        // Contexto
        (var context, var connectionKey) = Conexión.GetOneConnection();

        // respuesta
        var response = await Read(id, context);

        context.CloseActions(connectionKey);

        return response;

    }


    /// <summary>
    /// Obtiene los eventos asociados a un perfil
    /// </summary>
    /// <param name="id">ID del perfil</param>
    public static async Task<ReadAllResponse<EventModel>> ReadAll(int id)
    {

        // Contexto
        (var context, var connectionKey) = Conexión.GetOneConnection();

        // respuesta
        var response = await ReadAll(id, context);

        context.CloseActions(connectionKey);

        return response;

    }


    /// <summary>
    /// Eliminar un evento.
    /// </summary>
    /// <param name="id">ID del evento.</param>
    public static async Task<ResponseBase> Delete(int id)
    {

        // Contexto
        (var context, var connectionKey) = Conexión.GetOneConnection();

        // respuesta
        var response = await Delete(id, context);

        context.CloseActions(connectionKey);

        return response;

    }


    /// <summary>
    /// Actualizar un evento.
    /// </summary>
    /// <param name="event">Evento.</param>
    public static async Task<ResponseBase> Update(EventModel @event)
    {

        // Contexto
        (var context, var connectionKey) = Conexión.GetOneConnection();

        // respuesta
        var response = await Update(@event, context);

        context.CloseActions(connectionKey);

        return response;

    }

}