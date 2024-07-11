using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomnaPOS.Models
{
    public partial class UsersDataModel : ObservableObject
    {
        [ObservableProperty]
        private string _id;

        [ObservableProperty]
        private string _userName;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _role;

        public UsersDataModel() { }

        public UsersDataModel(string userId, string userName, string email, string role)
        {
            Id = userId;
            UserName = userName;
            Email = email;
            Role = role;
        }
    }
}
