namespace LIN.Calendar.Controllers;


[Route("events")]
public class EventsController : ControllerBase
{


    /// <summary>
    /// Crear evento.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="model">Modelo.</param>
    [HttpPost]
    [LocalToken]
    public async Task<HttpCreateResponse> Create([FromHeader] string token, [FromBody] EventModel model)
    {

        // Información del token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Validar
        if (model.Nombre.Trim().Length <= 0)
            return new CreateResponse()
            {
                Message = "Parámetros inválidos",
                Response = Responses.InvalidParam
            };

        // Integrantes.
        model.Guests ??= [];

        // Validar integrante creador.
        if (!model.Guests.Any(t => t.ProfileId == tokenInfo.ProfileId))
        {
            model.Guests.Add(new()
            {
                Profile = new()
                {
                    Id = tokenInfo.ProfileId
                }
            });
        }

        // Evaluar.
        if (model.EndStart < model.DateStart)
            model.EndStart = model.DateStart;


        if (model.IsAllDay)
        {
            model.DateStart = new DateTime(model.DateStart.Year, model.DateStart.Month, model.DateStart.Day);
            model.EndStart = new DateTime(model.DateStart.Year, model.DateStart.Month, model.DateStart.Day, 23, 59, 59);
        }

        // Crear el contacto
        var response = await Data.Events.Create(model);

        return response;

    }



    /// <summary>
    /// Obtiene los eventos asociados a un perfil.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("all")]
    [LocalToken]
    public async Task<HttpReadAllResponse<EventModel>> ReadAll([FromHeader] string token)
    {

        // Información del token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Obtiene los contactos
        var all = await Data.Events.ReadAll(tokenInfo.ProfileId);

        // Respuesta.
        return new ReadAllResponse<EventModel>()
        {
            Models = all.Models,
            Response = Responses.Success
        };

    }




    /// <summary>
    /// Eliminar un evento.
    /// </summary>
    /// <param name="id">Id del evento.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpDelete]
    [LocalToken]
    public async Task<HttpResponseBase> Delete([FromQuery] int id, [FromHeader] string token)
    {

        // Información del token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Iam.
        var iam = await Services.Iam.Validate(tokenInfo.ProfileId, id);

        // Validar Iam.
        if (iam != Types.Enumerations.IamLevels.Privileged)
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "No tienes autorización para eliminar este evento."
            };

        // Obtiene los contactos
        var response = await Data.Events.Delete(id);

        // Respuesta.
        return response;

    }



    /// <summary>
    /// Actualizar evento.
    /// </summary>
    /// <param name="eventModel">Modelo del evento.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpPatch]
    [LocalToken]
    public async Task<HttpResponseBase> Update([FromBody] EventModel eventModel, [FromHeader] string token)
    {

        // Información del token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Iam.
        var iam = await Services.Iam.Validate(tokenInfo.ProfileId, eventModel.Id);

        // Validar Iam.
        if (iam != Types.Enumerations.IamLevels.Privileged)
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "No tienes autorización para eliminar este evento."
            };

        // Obtiene los contactos
        var response = await Data.Events.Update(eventModel);

        // Respuesta.
        return response;

    }



}