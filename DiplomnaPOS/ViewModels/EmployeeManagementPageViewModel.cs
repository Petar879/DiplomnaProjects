using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomnaPOS.Models;
using DiplomnaPOS.Pages.Popups;
using DiplomnaPOS.Services;
using EfLibrary.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DiplomnaPOS.Pages
{
    public partial class EmployeeManagementPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private List<UsersDataModel> _employeesList = new();

        private List<string> rolesList = new();

        private readonly AuthServices _authServices;

        public EmployeeManagementPageViewModel(AuthServices authServices)
        {
            _authServices = authServices;
            GetUsersData();
            GetRoles();
        }

        public async Task GetUsersData()
        {
            string _userBearerToken = await _authServices.RefreshUserLogin();

            if (_userBearerToken != null)
            {
                EmployeesList.Clear();
                var uri = new Uri($"{AuthServices.serverURL}/api/Roles/GetUsersAndTheirRoles"); // Replace with your actual login endpoint

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userBearerToken);
                    //var content = new StringContent(Encoding.UTF8, "application/json");
                    var response = await client.GetAsync(uri);

                    if (response.IsSuccessStatusCode)
                    {
                        EmployeesList = JsonConvert.DeserializeObject<List<UsersDataModel>>(await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        // Handle login error response
                        Debug.WriteLine(response.StatusCode.ToString());
                    }
                }

                _userBearerToken = String.Empty;
            }
        }
        private async Task GetRoles()
        {
            string _userBearerToken = await _authServices.RefreshUserLogin();

            if (_userBearerToken != null)
            {
                EmployeesList.Clear();
                var uri = new Uri($"{AuthServices.serverURL}/api/roles/GetAllRoles"); // Replace with your actual login endpoint

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userBearerToken);
                    //var content = new StringContent(Encoding.UTF8, "application/json");
                    var response = await client.GetAsync(uri);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        var jsonData = JsonConvert.DeserializeObject<List<JObject>>(responseBody);

                        foreach (var role in jsonData)
                        {
                            rolesList.Add((string)role["name"]);
                        }

                        //await Application.Current.MainPage.DisplayAlert($"Добавяне на роля", rolesJson.ToString(), "Ok");
                        //SecureStorage.Default.Remove("loggedUserRefreshToken");
                    }
                    else
                    {
                        // Handle login error response
                        Debug.WriteLine(response.StatusCode.ToString());
                    }
                }

                _userBearerToken = String.Empty;
            }
        }

        [RelayCommand]
        public async Task AddNewEmployee()
        {
            var result = await Application.Current.MainPage.ShowPopupAsync(new AddNewEmployeePopup(_authServices, rolesList));

            if (result != null)
            {
                if ((bool)result)
                {
                    await Application.Current.MainPage.DisplayAlert("Success!", "New employee was registered", "Ok");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Something went wrong", "Ok");
                }
            }

            GetUsersData();
            GetRoles();
            //var result = await Application.Current.MainPage.ShowPopupAsync(new EditEmployeePopup());
        }

        [RelayCommand]
        public async Task AddNewRole()
        {
            string result = await Application.Current.MainPage.DisplayPromptAsync($"Добавяне на роля", "Име на новата роля");
            if (result != null)
            {
                if (await _authServices.CreateNewRole(result))
                {
                    await Application.Current.MainPage.DisplayAlert("Успех!", "Нова реоля е добавена успешно", "Ок");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Грешка", "Нещо се е объркало", "Ок");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Грешка", "Липсва текст", "Ок");
            }
        }

        [RelayCommand]
        public async Task EditEmployee(UsersDataModel usersDataModel)
        {
            await Application.Current.MainPage.DisplayAlert("TODO", " ", "Ок");
        }


        [RelayCommand]
        public async Task DeleteEmployee(UsersDataModel usersDataModel)
        {
            string _userBearerToken = await _authServices.RefreshUserLogin();

            bool answer = await Application.Current.MainPage.DisplayAlert("Внимание!", $"Искате ли да изтриете потребителя\"{usersDataModel.UserName}\"", "Да", "Не");

            if (answer)
            {
                if (_userBearerToken != null)
                {
                    var uri = new Uri($"{AuthServices.serverURL}/UserInfo/DeleteUser"); // Replace with your actual login endpoint
                    var registerModel = new
                    {
                        userId = usersDataModel.Id
                    };
                    var jsonInString = JsonConvert.SerializeObject(registerModel);

                    using (var client = new HttpClient())
                    {
                        var content = new StringContent(jsonInString, Encoding.UTF8, "application/json");
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userBearerToken);
                        var response = await client.PostAsync(uri, content);

                        Debug.WriteLine("Employee deletion returns: " + response.StatusCode);


                        if (response.IsSuccessStatusCode)
                        {
                            await Application.Current.MainPage.DisplayAlert("Успех!", "Информацията за служителя бе изтрита", "Ок");
                            await GetUsersData();
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Грешка", "Нещо се е объркало при изтриването", "Ок");
                        }
                    }

                    _userBearerToken = String.Empty;
                }
            }




        }

        
    
    }
}
