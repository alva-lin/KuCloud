using KuCloud.Core.Domains.StorageAggregate;
using Microsoft.EntityFrameworkCore;

namespace KuCloud.Infrastructure.Data;

public sealed partial class AppDbContext
{
    public DbSet<Folder> Folders => Set<Folder>();

    public DbSet<FileNode> FileNodes => Set<FileNode>();
}
