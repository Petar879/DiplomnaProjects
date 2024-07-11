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

namespace DiplomnaPOS.Services
{
    public class AuthServices
    {
        private const string AuthStateKey = "AuthState";
        public const string serverURL = @"https://localhost:7049";

        public async Task<bool> IsAuthenticatedAsync()
        {
            await Task.Delay(1000);

            var authState = Preferences.Default.Get<bool>(AuthStateKey, false);

            return authState;
        }

        public async Task<string> GetUserBearerTokenAsync(string inputEmail, string inputPassord)
        {
            var uri = new Uri($"{serverURL}/login?useCookies=false"); // Replace with your actual login endpoint
            var loginModel = new
            {
                email = inputEmail,
                password = inputPassord
            };
            var jsonInString = JsonConvert.SerializeObject(loginModel);

            using (var client = new HttpClient())
            {
                var content = new StringContent(jsonInString, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    // Handle successful login response
                    //var responseBody = await response.Content.ReadAsStringAsync();

                    var responseBody = await response.Content.ReadAsStringAsync();

                    JsonNode forecastNode = JsonNode.Parse(responseBody)!;

                    await SecureStorage.Default.SetAsync("loggedUserRefreshToken", forecastNode!["refreshToken"].ToString());

                    return forecastNode!["accessToken"].ToString();
                }
                else
                {
                    // Handle login error response
                    return String.Empty;
                }
            }
        }

        public async Task<bool> CanUserUseTheSystemAsync(string userBearerToken)
        {
            //var uri = new Uri($"{serverURL}/GetLoggedUserInfo"); // Replace with your actual login endpoint
            var uri = new Uri($"{serverURL}/UserInfo"); // Replace with your actual login endpoint

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userBearerToken);
                //var content = new StringContent(Encoding.UTF8, "application/json");
                var response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    JsonNode forecastNode = JsonNode.Parse(responseBody)!;

                    if (forecastNode!["userRole"] != null)
                    {
                        await Login(forecastNode!["userId"].ToString(), forecastNode!["userRole"].ToString());
                        return true;
                    }
                    else
                    {
                        SecureStorage.Default.Remove("loggedUserRefreshToken");
                    }
                }
                else
                {
                    // Handle login error response
                    Debug.WriteLine(response.StatusCode.ToString());
                }
            }

            return false;

        }
        public async Task Login(string userId, string userRole)
        {
            Preferences.Default.Set<bool>(AuthStateKey, true);
            await SecureStorage.Default.SetAsync("loggedUserId", userId);
            await SecureStorage.Default.SetAsync("loggedUserRole", userRole);
        }

        public async Task<bool> RegisterUser(string userEmail, string userPassword)
        {
            var uri = new Uri($"{serverURL}/register"); // Replace with your actual login endpoint
            var registerModel = new
            {
                email = userEmail,
                password = userPassword
            };
            var jsonInString = JsonConvert.SerializeObject(registerModel);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await RefreshUserLogin());
                var content = new StringContent(jsonInString, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                Debug.WriteLine("New employee registration returns: " + response.StatusCode);
                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> AssignUserRole(string _userName, string _userRole)
        {
            var uri = new Uri($"{serverURL}/api/Roles/AssignUserRole"); // Replace with your actual login endpoint
            var registerModel = new
            {
                userName = _userName,
                userRole = _userRole
            };
            var jsonInString = JsonConvert.SerializeObject(registerModel);

            using (var client = new HttpClient())
            {
                var content = new StringContent(jsonInString, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await RefreshUserLogin());
                var response = await client.PostAsync(uri, content);

                Debug.WriteLine("New employee with role returns: " + response.StatusCode);
                return response.IsSuccessStatusCode;
            }
        }

        public void Logout()
        {
            Preferences.Default.Remove(AuthStateKey);
            SecureStorage.RemoveAll();
            //TODO Logout endpoint
        }


        public async Task<string> RefreshUserLogin()
        {
            var uri = new Uri($"{serverURL}/refresh"); // Replace with your actual login endpoint
            var loginModel = new
            {
                refreshToken = await SecureStorage.Default.GetAsync("loggedUserRefreshToken")
            };
            var jsonInString = JsonConvert.SerializeObject(loginModel);

            using (var client = new HttpClient())
            {
                var content = new StringContent(jsonInString, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    // Handle successful login response
                    //var responseBody = await response.Content.ReadAsStringAsync();

                    var responseBody = await response.Content.ReadAsStringAsync();

                    JsonNode forecastNode = JsonNode.Parse(responseBody)!;

                    SecureStorage.SetAsync("loggedUserRefreshToken", forecastNode!["refreshToken"].ToString());

                    return forecastNode!["accessToken"].ToString();
                }
                else
                {
                    // Handle login error response
                    Debug.WriteLine(response.RequestMessage);
                }
            }

            return null;
        }

        public async Task<bool> CreateNewRole(string _newRole)
        {
            var uri = new Uri($"{serverURL}/api/Roles/CreateRole"); // Replace with your actual login endpoint
            var registerModel = new
            {
                roleName = _newRole
            };
            var jsonInString = JsonConvert.SerializeObject(registerModel);

            using (var client = new HttpClient())
            {
                var content = new StringContent(jsonInString, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await RefreshUserLogin());
                var response = await client.PostAsync(uri, content);

                Debug.WriteLine("New employee with role returns: " + response.StatusCode);
                return response.IsSuccessStatusCode;
            }
        }
    }
}
