using Dapr.Client;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyFrontEnd.Models;
using zipkin4net;

namespace MyFrontEnd.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DaprClient _daprClient;
        private readonly HttpClient _httpClient;

        public IndexModel(DaprClient daprClient, HttpClient httpClient)
        {
            _daprClient = daprClient;
            _httpClient = httpClient;
        }

        public async Task OnGet()
        {
            //HttpClient
            var response = await _httpClient.GetAsync("/weatherforecast");
            IEnumerable<WeatherForecast> forecasts = null;

            if (response.IsSuccessStatusCode)
            {
                forecasts = await response.Content.ReadFromJsonAsync<IEnumerable<WeatherForecast>>();
            }

            //DaprClient
            var forecasts2 = await _daprClient.InvokeMethodAsync<IEnumerable<WeatherForecast>>(
                HttpMethod.Get,
                "MyBackEnd",
                "weatherforecast");

            //通常ルートforecastsでも、Daprサイドカールートforecasts2でも、同じ結果が取得できる
            ViewData["WeatherForecastData"] = forecasts;
            
            //ZipkinTrace追加
            var trace = zipkin4net.Trace.Current.Child();
            trace.Record(Annotations.ServerRecv());
            trace.Record(Annotations.ServiceName("MyBackEnd1"));
            trace.Record(Annotations.Rpc("GET1"));
            trace.Record(Annotations.Tag("http.url", "http://zipkin:9411"));
            trace.Record(Annotations.ServerSend());

            //ZipkinTrace新規作成
            var trace2 = zipkin4net.Trace.Create();
            trace2.Record(Annotations.ServerRecv());
            trace2.Record(Annotations.ServiceName("MyBackEndNew"));
            trace2.Record(Annotations.Rpc("GET"));
            trace2.Record(Annotations.Tag("http.url", "test"));
            trace2.Record(Annotations.ServerSend());

        }
    }
}