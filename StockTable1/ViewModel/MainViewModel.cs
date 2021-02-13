using Microsoft.AspNetCore.SignalR.Client;
using StockTable1.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace StockTable1.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public delegate void StockUpdateDelegate(StockUpdate stockUpdate);

        #region Properties

        private static string _baseUrl = "https://js.devexpress.com/Demos/NetCore/liveUpdateSignalRHub";

        private Dictionary<string, StockUpdate> stockData;
        public Dictionary<string, StockUpdate> StockData
        {
            get => stockData;
            set
            {
                stockData = value;
                OnPropertyChanged("StockData");
            }
        }
        
        #endregion

        public MainViewModel()
        {
            StockData = new Dictionary<string, StockUpdate>();
            GetData();
        }

        void GetData()
        {
            //Program program = new Program(this);
            //program.StartHub();
            StartHub();
        }

        public void StockUpdateRe(StockUpdate stockUpdate)
        {
            if (!StockData.ContainsKey(stockUpdate.Symbol))
            {
                StockData.Add(stockUpdate.Symbol, new StockUpdate
                {
                    LastUpdate = DateTime.Now,
                    Change = stockUpdate.Change,
                    Symbol = stockUpdate.Symbol,
                    Price = stockUpdate.Price
                });
            }
            else
            {
                var selected = StockData[stockUpdate.Symbol];
                selected.LastUpdate = DateTime.Now;
                selected.Price = stockUpdate.Price;
                selected.Change = stockUpdate.Change;
            }
        }

        public void StartHub()
        {
            var _hubConnection = new HubConnectionBuilder()
                .WithUrl(_baseUrl).Build();

            _hubConnection.On<StockUpdate>("updateStockPrice", data =>
            {
                //Console.WriteLine(data.ToJson());
                StockUpdateRe(data);
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
}
