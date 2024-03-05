using Microsoft.EntityFrameworkCore;
using MvcCoreEmpleadosSession.Context;
using MvcCoreEmpleadosSession.Models;

namespace MvcCoreEmpleadosSession.Repositories
{
    public class RepositoryEmpleados
    {
        private EmpleadosContext context;

        public RepositoryEmpleados(EmpleadosContext context)
        {
            this.context = context;
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            return await this.context.Empleados.ToListAsync();
        }

        public async Task<Empleado> FindEmpleadoAsync(int id)
        {
            return await this.context.Empleados
                .FirstOrDefaultAsync(e => e.IdEmpleado == id);
        }
    }
}
