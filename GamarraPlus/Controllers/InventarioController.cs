using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GamarraPlus.Datos;
using GamarraPlus.Models;
using Newtonsoft.Json;
using System.Text;

namespace GamarraPlus.Controllers
{
    [Authorize]
    public class InventarioController : Controller
    {
        DA_Producto _daProducto = new DA_Producto();
        DA_Categoria _daCategoria = new DA_Categoria();

        public IActionResult Productos()
        {
            // Llama al método ListaProducto y espera su resultado
            var task = ListaProducto();
            task.Wait();
            var result = task.Result;

            // Devuelve el resultado obtenido
            return result;
        }

        public async Task<IActionResult> ListaProducto()
        {
            List<Producto> lista;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7034/api/Inventario/productos/");
                HttpResponseMessage response = await client.GetAsync("");
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    lista = JsonConvert.DeserializeObject<List<Producto>>(apiResponse);
                }
                else
                {
                    lista = new List<Producto>();
                }
            }

            // Obtener la lista de categorías y almacenarla en ViewBag
            ViewBag.Categorias = await ListaCategoria();

            return View(lista);
        }


        [HttpPost]
        public async Task<JsonResult> GuardarProducto([FromBody] Producto obj)
        {
            bool respuesta;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7034/api/Inventario/");

                var jsonProducto = JsonConvert.SerializeObject(obj);

                var content = new StringContent(jsonProducto, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("productos", content);

                if (response.IsSuccessStatusCode)
                {
                    respuesta = true;
                }
                else
                {
                    respuesta = false;
                    Console.WriteLine("Error al guardar el producto. Código de estado: " + response.StatusCode);
                }
            }

            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> ActualizarProducto([FromBody] Producto obj)
        {
            bool respuesta;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7034/api/Inventario/");
                    HttpResponseMessage response = await client.PutAsync($"productos/{obj.IdProducto}", new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        respuesta = true;
                    }
                    else
                    {
                        respuesta = false;
                        Console.WriteLine("Error al actualizar el producto. Código de estado: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar el producto: " + ex.Message);
                respuesta = false;
            }

            return Json(new { respuesta = respuesta });
        }

        [HttpDelete]
        public async Task<JsonResult> EliminarProducto(int idProducto)
        {
            bool respuesta;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7034/api/Inventario/");
                HttpResponseMessage response = await client.DeleteAsync($"productos/{idProducto}");
                if (response.IsSuccessStatusCode)
                {
                    respuesta = true;
                }
                else
                {
                    respuesta = false;
                    Console.WriteLine("Error al eliminar el producto. Código de estado: " + response.StatusCode);
                }
            }

            return Json(new { respuesta = respuesta });
        }



        public async Task<IActionResult> Categorias()
        {
            var result = await ListaCategoria();
            return View(result);
        }


        public async Task<List<Categoria>> ListaCategoria()
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
            return lista;
        }

        [HttpPost]
        public async Task<JsonResult> GuardarCategoria([FromBody] Categoria obj)
        {
            bool respuesta;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7034/api/Inventario/");

                var jsonCategoria = JsonConvert.SerializeObject(obj);

                var content = new StringContent(jsonCategoria, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("categorias", content);

                if (response.IsSuccessStatusCode)
                {
                    respuesta = true;
                }
                else
                {
                    respuesta = false;
                    Console.WriteLine("Error al guardar la categoría. Código de estado: " + response.StatusCode);
                }
            }

            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> ActualizarCategoria([FromBody] Categoria obj)
        {
            bool respuesta;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7034/api/Inventario/");
                    HttpResponseMessage response = await client.PutAsync($"categorias/{obj.IdCategoria}", new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        respuesta = true;
                    }
                    else
                    {
                        respuesta = false;
                        Console.WriteLine("Error al actualizar la categoría. Código de estado: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar la categoría: " + ex.Message);
                respuesta = false;
            }
            return Json(new { respuesta = respuesta });
        }


        [HttpDelete]
        public async Task<JsonResult> EliminarCategoria(int idCategoria)
        {
            bool respuesta;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7034/api/Inventario/");
                HttpResponseMessage response = await client.DeleteAsync($"categorias/{idCategoria}");
                if (response.IsSuccessStatusCode)
                {
                    respuesta = true;
                }
                else
                {
                    respuesta = false;
                    Console.WriteLine("Error al eliminar la categoría. Código de estado: " + response.StatusCode);
                }
            }
            return Json(new { respuesta = respuesta });
        }

    }
}
