﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GamarraPlus.Datos;
using GamarraPlus.Models;

namespace GamarraPlus.Controllers
{
    [Authorize]
    public class ReporteController : Controller
    {
        DA_Reporte _daReporte = new DA_Reporte();
        public IActionResult Ventas()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ReporteVenta(string fechaInicio, string fechaFin)
        {
            List<Reporte> oLista = new List<Reporte>();
            oLista = _daReporte.Listar(fechaInicio, fechaFin);
            return Json(new { data = oLista });
        }
    }
}
