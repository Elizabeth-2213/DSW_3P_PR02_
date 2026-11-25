using DSW_3P_PR02.Models;

namespace DSW_3P_PR02.Repositories;

public interface IPersonajeRepository
{
    List<Personaje> ObtenerTodos();
    Personaje? ObtenerPorId(int id);
    Personaje Agregar(Personaje nuevo);
    bool Actualizar(int id, Personaje actualizado);
    bool Eliminar(int id);
}
