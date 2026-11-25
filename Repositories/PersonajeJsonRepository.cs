using System.Text.Json;
using DSW_3P_PR02.Models;

namespace DSW_3P_PR02.Repositories;

public class PersonajeJsonRepository : IPersonajeRepository
{
    private readonly string _ruta;
    private readonly object _candado = new();
    private readonly JsonSerializerOptions _opciones = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public PersonajeJsonRepository(IWebHostEnvironment environment)
    {
        var directorio = Path.Combine(environment.ContentRootPath, "Data");
        Directory.CreateDirectory(directorio);
        _ruta = Path.Combine(directorio, "personajes.json");
        if (!File.Exists(_ruta))
            File.WriteAllText(_ruta, "[]");
    }

    public List<Personaje> ObtenerTodos()
    {
        lock (_candado)
            return Leer();
    }

    public Personaje? ObtenerPorId(int id)
    {
        lock (_candado)
            return Leer().FirstOrDefault(personaje => personaje.Id == id);
    }

    public Personaje Agregar(Personaje nuevo)
    {
        lock (_candado)
        {
            var personajes = Leer();
            nuevo.Id = personajes.Count == 0 ? 1 : checked(personajes.Max(personaje => personaje.Id) + 1);
            personajes.Add(nuevo);
            Guardar(personajes);
            return nuevo;
        }
    }

    public bool Actualizar(int id, Personaje actualizado)
    {
        lock (_candado)
        {
            var personajes = Leer();
            var indice = personajes.FindIndex(personaje => personaje.Id == id);
            if (indice < 0)
                return false;

            actualizado.Id = id;
            personajes[indice] = actualizado;
            Guardar(personajes);
            return true;
        }
    }

    public bool Eliminar(int id)
    {
        lock (_candado)
        {
            var personajes = Leer();
            if (personajes.RemoveAll(personaje => personaje.Id == id) == 0)
                return false;

            Guardar(personajes);
            return true;
        }
    }

    private List<Personaje> Leer() =>
        JsonSerializer.Deserialize<List<Personaje>>(File.ReadAllText(_ruta), _opciones)
        ?? throw new JsonException("personajes.json debe contener un arreglo JSON.");

    private void Guardar(List<Personaje> personajes)
    {
        var temporal = _ruta + ".tmp";
        File.WriteAllText(temporal, JsonSerializer.Serialize(personajes, _opciones));
        File.Move(temporal, _ruta, overwrite: true);
    }
}
