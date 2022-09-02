using System.Collections.Immutable;
using System.Text;
using static console.GlobalOptions;

namespace console;

public partial class TGenerate
{
    public static void DbContext(ImmutableArray<ClassMeta> metas)
    {
        // if (!CanOverrideFile && File.Exists(DbContextPath)) return;

        StringBuilder db = new();
        db.AppendLine($@"using Microsoft.EntityFrameworkCore;");
        db.AppendLine($@"using {RootNamespace}.Shared;");
        db.AppendLine($@"using Stl.Fusion.EntityFramework;");
        db.AppendLine();
        db.AppendLine($@"namespace {RootNamespace}.Services;");
        db.AppendLine($@"public partial class AppDbContext : DbContextBase");
        db.AppendLine($@"{{");
        foreach (var meta in metas)
        {
            db.AppendLine($@"    public DbSet<{meta.Name}> {meta.PluralName} {{get; protected set; }} = null!;");
            db.AppendLine();
        }

        db.AppendLine($@"}}");

        File.WriteAllText(DbContextPath, db.ToString());
    }
}