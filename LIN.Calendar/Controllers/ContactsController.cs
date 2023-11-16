using LIN.Contacts.Memory;

namespace LIN.Contacts.Controllers;


[Route("contacts")]
internal class ContactsController : ControllerBase
{


    /// <summary>
    /// Crear contacto.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="model">Modelo.</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromHeader] string token, [FromBody] ContactModel model)
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
                Message = "Par�metros inv�lidos",
                Response = Responses.InvalidParam
            };

        // Agrega de quien es el contacto
        model.Im = new()
        {
            Id = profileId
        };

        // Crear el contacto
        var response = await Data.Events.Create(model);

        return response;

    }



    /// <summary>
    /// Obtiene los contactos asociados a un perfil.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("all")]
    public async Task<HttpReadAllResponse<ContactModel>> ReadAll([FromHeader] string token)
    {

        // Info dek token
        var (isValid, profileId, _) = Jwt.Validate(token);

        // Token es invalido.
        if (!isValid)
            return new ReadAllResponse<ContactModel>()
            {
                Message = "Token invalido",
                Response = Responses.Unauthorized
            };

        // Obtiene los contactos
        var all = await Data.Events.ReadAll(profileId);

        // Registra en el memory
        var profileOnMemory = Mems.Sessions[profileId];

        if (profileOnMemory == null)
        {
            Mems.Sessions.Add(new()
            {
                Contactos = all.Models,
                Profile = new()
                {
                    Id = profileId
                }
            });
        }
        else
        {
            profileOnMemory.Contactos = all.Models;
        }


        // Respuesta.
        return new ReadAllResponse<ContactModel>()
        {
            Models = all.Models,
            Response = Responses.Success
        };

    }


}