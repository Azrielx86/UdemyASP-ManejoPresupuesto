using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Models.Transacciones;

namespace ManejoPresupuesto.Services;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Cuenta, CuentasCreacionViewModel>();
        CreateMap<TransaccionActualizarViewModel, Transaccion>().ReverseMap();
    }
}