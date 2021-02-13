using System;
using System.Collections.Generic;
using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StockTable1.Model;
using StockTable1.ViewModel;

namespace StockTable1
{
    public class Program
    {
        public Program(MainViewModel mainViewModel)
        {
            stockUpdateDelegate = new MainViewModel.StockUpdateDelegate(mainViewModel.StockUpdateRe);
        }

        private MainViewModel.StockUpdateDelegate stockUpdateDelegate;
        private static string _baseUrl = "https://js.devexpress.com/Demos/NetCore/liveUpdateSignalRHub";

        public void StartHub()
        {
            var _hubConnection = new HubConnectionBuilder()
                .WithUrl(_baseUrl).Build();
            
            _hubConnection.On<StockUpdate>("updateStockPrice", data =>
            {
                Console.WriteLine(data.ToJson());
                stockUpdateDelegate(data);
            });
            try
            {
                _hubConnection.StartAsync();
                Console.WriteLine("State " + _hubConnection.State);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToJson());
                throw;
            }
        }
    }

    public static class JsonExtesntions
    {
        public static string ToJson(this object obj)
        {
            try
            {
                var res = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    DateTimeZoneHandling = DateTimeZoneHandling.Local,
                    ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
                });
                return res;
            }
            catch (Exception e)
            {
                return $"error convert to json: {obj}, excption:{e.ToJson()}";
            }
        }
        public static string ToJson(this Exception ex)
        {
            var res = new Dictionary<string, string>()
            {
                {"Type",ex.GetType().ToString()},
                {"Message",ex.Message},
                {"StackTrace",ex.StackTrace}
            };
            return res.ToJson();
        }
    }
}