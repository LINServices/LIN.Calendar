namespace LIN.Calendar.Controllers;


[Route("profile")]
public class ProfileController : ControllerBase
{


    /// <summary>
    /// Inicia una sesión de usuario.
    /// </summary>
    /// <param name="user">Usuario único.</param>
    /// <param name="password">Contraseña del usuario.</param>
    [HttpGet("login")]
    public async Task<HttpReadOneResponse<AuthModel<ProfileModel>>> Login([FromQuery] string user, [FromQuery] string password)
    {

        // Validar parámetros.
        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
            return new(Responses.InvalidParam);

        // Respuesta de autenticación.
        var authResponse = await Access.Auth.Controllers.Authentication.Login(user, password, App.AppCode);

        // Autenticación errónea.
        if (authResponse.Response != Responses.Success)
            return new ReadOneResponse<AuthModel<ProfileModel>>
            {
                Message = "Autenticación fallida",
                Response = authResponse.Response
            };

        // Obtiene el perfil
        var profile = await Data.Profiles.ReadByAccount(authResponse.Model.Id);

        // Validar la respuesta.
        switch (profile.Response)
        {

            // Correcto.
            case Responses.Success:
                break;

            // El perfil no existe.
            case Responses.NotExistProfile:
                {

                    // Crear.
                    var createResponse = await Data.Profiles.Create(new()
                    {
                        Account = authResponse.Model,
                        Profile = new()
                        {
                            Name = authResponse.Model.Name,
                            AccountId = authResponse.Model.Id,
                            Creation = DateTime.Now
                        }
                    });

                    // Validar.
                    if (createResponse.Response != Responses.Success)
                    {
                        return new ReadOneResponse<AuthModel<ProfileModel>>
                        {
                            Response = Responses.UnavailableService,
                            Message = "Un error grave ocurrió"
                        };
                    }

                    profile = createResponse;
                    break;
                }

            // Si hubo un error grave.
            default:
                return new ReadOneResponse<AuthModel<ProfileModel>>
                {
                    Response = Responses.UnavailableService,
                    Message = "Un error grave ocurrió"
                };
        }


        // Genera el token
        var token = Jwt.Generate(profile.Model);


        // Respuesta.
        return new ReadOneResponse<AuthModel<ProfileModel>>
        {
            Response = Responses.Success,
            Model = new()
            {
                Account = authResponse.Model,
                TokenCollection = new()
                {
                    {
                        "identity", authResponse.Token
                    }
                },
                Profile = profile.Model
            },
            Token = token
        };

    }



    /// <summary>
    /// Iniciar sesión con el token.
    /// </summary>
    /// <param name="token">Token</param>
    [HttpGet("login/token")]
    public async Task<HttpReadOneResponse<AuthModel<ProfileModel>>> LoginToken([FromQuery] string token)
    {

        // Login en LIN el servidor.
        var response = await Access.Auth.Controllers.Authentication.Login(token);

        // Validar respuesta.
        if (response.Response != Responses.Success)
            return new(response.Response);


        // Obtener el perfil.
        var profile = await Data.Profiles.ReadByAccount(response.Model.Id);

        // Respuesta http.
        var httpResponse = new ReadOneResponse<AuthModel<ProfileModel>>()
        {
            Response = Responses.Success,
            Message = "Success"

        };

        // Validar.
        if (profile.Response == Responses.Success)
        {
            // Genera el token
            var tokenAcceso = Jwt.Generate(profile.Model);

            httpResponse.Token = tokenAcceso;
            httpResponse.Model.Profile = profile.Model;
        }

        httpResponse.Model.Account = response.Model;
        httpResponse.Model.TokenCollection = new()
        {
            {
                "identity", response.Token
            }
        };


        return httpResponse;

    }



    /// <summary>
    /// Buscar.
    /// </summary>
    /// <param name="pattern">Patron de búsqueda.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("search")]
    public async Task<HttpReadAllResponse<SessionModel<ProfileModel>>> Search([FromQuery] string pattern, [FromHeader] string token)
    {

        // Busca el acceso
        var accounts = await Access.Auth.Controllers.Account.Search(pattern, token);

        // Si no tiene acceso
        if (accounts.Response != Responses.Success)
            return new ReadAllResponse<SessionModel<ProfileModel>>
            {
                Response = Responses.Unauthorized,
                Message = "No tienes acceso a LIN Identity"
            };


        // Id de las cuentas.
        var mappedIds = accounts.Models.Select(T => T.Id).ToList();

        // Obtener los perfiles.
        var profiles = await Data.Profiles.ReadByAccounts(mappedIds);

        // Map.
        var final = from P in profiles.Models
                    join A in accounts.Models
                        on P.AccountId equals A.Id
                    select new SessionModel<ProfileModel>
                    {
                        Account = A,
                        Profile = P
                    };

        // Retorna el resultado
        return new ReadAllResponse<SessionModel<ProfileModel>>
        {
            Response = Responses.Success,
            Models = final.ToList()
        };

    }



}