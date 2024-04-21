namespace LIN.Contacts.Data;


public class Events
{



    #region Abstracciones


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


    #endregion




    /// <summary>
    /// Crea un evento.
    /// </summary>
    /// <param name="data">Modelo.</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<CreateResponse> Create(EventModel data, Conexión context)
    {
        // Ejecución
        try
        {

            // Los invitados.
            foreach (var guest in data.Guests)
            {
                guest.Event = data;
                context.DataBase.Attach(guest.Profile);
            }

            // Guardar el evento.
            var res = context.DataBase.Events.Add(data);
            await context.DataBase.SaveChangesAsync();
            return new(Responses.Success, data.Id);
        }
        catch
        {
        }
        return new();
    }



    /// <summary>
    /// Obtiene un evento.
    /// </summary>
    /// <param name="id">ID del evento</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<ReadOneResponse<EventModel>> Read(int id, Conexión context)
    {


        // Ejecución
        try
        {

            var profile = await (from P in context.DataBase.Events
                                 where P.Id == id
                                 select P).FirstOrDefaultAsync();

            return new(Responses.Success, profile ?? new());
        }
        catch
        {
        }
        return new();
    }



    /// <summary>
    /// Obtiene los eventos asociados a un perfil.
    /// </summary>
    /// <param name="id">ID del perfil.</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<ReadAllResponse<EventModel>> ReadAll(int id, Conexión context)
    {

        // Ejecución
        try
        {

            // Consulta.
            var contacts = await (from evento in context.DataBase.Events
                                  where evento.Guests.Any(t => t.ProfileId == id)
                                  orderby evento.Nombre
                                  select evento).ToListAsync();

            return new(Responses.Success, contacts);
        }
        catch
        {
        }
        return new();
    }



}