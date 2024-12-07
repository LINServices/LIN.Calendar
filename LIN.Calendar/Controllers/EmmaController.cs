using LIN.Types.Cloud.OpenAssistant.Api;
using LIN.Types.Cloud.OpenAssistant.Models;
using System.Text;

namespace LIN.Calendar.Controllers;

[Route("[controller]")]
public class EmmaController : ControllerBase
{

    /// <summary>
    /// Respuesta de Emma al usuario.
    /// </summary>
    /// <param name="tokenAuth">Token de identity.</param>
    /// <param name="consult">Consulta del usuario.</param>
    [HttpPost]
    public async Task<HttpReadOneResponse<EmmaSchemaResponse>> Assistant([FromHeader] string tokenAuth, [FromBody] string consult)
    {

        // Cliente HTTP.
        HttpClient client = new();

        // Headers.
        client.DefaultRequestHeaders.Add("token", tokenAuth);
        client.DefaultRequestHeaders.Add("useDefaultContext", true.ToString().ToLower());

        // Modelo de Emma.
        var request = new AssistantRequest
        {
            App = "calendar",
            Prompt = consult
        };

        // Generar el string content.
        StringContent stringContent = new(Newtonsoft.Json.JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        // Solicitud HTTP.
        var result = await client.PostAsync("https://api.emma.linplatform.com/emma", stringContent);

        // Esperar respuesta.
        var response = await result.Content.ReadAsStringAsync();

        // Objeto.
        var assistantResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ReadOneResponse<EmmaSchemaResponse>>(response);

        // Respuesta
        return assistantResponse ?? new(Responses.Undefined);

    }


    /// <summary>
    /// Solicitud del servidor de Emma.
    /// </summary>
    /// <param name="tokenAuth">Token de acceso.</param>
    [HttpGet]
    public async Task<HttpReadOneResponse<object>> RequestFromEmma([FromHeader] string tokenAuth)
    {

        // Validar token.
        var response = await LIN.Access.Auth.Controllers.Authentication.Login(tokenAuth);


        if (response.Response != Responses.Success)
        {
            return new ReadOneResponse<object>()
            {
                Model = "Este usuario no autenticado en LIN Calendar."
            };
        }

        // 
        var profile = await Data.Profiles.ReadByAccount(response.Model.Id);


        if (profile.Response != Responses.Success)
        {
            return new ReadOneResponse<object>()
            {
                Model = "Este usuario no tiene una cuenta en LIN Calendar."
            };
        }


        // Eventos.
        var eve = await Data.Events.ReadAll(profile.Model.Id);    



        StringBuilder stringBuilder = new ();
        foreach(var e in eve.Models)
        {


            stringBuilder.AppendLine ($$""" 
                                      {
                                        "Descripci�n del evento" : "{{e.Nombre}}",
                                        "El evento es todo el dia" : "{{e.IsAllDay}}",
                                        "Hora de inicio" : "{{e.DateStart:MMMM-dd-yyyy HH:mm}}",
                                        "Hora de fin" : "{{e.EndStart:MMMM-dd-yyyy HH:mm}}",
                                        "Tipo de evento" : "{{e.Type.ToString()}}"
                                      }
                                      """);
        }




        string final = $$""""

                        Estos son los eventos que tiene el usuario:

                        recuerda que tienes que responder con la informaci�n que tienes a tu disposici�n:

                        {{stringBuilder?.ToString()}}

                        Estos son comandos:
                        
                        {
                          "name": "#say",
                          "description": "Utiliza esta funci�n para decirle algo al usuario como saludos o responder a preguntas.",
                          "example":"#say('Hola')",
                          "parameters": {
                            "properties": {
                              "content": {
                                "type": "string",
                                "description": "contenido"
                              }
                            }
                          }
                        }

                        {
                          "name": "#agendar",
                          "description": "Utiliza esta funci�n para agendar eventos.",
                          "example":"#agendar('Hola', '02-15-2006 20:45')",
                          "parameters": {
                            "properties": {
                              "content": {
                                "type": "string",
                                "description": "contenido"
                              },
                              "content": {
                                "type": "date",
                                "description": "fecha y hora en formato 'MM-DD-YYYY HH:MM'"
                              }
                            }
                          }
                        }

                        Si el usuario te pide agendar y el horario esta ocupado, debes contestar : "Ya tienes un evento para esa fecha"

                        El formato para responder con comandos es:
                        "#NombreDelComando(Propiedades en orden separados por coma si es necesario)"
                        
                        
                        IMPORTANTE:
                        No en todos los casos en necesario usar comandos, solo �salos cuando se cumpla la descripci�n.

                        NUNCA debes inventar comandos nuevos, solo puedes usar los que ya existen.

                        """";
        return new ReadOneResponse<object>()
        {
            Model = final,
            Response = Responses.Success
        };

    }

}