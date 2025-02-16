using LIN.Calendar.Persistence.Context;
using LIN.Types.Enumerations;

namespace LIN.Calendar.Services;

public class Iam(DataContext context)
{

    /// <summary>
    /// Validar el acceso.
    /// </summary>
    /// <param name="profile">Id del perfil.</param>
    /// <param name="eventId">Id de la conversación.</param>
    public async Task<IamLevels> Validate(int profile, int eventId)
    {
        try
        {
            // Consulta.
            var have = await (from member in context.Guests
                              where member.ProfileId == profile
                              && member.EventId == eventId
                              select member).FirstOrDefaultAsync();

            // No existe.
            if (have == null)
                return IamLevels.NotAccess;

            // Visualizador.
            return IamLevels.Privileged;
        }
        catch(Exception)
        {
        }
        return IamLevels.NotAccess;
    }

}