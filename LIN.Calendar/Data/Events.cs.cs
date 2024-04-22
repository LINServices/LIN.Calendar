namespace LIN.Calendar.Data;


public partial class Events
{


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



    /// <summary>
    /// Eliminar un evento.
    /// </summary>
    /// <param name="id">ID del evento</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<ResponseBase> Delete(int id, Conexión context)
    {


        // Ejecución
        try
        {

            var result = await (from P in context.DataBase.Guests
                                 where P.EventId == id
                                 select P).ExecuteDeleteAsync();

            result = await (from P in context.DataBase.Events
                                where P.Id == id
                                select P).ExecuteDeleteAsync();

            if (result <= 0)
                return new(Responses.NotRows);

            return new(Responses.Success);
        }
        catch
        {
        }
        return new();
    }


}