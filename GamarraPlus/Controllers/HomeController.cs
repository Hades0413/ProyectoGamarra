﻿using Microsoft.AspNetCore.Mvc;
using GamarraPlus.Datos;
using GamarraPlus.Models;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Security.Authentication;

namespace GamarraPlus.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        DA_Producto _daProducto = new DA_Producto();
        DA_Venta _daVenta = new DA_Venta();

        public async Task<IActionResult> Index()
        {
            List<Producto> productos;

            // Crear un HttpClientHandler que omita la validación del certificado SSL
            var handler = new HttpClientHandler
            {
                // Si es un certificado autofirmado o un certificado inválido, se acepta.
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            // Forzar el uso de TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                using (var client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri("http://localhost:5009/api/Inventario/productos/");
                    HttpResponseMessage response = await client.GetAsync("");

                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        productos = JsonConvert.DeserializeObject<List<Producto>>(apiResponse);
                    }
                    else
                    {
                        productos = new List<Producto>();
                    }
                }
            }
            catch (HttpRequestException httpRequestException)
            {
                // Capturar más detalles sobre el error de la solicitud HTTP
                Console.WriteLine($"Error en la solicitud HTTP: {httpRequestException.Message}");
                productos = new List<Producto>();
            }
            catch (AuthenticationException authException)
            {
                // Capturar detalles sobre el error de autenticación
                Console.WriteLine($"Error de autenticación SSL: {authException.Message}");
                productos = new List<Producto>();
            }
            catch (Exception ex)
            {
                // Capturar cualquier otro error general
                Console.WriteLine($"Se produjo un error inesperado: {ex.Message}");
                productos = new List<Producto>();
            }

            return View(productos);
        }

        public IActionResult CrearVenta()
        {
            return View();
        }

        public IActionResult DetalleVenta()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public JsonResult AutoCompleteProducto(string search)
        {
            List<Autocomplete> autocomplete = new List<Autocomplete>();
            autocomplete = _daProducto.Listar()
                .Where(x => string.Concat(x.Codigo.ToUpper(), x.oCategoria.Descripcion.ToUpper(), x.Descripcion.ToUpper()).Contains(search.ToUpper()))
                .Select(m => new Autocomplete
                {
                    label = $"{m.Codigo} - {m.oCategoria.Descripcion} - {m.Descripcion}",
                    value = m.IdProducto
                }
                ).ToList();

            return Json(autocomplete);
        }

        [HttpGet]
        public JsonResult ObtenerProducto(int idproducto)
        {
            Producto? oProducto = new Producto();
            oProducto = _daProducto.Listar().Where(x => x.IdProducto == idproducto).FirstOrDefault();
            return Json(oProducto);
        }

        [HttpPost]
        public JsonResult RegistrarVenta([FromBody] Venta body)
        {
            string rpta = "";

            XElement venta = new XElement("Venta",
                new XElement("TipoPago", body.TipoPago),
                new XElement("NumeroDocumento", "0"),
                new XElement("DocumentoCliente", body.DocumentoCliente),
                new XElement("NombreCliente", body.NombreCliente),
                new XElement("MontoPagoCon", body.MontoPagoCon),
                new XElement("MontoCambio", body.MontoCambio),
                new XElement("MontoSubTotal", body.MontoSubTotal),
                new XElement("MontoIGV", body.MontoIGV),
                new XElement("MontoTotal", body.MontoTotal)
            );

            XElement oDetalleVenta = new XElement("Detalle_Venta");
            foreach (Detalle_Venta item in body.oDetalleVenta)
            {
                oDetalleVenta.Add(new XElement("Item",
                    new XElement("IdProducto", item.oProducto.IdProducto),
                    new XElement("PrecioVenta", item.PrecioVenta),
                    new XElement("Cantidad", item.Cantidad),
                    new XElement("Total", item.Total)
                    ));
            }

            venta.Add(oDetalleVenta);

            rpta = _daVenta.Registrar(venta.ToString());

            return Json(new { respuesta = rpta });
        }

        [HttpGet]
        public JsonResult ObtenerVenta(string nrodocumento)
        {
            Venta? oVenta = new Venta();
            oVenta = _daVenta.Detalle(nrodocumento);
            return Json(oVenta);
        }
    }
}