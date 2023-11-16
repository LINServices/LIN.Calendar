﻿namespace LIN.Contacts.Memory;


public class Sessions : Dictionary<int, MemorySession>
{

    /// <summary>
    /// Obtiene una session
    /// </summary>
    /// <param name="profile">ID del perfil</param>
    public new MemorySession? this[int profile]
    {
        get
        {
            var session = this.Where(T => T.Key == profile).FirstOrDefault();
            return session.Value;
        }
    }




    /// <summary>
    /// Agrega una nueva sesión
    /// </summary>
    /// <param name="session">Modelo</param>
    public void Add(MemorySession session)
    {
        Add(session.Profile.Id, session);
    }


}