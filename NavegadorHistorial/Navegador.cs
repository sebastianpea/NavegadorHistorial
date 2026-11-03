namespace NavegadorHistorial;

/// <summary>Una página del sitio simulado.</summary>
public record Pagina(string Url, string Titulo, string Contenido, string[] Enlaces);

/// <summary>
/// Lógica del historial de navegación con dos pilas:
/// "atrás" guarda las páginas anteriores y "adelante" las que se dejaron al retroceder.
/// </summary>
public class Navegador
{
    private readonly PilaEnlazada<string> atras = new();
    private readonly PilaEnlazada<string> adelante = new();

    public string? Actual { get; private set; }
    public bool PuedeRetroceder => !atras.IsEmpty;
    public bool PuedeAvanzar => !adelante.IsEmpty;
    public IEnumerable<string> PilaAtras => atras.DesdeLaCima();
    public IEnumerable<string> PilaAdelante => adelante.DesdeLaCima();

    /// <summary>Visitar una página nueva: la actual pasa a "atrás" y "adelante" se vacía.</summary>
    public void Visitar(string url)
    {
        if (url == Actual) return;          // recargar la misma página no cambia el historial
        if (Actual is not null) atras.Push(Actual);
        adelante.Limpiar();
        Actual = url;
    }

    /// <summary>Botón Atrás: la actual pasa a "adelante" y se saca la cima de "atrás".</summary>
    public void Retroceder()
    {
        if (!PuedeRetroceder) throw new InvalidOperationException("No hay páginas anteriores.");
        adelante.Push(Actual!);
        Actual = atras.Pop();
    }

    /// <summary>Botón Adelante: la actual pasa a "atrás" y se saca la cima de "adelante".</summary>
    public void Avanzar()
    {
        if (!PuedeAvanzar) throw new InvalidOperationException("No hay páginas siguientes.");
        atras.Push(Actual!);
        Actual = adelante.Pop();
    }
}

/// <summary>Sitio web ficticio para la simulación (no se usa Internet).</summary>
public static class SitioSimulado
{
    private static readonly Dictionary<string, Pagina> Paginas = new()
    {
        ["itsm.edu/inicio"] = new("itsm.edu/inicio", "Inicio – Instituto Tecnológico", "Bienvenido al portal del instituto. Elige una sección.", new[] { "itsm.edu/carreras", "itsm.edu/noticias", "itsm.edu/contacto" }),
        ["itsm.edu/carreras"] = new("itsm.edu/carreras", "Oferta educativa", "Conoce las ingenierías y licenciaturas que ofrecemos.", new[] { "itsm.edu/carreras/informatica", "itsm.edu/carreras/sistemas", "itsm.edu/inicio" }),
        ["itsm.edu/carreras/informatica"] = new("itsm.edu/carreras/informatica", "Ingeniería en Informática", "Plan de estudios, perfil de egreso y materias como Estructura de Datos.", new[] { "itsm.edu/materias/estructura-de-datos", "itsm.edu/carreras" }),
        ["itsm.edu/carreras/sistemas"] = new("itsm.edu/carreras/sistemas", "Ingeniería en Sistemas Computacionales", "Plan de estudios y especialidades.", new[] { "itsm.edu/carreras" }),
        ["itsm.edu/materias/estructura-de-datos"] = new("itsm.edu/materias/estructura-de-datos", "Estructura de Datos", "Unidad 3: estructuras lineales. Pilas, colas y listas.", new[] { "itsm.edu/carreras/informatica", "itsm.edu/inicio" }),
        ["itsm.edu/noticias"] = new("itsm.edu/noticias", "Noticias", "Abierta la convocatoria de inscripciones.", new[] { "itsm.edu/inicio", "itsm.edu/contacto" }),
        ["itsm.edu/contacto"] = new("itsm.edu/contacto", "Contacto", "Teléfono, correo y ubicación del instituto.", new[] { "itsm.edu/inicio" }),
    };

    public const string PaginaInicial = "itsm.edu/inicio";

    public static Pagina Obtener(string url) =>
        Paginas.TryGetValue(url.Trim().ToLowerInvariant(), out var p)
            ? p
            : new Pagina(url, "404 – Página no encontrada", $"La dirección \"{url}\" no existe en el sitio simulado.", new[] { PaginaInicial });
}