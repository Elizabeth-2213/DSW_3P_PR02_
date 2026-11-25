using DSW_3P_PR02.Data;
using DSW_3P_PR02.Models;
using Microsoft.EntityFrameworkCore;

namespace DSW_3P_PR02.Repositories;

public class PersonajeSqlRepository(AppDbContext context) : IPersonajeRepository
{
    public List<Personaje> ObtenerTodos() =>
        context.Personajes.AsNoTracking().OrderBy(personaje => personaje.Id).ToList();

    public Personaje? ObtenerPorId(int id) =>
        context.Personajes.AsNoTracking().FirstOrDefault(personaje => personaje.Id == id);

    public Personaje Agregar(Personaje nuevo)
    {
        nuevo.Id = 0;
        context.Personajes.Add(nuevo);
        context.SaveChanges();
        return nuevo;
    }

    public bool Actualizar(int id, Personaje actualizado)
    {
        var actual = context.Personajes.Find(id);
        if (actual is null)
            return false;

        actualizado.Id = id;
        context.Entry(actual).CurrentValues.SetValues(actualizado);
        return GuardarCambios();
    }

    public bool Eliminar(int id)
    {
        var personaje = context.Personajes.Find(id);
        if (personaje is null)
            return false;

        context.Personajes.Remove(personaje);
        return GuardarCambios();
    }

    private bool GuardarCambios()
    {
        try
        {
            context.SaveChanges();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
    }
}
