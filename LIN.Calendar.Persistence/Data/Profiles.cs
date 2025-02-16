using LIN.Types.Calendar.Models;
using LIN.Types.Cloud.Identity.Abstracts;
using LIN.Types.Responses;
using Microsoft.EntityFrameworkCore;

namespace LIN.Calendar.Persistence.Data;

public class Profiles(Context.DataContext context)
{

    /// <summary>
    /// Crea un perfil.
    /// </summary>
    /// <param name="data">Modelo.</param>
    public async Task<ReadOneResponse<ProfileModel>> Create(AuthModel<ProfileModel> data)
    {
        // ID
        data.Profile.Id = 0;

        // Ejecución
        try
        {
            var res = context.Profiles.Add(data.Profile);
            await context.SaveChangesAsync();
            return new(Responses.Success, data.Profile);
        }
        catch (Exception)
        {
        }
        return new();
    }


    /// <summary>
    /// Obtiene un perfil
    /// </summary>
    /// <param name="id">ID del perfil</param>
    public async Task<ReadOneResponse<ProfileModel>> Read(int id)
    {
        // Ejecución
        try
        {
            var profile = await (from P in context.Profiles
                                 where P.Id == id
                                 select P).FirstOrDefaultAsync();

            return new(Responses.Success, profile ?? new());
        }
        catch (Exception)
        {
        }
        return new();
    }


    /// <summary>
    /// Obtiene un perfil por medio del ID de su cuenta.
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    public async Task<ReadOneResponse<ProfileModel>> ReadByAccount(int id)
    {

        // Ejecución
        try
        {
            // Consulta.
            var profile = await (from P in context.Profiles
                                 where P.AccountId == id
                                 select P).FirstOrDefaultAsync();

            // Si no existe.
            if (profile == null)
                return new(Responses.NotExistProfile);

            return new(Responses.Success, profile ?? new());
        }
        catch (Exception)
        {
        }
        return new();
    }


    /// <summary>
    /// Obtiene perfiles según los Id de las cuentas.
    /// </summary>
    /// <param name="ids">Lista de Ids.</param>
    public async Task<ReadAllResponse<ProfileModel>> ReadByAccounts(IEnumerable<int> ids)
    {
        // Ejecución
        try
        {

            var profile = await (from P in context.Profiles
                                 where ids.Contains(P.AccountId)
                                 select P).ToListAsync();

            if (profile == null)
                return new(Responses.NotExistProfile);

            return new(Responses.Success, profile ?? []);
        }
        catch (Exception)
        {
        }
        return new();
    }

}