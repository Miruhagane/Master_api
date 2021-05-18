using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Queue.Models.ViewModel
{
    public class UserViewModel : INotifyPropertyChanged
    {
        public string Token { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}