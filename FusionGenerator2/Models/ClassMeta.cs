using System.Linq;
using System.Collections.Immutable;

namespace console;

public class ClassMeta
{
    public string Name { get; set; } = null!;
    public string ServiceName { get; set; } = null!;
    public string PluralName { get; set; } = null!;
    public string LowerPluralName { get; set; } = null!;
    public string LowerName { get; set; } = null!;
    public string Namespace { get; set; } = null!;
    public string Rootnamespace { get; set; } = null!;
    public PropertyMeta Primary { get; set; } = null!;
    public ImmutableArray<PropertyMeta> TypeParameters { get; set; }

    public ImmutableArray<PropertyMeta> StringProperties => TypeParameters.Where(p => p.Type.Contains("string")).ToImmutableArray();

    public ImmutableArray<PropertyMeta> SimpleProperties => TypeParameters.Where(p => !p.IsRelation && !p.Name.Equals("Id")).ToImmutableArray();

    public ImmutableArray<PropertyMeta> Relations => TypeParameters.Where(p => p.Type.Contains("List") || p.Type.Contains("Entity")).ToImmutableArray();
    public ImmutableArray<PropertyMeta>? UniqRelations => Relations.Length > 0 ? Relations.GroupBy(x => x.Entity)?.Select(g => g.First())?.ToImmutableArray() : null;

    public PropertyMeta FirstStringProp => StringProperties.First();

}



