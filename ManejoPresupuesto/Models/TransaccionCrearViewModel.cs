using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models;

#nullable disable

public class TransaccionCrearViewModel : Transaccion
{
    public IEnumerable<SelectListItem> Categorias { get; set; }
    public IEnumerable<SelectListItem> Cuentas { get; set; }

    [Display(Name = "Tipo de operación")]
    public TipoOperacion TipoOperacionId { get; set; } = TipoOperacion.Ingreso;
}