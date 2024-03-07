using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MvcCoreEmpleadosSession.Extensions;
using MvcCoreEmpleadosSession.Models;
using MvcCoreEmpleadosSession.Repositories;

namespace MvcCoreEmpleadosSession.Controllers
{
    public class EmpleadosController : Controller
    {
        private RepositoryEmpleados repo;
        private IMemoryCache memoryCache;

        public EmpleadosController(RepositoryEmpleados repo, IMemoryCache memoryCache)
        {
            this.repo = repo;
            this.memoryCache = memoryCache;
        }

        public async Task<IActionResult> EmpleadosFavoritos(int? ideliminar)
        {
            if (ideliminar != null)
            {
                List<Empleado> empleados =
                    this.memoryCache.Get<List<Empleado>>("FAVORITOS");
                if (empleados != null)
                {
                    Empleado emp = empleados.Find(e => e.IdEmpleado == ideliminar);
                    empleados.Remove(emp);
                    if (empleados.Count() == 0)
                    {
                        this.memoryCache.Remove("FAVORITOS");
                    }
                    else
                    {
                        this.memoryCache.Set("FAVORITOS", empleados);
                    }
                }
            }
            return View();
        }

        public async Task<IActionResult> SessionEmpleadosOk
            (int? idempleado, int? idfavorito)
        {
            if (idempleado != null)
            {
                // Almacenamos lo mínimo, colección int
                List<int> idsEmpleados;
                if (HttpContext.Session.GetString("IDSEMPLEADOS") == null)
                {
                    idsEmpleados = new List<int>();
                }
                else
                {
                    idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }
                idsEmpleados.Add(idempleado.Value);
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                ViewData["MENSAJE"] = "Empleados almacenados: " + idsEmpleados.Count();
            }
            if (idfavorito != null)
            {
                List<Empleado> empleadosFavoritos;
                if (this.memoryCache.Get("FAVORITOS") == null)
                {
                    empleadosFavoritos = new List<Empleado>();
                }
                else
                {
                    empleadosFavoritos =
                        this.memoryCache.Get<List<Empleado>>("FAVORITOS");
                }
                Empleado empleado =
                    await this.repo.FindEmpleadoAsync(idfavorito.Value);
                empleadosFavoritos.Add(empleado);
                this.memoryCache.Set("FAVORITOS", empleadosFavoritos);
            }
            List<int> ids = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            // Si queremos que se borren del listado al añadirlos al Session
            /*
            if (ids == null)
            {
                List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
                return View(empleados);
            }
            else
            {
                List<Empleado> empleados =
                    await this.repo.GetEmpleadosNotInSessionAsync(ids);
                return View(empleados);
            }
            */
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        [ResponseCache(Duration = 80, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> EmpleadosAlmacenadosOk(int? ideliminar)
        {
            List<int> idsEmpleados =
                HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (idsEmpleados != null)
            {
                // Eliminar de Session
                if (ideliminar != null)
                {
                    idsEmpleados.Remove(ideliminar.Value);
                    // Debemos preguntar si ya no tenemos empleados en la colección
                    if (idsEmpleados.Count() == 0)
                    {
                        HttpContext.Session.Remove("IDSEMPLEADOS");
                    }
                    else
                    {
                        HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                    }
                }
                List<Empleado> empleados =
                    await this.repo.GetEmpleadosSessionAsync(idsEmpleados);
                return View(empleados);
            }
            return View();
        }





        public async Task<IActionResult> SessionEmpleados(int? idEmpleado)
        {
            if (idEmpleado != null)
            {
                Empleado empleado =
                    await this.repo.FindEmpleadoAsync(idEmpleado.Value);
                List<Empleado> empleadosList;
                if (HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS") != null)
                {
                    empleadosList =
                        HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS");
                }
                else
                {
                    empleadosList = new List<Empleado>();
                }
                empleadosList.Add(empleado);
                HttpContext.Session.SetObject("EMPLEADOS", empleadosList);
                ViewData["MENSAJE"] = "Empleado " + empleado.Apellido + " almacenado";
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public IActionResult EmpleadosAlmacenados()
        {
            return View();
        }

        public async Task<IActionResult> SessionSalarios(int? salario)
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
