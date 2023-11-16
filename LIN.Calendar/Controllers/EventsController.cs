namespace LIN.Contacts.Controllers;


[Route("events")]
internal class EventsController : ControllerBase
{


    /// <summary>
    /// Crear evento.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="model">Modelo.</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromHeader] string token, [FromBody] EventModel model)
    {

        // Info del token.
        var (isValid, profileId, _) = Jwt.Validate(token);

        // Si el token es invalido.
        if (!isValid)
            return new CreateResponse()
            {
                Message = "Token invalido",
                Response = Responses.Unauthorized
            };

        // Validar
        if (model.Nombre.Trim().Length <= 0)
            return new CreateResponse()
            {
                Message = "Parámetros inválidos",
                Response = Responses.InvalidParam
            };

        // Agrega de quien es el contacto
        model.Profile = new()
        {
            Id = profileId
        };

        // Crear el contacto
        var response = await Data.Events.Create(model);

        return response;

    }



    /// <summary>
    /// Obtiene los eventos asociados a un perfil.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("all")]
    public async Task<HttpReadAllResponse<EventModel>> ReadAll([FromHeader] string token)
    {

        // Info dek token
        var (isValid, profileId, _) = Jwt.Validate(token);

        // Token es invalido.
        if (!isValid)
            return new ReadAllResponse<EventModel>()
            {
                Message = "Token invalido",
                Response = Responses.Unauthorized
            };

        // Obtiene los contactos
        var all = await Data.Events.ReadAll(profileId);

        // Respuesta.
        return new ReadAllResponse<EventModel>()
        {
            Models = all.Models,
            Response = Responses.Success
        };

    }


}