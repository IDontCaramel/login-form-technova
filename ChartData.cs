using System;
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace AddData
{


    public class ChartData : INotifyPropertyChanged
    {
        public string CategoryName { get; set; }
        public ObservableCollection<SeriesData> Series { get; set; }

        public ChartData(string categoryName)
        {
            CategoryName = categoryName;
            Series = new ObservableCollection<SeriesData>();
        }

        public void AddSeriesData(string seriesName, int value)
        {
            Series.Add(new SeriesData(seriesName, value));
        }

        public class SeriesData : INotifyPropertyChanged
        {
            private string _name;
            public string Name
            {
                get { return _name; }
                set
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }

            private int _value;
            public int Value
            {
                get { return _value; }
                set
                {
                    _value = value;
                    OnPropertyChanged("Value");
                }
            }

            public SeriesData(string name, int value)
            {
                Name = name;
                Value = value;
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
