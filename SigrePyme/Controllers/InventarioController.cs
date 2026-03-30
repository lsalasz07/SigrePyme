using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

public class InventarioController : Controller
{
    private readonly ApplicationDbContext _context;

    public InventarioController(ApplicationDbContext context)
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
        return View(producto);
    }

    // PROCESAR ENTRADA
    [HttpPost]
    public IActionResult Entrada(int id, int cantidad)
    {
        var producto = _context.Productos.Find(id);

        producto.Stock += cantidad;

        _context.Movimientos.Add(new MovimientoInventario
        {
            ProductoId = id,
            Cantidad = cantidad,
            TipoMovimiento = "ENTRADA",
            Fecha = DateTime.Now
        });

        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    // FORMULARIO SALIDA
    public IActionResult Salida(int id)
    {
        var producto = _context.Productos.Find(id);
        return View(producto);
    }

    // PROCESAR SALIDA
    [HttpPost]
    public IActionResult Salida(int id, int cantidad)
    {
        var producto = _context.Productos.Find(id);

        if (producto.Stock < cantidad)
        {
            ModelState.AddModelError("", "Stock insuficiente");
            return View(producto);
        }

        producto.Stock -= cantidad;

        _context.Movimientos.Add(new MovimientoInventario
        {
            ProductoId = id,
            Cantidad = cantidad,
            TipoMovimiento = "SALIDA",
            Fecha = DateTime.Now
        });

        _context.SaveChanges();
        return RedirectToAction("Index");
    }