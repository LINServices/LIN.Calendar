using LIN.Types.Calendar.Models;
using LIN.Types.Responses;
using Microsoft.EntityFrameworkCore;

namespace LIN.Calendar.Persistence.Data;

public class Events(Context.DataContext context)
{

    /// <summary>
    /// Crea un evento.
    /// </summary>
    /// <param name="data">Modelo.</param>
    public async Task<CreateResponse> Create(EventModel data)
    {
        // Ejecución
        try
        {
            // Los invitados.
            foreach (var guest in data.Guests)
            {
                guest.Event = data;
                context.Attach(guest.Profile);
            }

            // Guardar el evento.
            var res = context.Events.Add(data);
            await context.SaveChangesAsync();
            return new(Responses.Success, data.Id);
        }
        catch (Exception)
        {
        }
        return new();
    }


    /// <summary>
    /// Obtiene un evento.
    /// </summary>
    /// <param name="id">ID del evento</param>
    public async Task<ReadOneResponse<EventModel>> Read(int id)
    {
        // Ejecución
        try
        {
            var eventModel = await (from @event in context.Events
                                    where @event.Id == id
                                    select @event).FirstOrDefaultAsync();

            return new(eventModel is null ? Responses.NotRows : Responses.Success, eventModel!);
        }
        catch (Exception)
        {
        }
        return new();
    }


    /// <summary>
    /// Obtiene los eventos asociados a un perfil.
    /// </summary>
    /// <param name="id">ID del perfil.</param>
    public async Task<ReadAllResponse<EventModel>> ReadAll(int id)
    {

        // Ejecución
        try
        {
            // Consulta.
            var events = await (from evento in context.Events
                                  where evento.Guests.Any(t => t.ProfileId == id)
                                  orderby evento.Nombre
                                  select new EventModel
                                  {
                                      DateStart = evento.DateStart,
                                      IsAllDay = evento.IsAllDay,
                                      EndStart = evento.EndStart,
                                      Guests = context.Guests.Where(t => t.EventId == evento.Id).Include(t => t.Profile).ToList(),
                                      Id = evento.Id,
                                      Nombre = evento.Nombre,
                                      Type = evento.Type
                                  }).ToListAsync();

            return new(Responses.Success, events);
        }
        catch (Exception)
        {
        }
        return new();
    }


    /// <summary>
    /// Eliminar un evento.
    /// </summary>
    /// <param name="id">ID del evento</param>
    public async Task<ResponseBase> Delete(int id)
    {
        // Ejecución
        try
        {

            var result = await (from P in context.Guests
                                where P.EventId == id
                                select P).ExecuteDeleteAsync();

            result = await (from P in context.Events
                            where P.Id == id
                            select P).ExecuteDeleteAsync();

            if (result <= 0)
                return new(Responses.NotRows);

            return new(Responses.Success);
        }
        catch (Exception)
        {
        }
        return new();
    }


    /// <summary>
    /// Actualizar evento.
    /// </summary>
    /// <param name="eventModel">Modelo.</param>
    public async Task<ResponseBase> Update(EventModel eventModel)
    {
        // Ejecución
        try
        {
            var count = await (from @event in context.Events
                           where @event.Id == eventModel.Id
                           select @event).ExecuteUpdateAsync(t => t.SetProperty(t => t.IsAllDay, eventModel.IsAllDay)
                           .SetProperty(t => t.Nombre, eventModel.Nombre)
                           .SetProperty(t => t.DateStart, eventModel.DateStart)
                           .SetProperty(t => t.EndStart, eventModel.EndStart)
                           .SetProperty(t => t.Type, eventModel.Type));

            return new(Responses.Success);
        }
        catch (Exception)
        {
        }
        return new();
    }

}