using Microsoft.EntityFrameworkCore;
using AppNetCoreWebAPIDAE2.Models;

namespace AppNetCoreWebAPIDAE2.Data
{
    public class DBContext: DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }

        public DbSet<eva_cat_edificios> eva_cat_edificios { get; set; }
        public DbSet<eva_cat_espacios> eva_cat_espacios { get; set; }
        public DbSet<eva_cat_tipo_competencias> eva_cat_tipo_competencias { get; set; }
        public DbSet<eva_cat_competencias> eva_cat_competencias { get; set; }
        public DbSet<eva_cat_conocimientos> eva_cat_conocimientos { get; set; }

    }
}
