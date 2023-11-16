﻿namespace LIN.Contacts.Data;


public class Contacts
{



    #region Abstracciones


    /// <summary>
    /// Crea un contacto.
    /// </summary>
    /// <param name="data">Modelo.</param>
    public static async Task<CreateResponse> Create(ContactModel data)
    {

        // Contexto
        (var context, var connectionKey) = Conexión.GetOneConnection();

        // respuesta
        var response = await Create(data, context);

        context.CloseActions(connectionKey);

        return response;

    }



    /// <summary>
    /// Obtiene un contacto
    /// </summary>
    /// <param name="id">ID del contacto</param>
    public static async Task<ReadOneResponse<ContactModel>> Read(int id)
    {

        // Contexto
        (var context, var connectionKey) = Conexión.GetOneConnection();

        // respuesta
        var response = await Read(id, context);

        context.CloseActions(connectionKey);

        return response;

    }



    /// <summary>
    /// Obtiene los contactos asociados a un perfil
    /// </summary>
    /// <param name="id">ID del perfil</param>
    public static async Task<ReadAllResponse<ContactModel>> ReadAll(int id)
    {

        // Contexto
        (var context, var connectionKey) = Conexión.GetOneConnection();

        // respuesta
        var response = await ReadAll(id, context);

        context.CloseActions(connectionKey);

        return response;

    }


    #endregion




    /// <summary>
    /// Crea un contacto.
    /// </summary>
    /// <param name="data">Modelo.</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<CreateResponse> Create(ContactModel data, Conexión context)
    {
        // Ejecución
        try
        {

            // Establecer el perfil.
            foreach (var e in data.Mails)
                e.Profile = data.Im;

            // Establecer el perfil.
            foreach (var e in data.Phones)
                e.Profile = data.Im;

            // El usuario ya existe.
            context.DataBase.Attach(data.Im);

            var res = context.DataBase.Contacts.Add(data);
            await context.DataBase.SaveChangesAsync();
            return new(Responses.Success, data.Id);
        }
        catch
        {
        }
        return new();
    }



    /// <summary>
    /// Obtiene un contacto
    /// </summary>
    /// <param name="id">ID del contacto</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<ReadOneResponse<ContactModel>> Read(int id, Conexión context)
    {


        // Ejecución
        try
        {

            var profile = await (from P in context.DataBase.Contacts
                                 where P.Id == id
                                 select new ContactModel
                                 {
                                     Picture = P.Picture,
                                     Birthday = P.Birthday,
                                     Id = P.Id,
                                     Mails = P.Mails,
                                     Nombre = P.Nombre,
                                     Type = P.Type,
                                     Phones = P.Phones
                                 }).FirstOrDefaultAsync();

            return new(Responses.Success, profile ?? new());
        }
        catch
        {
        }
        return new();
    }



    /// <summary>
    /// Obtiene los contactos asociados a un perfil.
    /// </summary>
    /// <param name="id">ID del perfil.</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<ReadAllResponse<ContactModel>> ReadAll(int id, Conexión context)
    {


        // Ejecución
        try
        {

            // Query de contactos
            var contacts = await (from contact in context.DataBase.Contacts
                                  where contact.Im.Id == id
                                  orderby contact.Nombre
                                  select new ContactModel
                                  {
                                      Picture = contact.Picture,
                                      Birthday = contact.Birthday,
                                      Id = contact.Id,
                                      Mails = contact.Mails,
                                      Nombre = contact.Nombre,
                                      Type = contact.Type,
                                      Phones = contact.Phones
                                  }).ToListAsync();

            return new(Responses.Success, contacts);
        }
        catch
        {
        }
        return new();
    }



}