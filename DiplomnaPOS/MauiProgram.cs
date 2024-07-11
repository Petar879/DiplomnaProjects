using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.Messaging;
using DiplomnaPOS.Interfaces;
using DiplomnaPOS.Messages;
using DiplomnaPOS.Models;
using DiplomnaPOS.Pages;
using DiplomnaPOS.Pages.AuthenticationPages;


#if WINDOWS || ANDROID
using DiplomnaPOS.Platforms;
#endif


#if WINDOWS
using DiplomnaPOS.Platforms.Windows;
#endif

using DiplomnaPOS.Services;
using MauiIcons.Fluent;
using Microsoft.Extensions.DependencyInjection;

//using DiplomnaPOS.Platforms;
//#if WINDOWS

//#endif

using Microsoft.Extensions.Logging;
using QuestPDF.Infrastructure;
using System.Diagnostics;
using System.Threading;

namespace DiplomnaPOS
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    //fonts.AddFont("Segoe Fluent Icons.ttf", "FA");
                });

            // Initialise the .Net Maui Icons - Fluent
            builder.UseMauiApp<App>().UseFluentMauiIcons();

            builder.Services.AddTransient<AuthServices>();
            builder.Services.AddTransient<LoadingPage>();
            builder.Services.AddTransient<LoginPage>();

            builder.Services.AddSingleton<DatabaseManager>();
            builder.Services.AddSingleton<Cart>();
            builder.Services.AddSingleton<CartProductModel>();

#if WINDOWS
            builder.Services.AddTransient<INfcService, InfcService>();
#endif

            builder.Services.AddTransient<StartPageViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<UpdateMenuPageViewModel>();
            builder.Services.AddTransient<DeviceManagerViewModel>();
            builder.Services.AddTransient<RefundsPageViewModel>();
            builder.Services.AddTransient<EmployeeManagementPageViewModel>();


            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<StartPage>();
            builder.Services.AddTransient<DeviceManagerPage>();
            builder.Services.AddTransient<UpdateMenuPage>();
            builder.Services.AddTransient<RefundsPage>();
            builder.Services.AddTransient<ReceiptManagerPage>();
            builder.Services.AddTransient<EmployeeManagementPage>();

#if WINDOWS
            NfcDeviceMonitor monitor = new();
            monitor.DeviceConnected += (sender, e) =>
            {
                WeakReferenceMessenger.Default.Send(new NfcStatusMessage(true));
                Console.WriteLine("NFC device connected.");
            };

            monitor.DeviceDisconnected += (sender, e) =>
            {
                WeakReferenceMessenger.Default.Send(new NfcStatusMessage(false));

                Device.BeginInvokeOnMainThread(async () =>
                {
                    Application.Current.MainPage.DisplayAlert($"Грешка!", "Липса свързан четец", "Ок");
                    
                });

                Console.WriteLine("NFC device disconnected.");
            };
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
