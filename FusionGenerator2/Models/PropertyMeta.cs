using console.Pluralization;
using System.Collections.Immutable;
using System.Net.Http.Headers;
namespace console;

public class PropertyMeta
{
    public string Name { get; set; } = null!;
    public string PluralName => Pluralizer.Pluralize(Name);
    public string PluralType => Pluralizer.Pluralize(Entity.Replace("Entity",""));
    public string Entity => Type.Contains("List") ? Type.Replace("List<", "").Replace(">", "").Replace("?", "") : Type.Replace("?", "");
    public bool IsArryRelation => Type.Contains("Entity") && Type.Contains("List");
    public bool IsRelation => Type.Contains("Entity");
    public ImmutableArray<ClassMeta> Metas { get; set; }
    public string? GetFirstPropertyName => Metas.FirstOrDefault(x => x.Name.Equals(Entity))?.FirstStringProp.Name;

    public string Type { get; set; } = null!;
    public bool IsPrimary { get; set; }
}



