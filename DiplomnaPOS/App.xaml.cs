using DiplomnaPOS.Pages;
using Microsoft.Maui.Controls;
using System;

namespace DiplomnaPOS
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = page;
            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);
            window.MinimumWidth = 1280;
            window.MinimumHeight = 800;

            return window;
        }
    }
}
