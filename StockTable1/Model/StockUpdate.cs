using StockTable1.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace StockTable1.Model
{
    public class StockUpdate : BaseViewModel
    {
        private DateTime lastUpdate;
        private string symbol;
        private double price;
        private double change;
        private bool isColor;
        private bool isSelected;
        private double percentage;
        private SolidColorBrush color;

        public DateTime LastUpdate { get => lastUpdate; set { lastUpdate = value; OnPropertyChanged("LastUpdate"); } }
        public string Symbol { get => symbol; set { symbol = value; OnPropertyChanged("Symbol"); } }
        public double Price { get => price; set { price = value; OnPropertyChanged("Price"); } }
        public double Change { get => change; set { change = value; OnPropertyChanged("Change"); } }
        public bool IsColor { get => isColor; set { isColor = value; OnPropertyChanged("IsColor"); } }
        public SolidColorBrush Color { get => color; set { color = value; OnPropertyChanged("Color"); } }
        public bool IsSelected { get => isSelected; set { isSelected = value; OnPropertyChanged("IsSelected"); } }
        public double Percentage { get => percentage; set { percentage = value; OnPropertyChanged("Percentage"); } }

    }
}
