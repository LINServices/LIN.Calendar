using LIN.Types.Enumerations;

namespace LIN.Calendar.Services;

public class Iam
{

    /// <summary>
    /// Validar el acceso.
    /// </summary>
    /// <param name="profile">Id del perfil.</param>
    /// <param name="eventId">Id de la conversación.</param>
    public async static Task<IamLevels> Validate(int profile, int eventId)
    {
        try
        {

            // Contexto de conexión a la bd.
            var (context, contextKey) = Conexión.GetOneConnection();

            // Consulta.
            var have = await (from member in context.DataBase.Guests
                              where member.ProfileId == profile
                              && member.EventId == eventId
                              select member).FirstOrDefaultAsync();

            // Cerrar la conexión.
            context.CloseActions(contextKey);

            // No existe.
            if (have == null)
                return IamLevels.NotAccess;

            // Visualizador.
            return IamLevels.Privileged;
        }
        catch
        {
        }
        return IamLevels.NotAccess;
    }

}