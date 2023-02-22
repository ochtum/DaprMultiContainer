using Dapr.Client;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyFrontEnd.Models;
using System.Diagnostics;
using Dapr.AspNetCore;
using zipkin4net;

namespace MyFrontEnd.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DaprClient _daprClient;


        public IndexModel(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        public async Task OnGet()
        {
            var forecasts = await _daprClient.InvokeMethodAsync<IEnumerable<WeatherForecast>>(
                HttpMethod.Get,
                "MyBackEnd",
                "weatherforecast");
            ViewData["WeatherForecastData"] = forecasts;
            var trace = zipkin4net.Trace.Current.Child();
            trace.Record(Annotations.ServerRecv());
            trace.Record(Annotations.ServiceName("MyBackEnd1"));
            trace.Record(Annotations.Rpc("GET1"));
            trace.Record(Annotations.Tag("http.url", "http://zipkin:9411"));
            trace.Record(Annotations.ServerSend());

            var trace2 = zipkin4net.Trace.Create();
            trace2.Record(Annotations.ServerRecv());
            trace2.Record(Annotations.ServiceName("MyBackEndNew2222222222"));
            trace2.Record(Annotations.Rpc("GET"));
            trace2.Record(Annotations.Tag("http.url", "test"));
            trace2.Record(Annotations.ServerSend());

        }
    }
}