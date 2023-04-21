using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models;

#nullable disable

public class Transaccion
{
    [Range(0, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría.")]
    [Display(Name = "Categoría")]
    public int CategoriaId { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Debe seleccionar una cuenta.")]
    [Display(Name = "Cuenta")]
    public int CuentaId { get; set; }

    [Display(Name = "Fecha transacción")]
    [DataType(DataType.Date)]
    public DateTime FechaTransaccion { get; set; } = DateTime.Today;

    public int Id { get; set; }
    public decimal Monto { get; set; }

    [StringLength(1000, ErrorMessage = "La nota no puede ser mayor a {1} caracteres.")]
    public string Nota { get; set; }

    [Display(Name = "Usuario")]
    public int UsuarioId { get; set; }
}