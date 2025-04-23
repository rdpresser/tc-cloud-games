namespace TC.CloudGames.Domain.Abstractions;

public abstract class ValueObject
{
    //protected abstract IEnumerable<object> GetEqualityComponents();

    public abstract override bool Equals(object? obj);

    public abstract override int GetHashCode();
    
    // public override bool Equals(object obj)
    // {
    //     if (obj == null || obj.GetType() != GetType())
    //         return false;
    //
    //     var other = (ValueObject)obj;
    //     var thisComponents = GetEqualityComponents().GetEnumerator();
    //     var otherComponents = other.GetEqualityComponents().GetEnumerator();
    //
    //     while (thisComponents.MoveNext() && otherComponents.MoveNext())
    //     {
    //         if (thisComponents.Current == null ^ otherComponents.Current == null)
    //             return false;
    //
    //         if (thisComponents.Current != null && !thisComponents.Current.Equals(otherComponents.Current))
    //             return false;
    //     }
    //
    //     return !thisComponents.MoveNext() && !otherComponents.MoveNext();
    // }

    // public override int GetHashCode()
    // {
    //     var hashCode = new HashCode();
    //     foreach (var component in GetEqualityComponents())
    //     {
    //         hashCode.Add(component);
    //     }
    //     return hashCode.ToHashCode();
    // }

    public static bool operator ==(ValueObject left, ValueObject right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(ValueObject left, ValueObject right)
    {
        return !(left == right);
    }
}
