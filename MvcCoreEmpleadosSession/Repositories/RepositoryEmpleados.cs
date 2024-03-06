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

        public async Task<List<Empleado>> GetEmpleadosSessionAsync(List<int> ids)
        {
            // Para realizar un in dentro de LINQ, debemos hacerlo con
            // Collection.Contains(dato a buscar)
            // SELECT * FROM EMP WHERE EMP_NO IN (7777,8888,9999)
            var consulta = from datos in this.context.Empleados
                           where ids.Contains(datos.IdEmpleado)
                           select datos;
            if (consulta.Count() == 0)
            {
                return null;
            }
            return await consulta.ToListAsync();
        }

        public async Task<List<Empleado>> GetEmpleadosNotInSessionAsync(List<int> ids)
        {
            var consulta = from datos in this.context.Empleados
                           where ids.Contains(datos.IdEmpleado) == false
                           select datos;
            if (consulta.Count() == 0)
            {
                return null;
            }
            return await consulta.ToListAsync();
        }
    }
}
