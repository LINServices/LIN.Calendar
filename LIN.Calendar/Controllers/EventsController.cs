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



}