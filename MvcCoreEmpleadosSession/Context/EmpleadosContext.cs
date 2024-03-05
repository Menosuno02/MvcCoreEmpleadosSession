using Microsoft.EntityFrameworkCore;
using MvcCoreEmpleadosSession.Models;

namespace MvcCoreEmpleadosSession.Context
{
    public class EmpleadosContext : DbContext
    {
        public EmpleadosContext(DbContextOptions<EmpleadosContext> options)
            : base(options) { }

        public DbSet<Empleado> Empleados { get; set; }
    }
}
