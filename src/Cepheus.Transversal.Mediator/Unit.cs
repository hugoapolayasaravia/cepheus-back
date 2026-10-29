namespace MediatR;

/// <summary>
/// Representa la ausencia de un valor de retorno (equivalente a "void" pero
/// utilizable como parámetro de tipo genérico, ej. IRequestHandler&lt;T, Unit&gt;).
/// Mismo mecanismo que usa MediatR internamente.
/// </summary>
public readonly struct Unit : IEquatable<Unit>
{
    public static readonly Unit Value = new();

    public bool Equals(Unit other) => true;
    public override bool Equals(object? obj) => obj is Unit;
    public override int GetHashCode() => 0;
    public override string ToString() => "()";

    public static bool operator ==(Unit left, Unit right) => true;
    public static bool operator !=(Unit left, Unit right) => false;
}
