using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.Security.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Configurar Kestrel para usar TLS 1.2
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Forzar el uso de TLS 1.2
    serverOptions.ConfigureHttpsDefaults(co =>
    {
        co.SslProtocols = SslProtocols.Tls12;
    });
});

// Agregar servicios a la colección
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

// Configuración de autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Acceso/Index"; // Ruta de inicio de sesión
    });

// Configuración de sesiones
builder.Services.AddSession();

// Configurar servicios HTTP y agregar HttpClientHandler para ignorar errores de certificados autofirmados en desarrollo (si es necesario)
builder.Services.AddHttpClient("ApiCliente", client =>
{
    client.BaseAddress = new Uri("https://localhost:5009");
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    // Si estás usando un certificado autofirmado en desarrollo, esto lo permite
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true // Aceptar cualquier certificado
});

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    // Habilitar Swagger u otras herramientas de desarrollo
    app.UseDeveloperExceptionPage();
}

// Usar autenticación
app.UseAuthentication();

// Usar sesiones
app.UseSession();

// Servir archivos estáticos
app.UseStaticFiles();

// Configurar la ruta de enrutamiento
app.UseRouting();

// Usar autorización
app.UseAuthorization();

// Configurar la ruta predeterminada de controladores
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Index}/{id?}");

app.Run();
