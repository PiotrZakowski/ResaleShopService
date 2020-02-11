using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RESTApp.Models
{
    public class Item : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _manufacturerName;
        public string ManufacturerName {
            get { return _manufacturerName;  }
            set {
                _manufacturerName = value;
                OnPropertyChanged();
            }
        }

        private string _modelName;
        public string ModelName {
            get { return _modelName; }
            set{
                _modelName = value;
                OnPropertyChanged();
            }
        }

        public float Price { get; set; }
        public int Quantity { get; set; }
        public string OriginCountry { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Item()
        { }
    }
}