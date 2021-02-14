using Microsoft.AspNetCore.SignalR.Client;
using StockTable1.DbContext;
using StockTable1.Model;
using StockTable1.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace StockTable1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private object senderObj;
        private static string _baseUrl = "https://js.devexpress.com/Demos/NetCore/liveUpdateSignalRHub";
        public event PropertyChangedEventHandler PropertyChanged;

        private string message;
        public string Message
        {
            get => this.message;
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }

        private SolidColorBrush upColor;
        public SolidColorBrush UpColor
        {
            get => this.upColor;
            set
            {
                upColor = value;
                OnPropertyChanged("UpColor");
            }
        }

        private SolidColorBrush downColor;
        public SolidColorBrush DownColor
        {
            get => this.downColor;
            set
            {
                downColor = value;
                OnPropertyChanged("DownColor");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //MainViewModel mainViewModel = new MainViewModel();
            //this.DataContext = mainViewModel;

            this.DataContext = this;
            UpColor = new SolidColorBrush(Colors.Green);
            DownColor = new SolidColorBrush(Colors.Red);
            StockData = new Dictionary<string, StockUpdate>();
            StartHub();
        }

        public async void StartHub()
        {
            SqliteDataAccess.TruncateStock();
            var _hubConnection = new HubConnectionBuilder()
                .WithUrl(_baseUrl).Build();

            _hubConnection.On<StockUpdate>("updateStockPrice", data =>
            {
                StockUpdateCallBack(data);
            });
            try
            {
                Message = "Waiting";
                await _hubConnection.StartAsync();
                Console.WriteLine("State " + _hubConnection.State);
                Message = _hubConnection.State.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToJson());
                Message = e.ToJson();
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void StockUpdateCallBack(StockUpdate stockUpdate)
        {
            try
            {
                if (!StockData.ContainsKey(stockUpdate.Symbol))
                {
                    var isColor = false;
                    if (stockUpdate.Change >= 0)
                        isColor = true;
                    else
                        isColor = false;

                    stockUpdate.LastUpdate = DateTime.Now;
                    stockUpdate.Color = isColor ? UpColor : DownColor;
                    stockUpdate.IsColor = isColor;
                    StockData.Add(stockUpdate.Symbol, stockUpdate);
                    SqliteDataAccess.InsertStock(stockUpdate);
                    Dg.Items.Refresh();
                }
                else
                {
                    UpdateStock(stockUpdate);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void UpdateStock(StockUpdate stockUpdate)
        {
            if (Dg.SelectedItems.Count >= 3)
            {
                for (int i = 0; i < 2; i++)
                {
                    (((KeyValuePair<string, StockUpdate>)Dg.SelectedItems[i]).Value).IsSelected = false;
                }
            }
            var selected = StockData[stockUpdate.Symbol];
            if (!stockUpdate.Change.ToString().Contains('-'))
            {
                selected.IsColor = true;
                selected.Color = UpColor;
                System.Diagnostics.Debug.WriteLine(stockUpdate.Change + "\t" + selected.IsColor);
            }
            else
            {
                selected.IsColor = false;
                selected.Color = DownColor;
                System.Diagnostics.Debug.WriteLine(stockUpdate.Change + "\t" + selected.IsColor);
            }
            selected.LastUpdate = DateTime.Now;
            selected.Price = stockUpdate.Price;
            selected.Change = stockUpdate.Change;
            selected.IsSelected = true;
            SqliteDataAccess.UpdateStock(stockUpdate);
        }

        private void Rectangle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                var selectedColor = ((System.Windows.Shapes.Shape)(sender)).Fill.ToString();
                var brush = new BrushConverter().ConvertFromString(selectedColor) as SolidColorBrush;
                (((System.Windows.Shapes.Shape)(((System.Windows.Controls.Decorator)(((System.Windows.Controls.ContentControl)(senderObj)).Content)).Child)).Fill) = brush;
                foreach (var item in StockData)
                {
                    if (item.Value.Change >= 0)
                        item.Value.Color = UpColor;
                    else
                        item.Value.Color = DownColor;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DownColor_Click(object sender, RoutedEventArgs e)
        {
            PopupColorPalette.IsOpen = true;
            senderObj = sender;
        }

        private void UpColor_Click(object sender, RoutedEventArgs e)
        {
            PopupColorPalette.IsOpen = true;
            senderObj = sender;
        }
    }
}
