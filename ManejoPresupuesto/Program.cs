using ManejoPresupuesto.Models.Users;
using ManejoPresupuesto.Services;
using ManejoPresupuesto.Services.Messages;
using ManejoPresupuesto.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
builder.Services.AddControllersWithViews(op =>
{
    op.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddTransient<IRepositoryTiposCuentas, RepositoryTiposCuentas>();
builder.Services.AddTransient<IRepositoryAccounts, RepositoryAccounts>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IRepositoryCategory, RepositoryCategory>();
builder.Services.AddTransient<IRepositoryTransactions, RepositoryTransactions>();
builder.Services.AddTransient<IReportService, ReportService>();
builder.Services.AddTransient<IRepositoryUsers, RepositoryUsers>();
builder.Services.AddTransient<IUserStore<User>, UserStore>();
builder.Services.AddIdentityCore<User>().AddErrorDescriber<MensajesDeErrorIdentity>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAuthentication(op =>
{
    op.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    op.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    op.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
}).AddCookie(IdentityConstants.ApplicationScheme, op =>
{
    op.LoginPath = "/usuarios/login";
});


builder.Services.AddTransient<SignInManager<User>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Transacciones}/{action=Index}/{id?}");

app.Run();