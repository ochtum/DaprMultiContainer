
using zipkin4net.Transport.Http;
using zipkin4net;
using zipkin4net.Tracers.Zipkin;
using zipkin4net.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDaprClient();
builder.Services.AddHttpClient("Tracer").AddHttpMessageHandler(provider => TracingHandler.WithoutInnerHandler(provider.GetService<IConfiguration>()["applicagtionName"]));
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
MyFrontEnd.Class.ZipkinExtension.UserZipkinCore(app, app.Lifetime,app.Services.GetRequiredService<ILoggerFactory>(), "http://zipkin:9411","MyFrontEnd");


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
