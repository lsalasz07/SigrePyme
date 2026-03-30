using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigrePyme.Data;
using SigrePyme.Models;
using System;

public class InventarioController : Controller
{
    private readonly AppDbContext _context;

    public InventarioController(AppDbContext context)
    {
        _context = context;
    }

    // LISTAR PRODUCTOS
    public IActionResult Index()
    {
        var productos = _context.Productos.ToList();
        return View(productos);
    }

    // FORMULARIO ENTRADA
    public IActionResult Entrada(int id)
    {
        var producto = _context.Productos.Find(id);
        if (producto == null)
            return NotFound();

        return View(producto);
    }

    // PROCESAR ENTRADA
    [HttpPost]
    public IActionResult Entrada(int id, int cantidad)
    {
        var producto = _context.Productos.Find(id);
        if (producto == null)
            return NotFound();

        if (cantidad <= 0)
        {
            ModelState.AddModelError("", "La cantidad debe ser mayor a 0");
            return View(producto);
        }

        producto.StockActual += cantidad;

        _context.TransaccionesInventario.Add(new TransaccionInventario
        {
            ProductoId = id,
            Cantidad = cantidad,
            Tipo = TipoTransaccion.Entrada,
            Fecha = DateTime.Now,
            StockResultante = producto.StockActual
        });

        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    // FORMULARIO SALIDA
    public IActionResult Salida(int id)
    {
        var producto = _context.Productos.Find(id);
        if (producto == null)
            return NotFound();

        return View(producto);
    }

    // PROCESAR SALIDA
    [HttpPost]
    public IActionResult Salida(int id, int cantidad)
    {
        var producto = _context.Productos.Find(id);
        if (producto == null)
            return NotFound();

        if (cantidad <= 0)
        {
            ModelState.AddModelError("", "La cantidad debe ser mayor a 0");
            return View(producto);
        }

        if (producto.StockActual < cantidad)
        {
            ModelState.AddModelError("", "Stock insuficiente");
            return View(producto);
        }

        producto.StockActual -= cantidad;

        _context.TransaccionesInventario.Add(new TransaccionInventario
        {
            ProductoId = id,
            Cantidad = cantidad,
            Tipo = TipoTransaccion.Salida,
            Fecha = DateTime.Now,
            StockResultante = producto.StockActual
        });

        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}