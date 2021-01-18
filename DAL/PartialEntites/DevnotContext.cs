using DAL.Entities.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.PartialEntites
{
    //Scaffold-DBContext "Server=.;Database=Devnot2021;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Entities -ContextDir "Entities\DbContexts" -Context Devnot2021Context  -Force -Project DAL -StartupProject DAL
    public class DevnotContext : Devnot2021Context
    {

        public DevnotContext()
        {
        }
        public DevnotContext(DbContextOptions<Devnot2021Context> options)
             : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            base.OnConfiguring(optionsBuilder);
            base.OnConfiguring(optionsBuilder.UseLoggerFactory(DevnotLoggerFactory));
#endif
#if RELEASE
                base.OnConfiguring(optionsBuilder);
#endif
        }

        /* 
            Yazdığımız entity`lerin log'unun yazılması. 
            Linq `ların debug moddayken sql execution plan scriptini görmemizi sağlıyor. 
        */
        public static readonly ILoggerFactory DevnotLoggerFactory
         = LoggerFactory.Create(builder =>
         {
             builder
                 .AddFilter((category, level) =>
                     category == DbLoggerCategory.Database.Command.Name
                     && level == LogLevel.Information)
                 .AddDebug();
         });
    }
}
