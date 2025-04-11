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
        private readonly IHttpClientFactory _httpClientFactory; // Usamos IHttpClientFactory
        private readonly DA_Producto _daProducto = new DA_Producto();
        private readonly DA_Categoria _daCategoria = new DA_Categoria();

        public InventarioController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Productos()
        {
            var categoriasTask = ListaCategoria();
            var categorias = await categoriasTask;

            var productosTask = ListaProducto();
            var productos = await productosTask;

            ViewBag.Productos = productos;
            ViewBag.Categorias = categorias;

            return View();
        }

        public async Task<IActionResult> ListaProducto()
        {
            List<Producto> lista;

            try
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5009/api/Inventario/productos/");
                    HttpResponseMessage response = await client.GetAsync("");
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        lista = JsonConvert.DeserializeObject<List<Producto>>(apiResponse);
                    }
                    else
                    {
                        lista = new List<Producto>();
                        Console.WriteLine($"Error al obtener productos. Código de estado: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                lista = new List<Producto>();
                Console.WriteLine($"Excepción al obtener productos: {ex.Message}");
            }

            ViewBag.Categorias = await ListaCategoria();
            return View(lista);
        }

        [HttpPost]
        public async Task<JsonResult> GuardarProducto([FromBody] Producto obj)
        {
            bool respuesta;

            try
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    client.BaseAddress = new Uri("https://localhost:5009/api/Inventario/");

                    var jsonProducto = JsonConvert.SerializeObject(obj);
                    var content = new StringContent(jsonProducto, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("productos", content);

                    respuesta = response.IsSuccessStatusCode;
                    if (!respuesta)
                    {
                        Console.WriteLine($"Error al guardar el producto. Código de estado: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
                Console.WriteLine($"Excepción al guardar el producto: {ex.Message}");
            }

            return Json(new { respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> ActualizarProducto([FromBody] Producto obj)
        {
            bool respuesta;

            try
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    client.BaseAddress = new Uri("https://localhost:5009/api/Inventario/");
                    var jsonProducto = JsonConvert.SerializeObject(obj);
                    var content = new StringContent(jsonProducto, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync($"productos/{obj.IdProducto}", content);

                    respuesta = response.IsSuccessStatusCode;
                    if (!respuesta)
                    {
                        Console.WriteLine($"Error al actualizar el producto. Código de estado: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
                Console.WriteLine($"Excepción al actualizar el producto: {ex.Message}");
            }

            return Json(new { respuesta });
        }

        [HttpDelete]
        public async Task<JsonResult> EliminarProducto(int idProducto)
        {
            bool respuesta;

            try
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    client.BaseAddress = new Uri("https://localhost:5009/api/Inventario/");
                    HttpResponseMessage response = await client.DeleteAsync($"productos/{idProducto}");

                    respuesta = response.IsSuccessStatusCode;
                    if (!respuesta)
                    {
                        Console.WriteLine($"Error al eliminar el producto. Código de estado: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
                Console.WriteLine($"Excepción al eliminar el producto: {ex.Message}");
            }

            return Json(new { respuesta });
        }

        public async Task<IActionResult> Categorias()
        {
            var result = await ListaCategoria();
            return View(result);
        }

        public async Task<List<Categoria>> ListaCategoria()
        {
            List<Categoria> lista;

            try
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    client.BaseAddress = new Uri("https://localhost:5009/api/Inventario/categorias/");
                    HttpResponseMessage response = await client.GetAsync("");
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        lista = JsonConvert.DeserializeObject<List<Categoria>>(apiResponse);
                    }
                    else
                    {
                        lista = new List<Categoria>();
                        Console.WriteLine($"Error al obtener categorías. Código de estado: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                lista = new List<Categoria>();
                Console.WriteLine($"Excepción al obtener categorías: {ex.Message}");
            }

            return lista;
        }

        [HttpPost]
        public async Task<JsonResult> GuardarCategoria([FromBody] Categoria obj)
        {
            bool respuesta;

            try
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    client.BaseAddress = new Uri("https://localhost:5009/api/Inventario/");

                    var jsonCategoria = JsonConvert.SerializeObject(obj);
                    var content = new StringContent(jsonCategoria, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("categorias", content);

                    respuesta = response.IsSuccessStatusCode;
                    if (!respuesta)
                    {
                        Console.WriteLine($"Error al guardar la categoría. Código de estado: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
                Console.WriteLine($"Excepción al guardar la categoría: {ex.Message}");
            }

            return Json(new { respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> ActualizarCategoria([FromBody] Categoria obj)
        {
            bool respuesta;

            try
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    client.BaseAddress = new Uri("https://localhost:5009/api/Inventario/");
                    var jsonCategoria = JsonConvert.SerializeObject(obj);
                    var content = new StringContent(jsonCategoria, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync($"categorias/{obj.IdCategoria}", content);

                    respuesta = response.IsSuccessStatusCode;
                    if (!respuesta)
                    {
                        Console.WriteLine($"Error al actualizar la categoría. Código de estado: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
                Console.WriteLine($"Excepción al actualizar la categoría: {ex.Message}");
            }

            return Json(new { respuesta });
        }

        [HttpDelete]
        public async Task<JsonResult> EliminarCategoria(int idCategoria)
        {
            bool respuesta;

            try
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    client.BaseAddress = new Uri("https://localhost:5009/api/Inventario/");
                    HttpResponseMessage response = await client.DeleteAsync($"categorias/{idCategoria}");

                    respuesta = response.IsSuccessStatusCode;
                    if (!respuesta)
                    {
                        Console.WriteLine($"Error al eliminar la categoría. Código de estado: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
                Console.WriteLine($"Excepción al eliminar la categoría: {ex.Message}");
            }

            return Json(new { respuesta });
        }
    }
}
