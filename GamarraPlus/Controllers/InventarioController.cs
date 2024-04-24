using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GamarraPlus.Datos;
using GamarraPlus.Models;
using Newtonsoft.Json;

namespace GamarraPlus.Controllers
{
    [Authorize]
    public class InventarioController : Controller
    {
        DA_Producto _daProducto = new DA_Producto();
        DA_Categoria _daCategoria = new DA_Categoria();

        public IActionResult Productos()
        {
            return View();
        }

        public IActionResult Categorias()
        {
            // Llama al método ListaCategoria y espera su resultado
            var task = ListaCategoria();
            task.Wait();
            var result = task.Result;

            // Devuelve el resultado obtenido
            return result;
        }




        public async Task<IActionResult> ListaCategoria()
        {
            List<Categoria> lista;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7034/api/Inventario/categorias/");
                HttpResponseMessage response = await client.GetAsync("");
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    lista = JsonConvert.DeserializeObject<List<Categoria>>(apiResponse);
                }
                else
                {
                    lista = new List<Categoria>();
                }
            }
            return View(await Task.Run(() => lista));
        }









        

        [HttpGet]
        public JsonResult ListaProducto()
        {
            List<Producto> oLista = new List<Producto>();
            oLista = _daProducto.Listar();
            return Json(new { data = oLista });
        }
       


        [HttpPost]
        public JsonResult GuardarProducto([FromBody] Producto obj)
        {
            string operacion = Request.Headers["operacion"];
            bool respuesta;

            if (operacion == "crear")
            {
                respuesta = _daProducto.Guardar(obj);
            }
            else {
                respuesta = _daProducto.Editar(obj);
            }
            

            return Json(new { respuesta = respuesta });
        }

        [HttpDelete]
        public JsonResult EliminarProducto(int idproducto)
        {
            bool respuesta;
            respuesta = _daProducto.Eliminar(idproducto);
            return Json(new { respuesta = respuesta });
        }

        /*
        [HttpGet]
        public JsonResult ListaCategoria()
        {
            List<Categoria> oLista = new List<Categoria>();
            oLista = _daCategoria.Listar();
            return Json(new { data = oLista });
        }
        */

        [HttpPost]
        public JsonResult GuardarCategoria([FromBody] Categoria obj)
        {
            string operacion = Request.Headers["operacion"];
            bool respuesta;

            if (operacion == "crear")
            {
                respuesta = _daCategoria.Guardar(obj);
            }
            else
            {
                respuesta = _daCategoria.Editar(obj);
            }


            return Json(new { respuesta = respuesta });
        }

        [HttpDelete]
        public JsonResult EliminarCategoria(int idcategoria)
        {
            bool respuesta;
            respuesta = _daCategoria.Eliminar(idcategoria);
            return Json(new { respuesta = respuesta });
        }

    }
}
