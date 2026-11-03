namespace NavegadorHistorial;

/// <summary>
/// Pila dinámica (LIFO) implementada con nodos enlazados. No tiene capacidad fija:
/// cada Push crea un nodo que apunta al que antes era la cima.
/// </summary>
public class PilaEnlazada<T>
{
    private sealed class Nodo
    {
        public T Dato;
        public Nodo? Debajo;
        public Nodo(T dato, Nodo? debajo) { Dato = dato; Debajo = debajo; }
    }

    private Nodo? cima;

    public int Cantidad { get; private set; }
    public bool IsEmpty => cima is null;

    public void Push(T dato)
    {
        cima = new Nodo(dato, cima);
        Cantidad++;
    }

    public T Pop()
    {
        if (cima is null) throw new InvalidOperationException("La pila está vacía.");
        T dato = cima.Dato;
        cima = cima.Debajo;
        Cantidad--;
        return dato;
    }

    public T Peek()
    {
        if (cima is null) throw new InvalidOperationException("La pila está vacía.");
        return cima.Dato;
    }

    /// <summary>Vacía la pila: los nodos quedan sin referencias y el recolector de basura los libera.</summary>
    public void Limpiar()
    {
        cima = null;
        Cantidad = 0;
    }

    /// <summary>Recorre desde la cima hacia el fondo (solo lectura, para mostrar en pantalla).</summary>
    public IEnumerable<T> DesdeLaCima()
    {
        for (var n = cima; n is not null; n = n.Debajo) yield return n.Dato;
    }
}