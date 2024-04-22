using LIN.Calendar.Data;
using LIN.Calendar.Services;

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


        model.Guests ??= [];

        if (!model.Guests.Any(t => t.ProfileId == profileId))
        {
            model.Guests.Add(new()
            {
                Profile = new()
                {
                    Id = profileId
                }
            });
        }

        // Crear el contacto
        var response = await Events.Create(model);

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
        var all = await Events.ReadAll(profileId);

        // Respuesta.
        return new ReadAllResponse<EventModel>()
        {
            Models = all.Models,
            Response = Responses.Success
        };

    }


}