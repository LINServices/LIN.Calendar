using LIN.Types.Emma.Models;

namespace LIN.Contacts.Controllers;


[Route("Emma")]
public class EmmaController : ControllerBase
{


    /// <summary>
    /// Emma IA.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="consult">Prompt.</param>
    [HttpPost]
    public async Task<HttpReadOneResponse<ResponseIAModel>> ReadAll([FromHeader] string token, [FromBody] string consult)
    {
        return new()
        {
            Message = "No disponible.",
            Response = Responses.Unauthorized
        };
    }


}