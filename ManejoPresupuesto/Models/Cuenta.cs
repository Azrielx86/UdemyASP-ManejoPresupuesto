using ManejoPresupuesto.Validations;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models;

public class Cuenta
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo {0} es requerido")]
    [MaxLength(50)]
    [FirstCharMayus]
    public string Nombre { get; set; }

    [Display(Name = "Tipo Cuenta")]
    public int TipoCuentaId { get; set; }

    public decimal Balance { get; set; }

    [StringLength(maximumLength: 1000)]
    public string? Descripcion { get; set; }

    public string? TipoCuenta { get; set; }
}