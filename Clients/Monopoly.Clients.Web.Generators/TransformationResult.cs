using Microsoft.CodeAnalysis;

namespace Monopoly.Clients.Web.Generators;

public class TransformationResult(
    string name,
    string @namespace,
    string requestInterfaceName,
    IMethodSymbol[] requestMethods,
    IMethodSymbol[] responseMethods)
    : IEquatable<TransformationResult>
{
    public string Name { get; } = name;
    public string Namespace { get; } = @namespace;
    public string RequestInterfaceName { get; } = requestInterfaceName;
    public IMethodSymbol[] RequestMethods { get; } = requestMethods;
    public IMethodSymbol[] ResponseMethods { get; } = responseMethods;
 
    public bool Equals(TransformationResult? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name
               && Namespace == other.Namespace
               && RequestInterfaceName == other.RequestInterfaceName
               && CompareIMethodSymbol(RequestMethods, other.RequestMethods)
               && CompareIMethodSymbol(ResponseMethods, other.ResponseMethods);
    }
 
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((TransformationResult)obj);
    }
         
    private static bool CompareIMethodSymbol(ReadOnlySpan<IMethodSymbol> x, ReadOnlySpan<IMethodSymbol> y)
    {
        if (x.Length != y.Length)
        {
            return false;
        }
 
        for (var i = 0; i < x.Length; i++)
        {
            if (new MethodSymbolComparer().Equals(x[i], y[i]) == false)
            {
                return false;
            }
        }
 
        return true;
    }
 
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Name.GetHashCode();
            hashCode = (hashCode * 397) ^ Namespace.GetHashCode();
            hashCode = (hashCode * 397) ^ RequestInterfaceName.GetHashCode();
            hashCode = (hashCode * 397) ^ RequestMethods.GetHashCode();
            hashCode = (hashCode * 397) ^ ResponseMethods.GetHashCode();
            return hashCode;
        }
    }
         
    private class MethodSymbolComparer : IEqualityComparer<IMethodSymbol>
    {
        public bool Equals(IMethodSymbol x, IMethodSymbol y)
        {
            return x.Name == y.Name
                   && x.Parameters.SequenceEqual(y.Parameters, new ParameterSymbolComparer());
        }
 
        public int GetHashCode(IMethodSymbol obj)
        {
            return obj.Name.GetHashCode();
        }
    }
         
    private class ParameterSymbolComparer : IEqualityComparer<IParameterSymbol>
    {
        public bool Equals(IParameterSymbol x, IParameterSymbol y)
        {
            return x.Type.ToString() == y.Type.ToString() && x.Name == y.Name;
        }
 
        public int GetHashCode(IParameterSymbol obj)
        {
            return obj.Type.ToString().GetHashCode();
        }
    }
}