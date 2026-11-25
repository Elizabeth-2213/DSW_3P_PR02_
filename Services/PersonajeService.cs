using DSW_3P_PR02.Models;
using DSW_3P_PR02.Repositories;

namespace DSW_3P_PR02.Services;

public class PersonajeService(IPersonajeRepository repository) : IPersonajeService
{
    public List<Personaje> ObtenerTodos() => repository.ObtenerTodos();

    public Personaje? ObtenerPorId(int id) => repository.ObtenerPorId(id);

    public List<Personaje> ObtenerPorVideojuego(string nombre) =>
        repository.ObtenerTodos()
            .Where(personaje => string.Equals(personaje.Videojuego, nombre.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToList();

    public List<Personaje> ObtenerPorNivel(int nivel) =>
        repository.ObtenerTodos().Where(personaje => personaje.Nivel >= nivel).ToList();

    public Personaje Agregar(Personaje nuevo)
    {
        Validar(nuevo);
        nuevo.Disponible = true;
        return repository.Agregar(nuevo);
    }

    public bool Actualizar(int id, Personaje actualizado)
    {
        Validar(actualizado);
        return repository.Actualizar(id, actualizado);
    }

    public bool Eliminar(int id) => repository.Eliminar(id);

    private static void Validar(Personaje personaje)
    {
        if (string.IsNullOrWhiteSpace(personaje.Nombre))
            throw new ArgumentException("El nombre del personaje es obligatorio.");
        if (string.IsNullOrWhiteSpace(personaje.Videojuego))
            throw new ArgumentException("El videojuego es obligatorio.");
        if (string.IsNullOrWhiteSpace(personaje.Clase))
            throw new ArgumentException("La clase es obligatoria.");
        if (personaje.Nivel is < 1 or > 100)
            throw new ArgumentException("El nivel debe estar entre 1 y 100.");
        if (personaje.Poder <= 0)
            throw new ArgumentException("El poder debe ser mayor que cero.");

        var imagen = personaje.ImagenUrl?.Trim();
        if (!Uri.TryCreate(imagen, UriKind.Absolute, out var url) ||
            !url.IsWellFormedOriginalString() ||
            url.Scheme is not ("http" or "https") ||
            string.IsNullOrEmpty(url.Host))
        {
            throw new ArgumentException("ImagenUrl debe ser una URL absoluta HTTP o HTTPS.");
        }

        personaje.Nombre = personaje.Nombre.Trim();
        personaje.Videojuego = personaje.Videojuego.Trim();
        personaje.Clase = personaje.Clase.Trim();
        personaje.ImagenUrl = imagen!;
    }
}
