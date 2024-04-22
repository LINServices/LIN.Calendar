namespace LIN.Calendar.Data;


public partial class Events
{


    /// <summary>
    /// Crea un contacto.
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
    /// Obtiene un contacto
    /// </summary>
    /// <param name="id">ID del contacto</param>
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
    /// Obtiene los contactos asociados a un perfil
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


}