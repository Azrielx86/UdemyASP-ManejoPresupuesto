﻿using ManejoPresupuesto.Models;
using ManejoPresupuesto.Models.Transacciones;

namespace ManejoPresupuesto.Services;

public interface IReportService
{
    Task<ReporteTransacciones> GetDetailedTransactions(int uid, int month, int year, dynamic ViewBag);

    Task<ReporteTransacciones> GetTransactionReportByUser(int uid, int cuentaId, int month, int year, dynamic ViewBag);

    Task<IEnumerable<TransaccionesSemanal>> GetWeeklyReport(int uid, int month, int year, dynamic ViewBag);
}