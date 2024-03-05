using Microsoft.AspNetCore.Mvc;
using MvcCoreEmpleadosSession.Models;
using MvcCoreEmpleadosSession.Repositories;

namespace MvcCoreEmpleadosSession.Controllers
{
    public class EmpleadosController : Controller
    {
        private RepositoryEmpleados repo;

        public EmpleadosController(RepositoryEmpleados repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> SessionSalarios
            (int? salario)
        {
            if (salario != null)
            {
                int sumaSalarial = 0;
                if (HttpContext.Session.GetString("SUMASALARIAL") != null)
                {
                    sumaSalarial = int.Parse
                        (HttpContext.Session.GetString("SUMASALARIAL"));
                }
                sumaSalarial += salario.Value;
                HttpContext.Session
                    .SetString("SUMASALARIAL", sumaSalarial.ToString());
                ViewData["MENSAJE"] = "Salario almacenado: " + salario.Value;
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public IActionResult SumaSalarios()
        {
            return View();
        }
    }
}
