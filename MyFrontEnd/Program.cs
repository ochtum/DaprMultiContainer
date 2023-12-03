
using zipkin4net.Transport.Http;
using zipkin4net;
using zipkin4net.Tracers.Zipkin;
using zipkin4net.Middleware;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});
// Add services to the container.

//DaprClient
builder.Services.AddDaprClient(builder =>
{
    builder.UseHttpEndpoint("http://envoygateway:10000");
});
//HttpClient
builder.Services.AddHttpClient();
//ZipkinTraceハンドラー
builder.Services.AddHttpClient("Tracer").AddHttpMessageHandler(provider => TracingHandler.WithoutInnerHandler(provider.GetService<IConfiguration>()["applicagtionName"]));

builder.Services.AddRazorPages();

//HttpClientのBaseUrl設定
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://envoygateway:10000") });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//ZipkinTrace出力
MyFrontEnd.Class.ZipkinExtension.UserZipkinCore(app, app.Lifetime,app.Services.GetRequiredService<ILoggerFactory>(), "http://zipkin:9411","MyFrontEnd");


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
