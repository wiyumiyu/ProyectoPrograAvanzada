using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoPrograAvanzada.Data;

namespace ProyectoPrograAvanzada.Controllers
{
    [Authorize(Roles = "Jefe,Administrador")]
    public class TBitacorasController : Controller
    {
        private readonly DbAlquilerVehiculosContext _context;

        public TBitacorasController(DbAlquilerVehiculosContext context)
        {
            _context = context;
        }

        // ======================================================
        // LISTADO (SOLO LECTURA)
        // ======================================================
        public async Task<IActionResult> Index()
        {
            var bitacora = await _context.TBitacoras
                .OrderByDescending(b => b.Fecha)
                .ToListAsync();

            return View(bitacora);
        }

        // ======================================================
        // DETALLES (SOLO LECTURA)
        // ======================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var bitacora = await _context.TBitacoras
                .FirstOrDefaultAsync(b => b.IdBitacora == id);

            if (bitacora == null)
                return NotFound();

            return View(bitacora);
        }
    }
}
