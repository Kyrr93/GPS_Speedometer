using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using GPS_Speedometer.Services;
using GPS_Speedometer.ViewModels;
using GPS_Speedometer.Views;

namespace GPS_Speedometer
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // Create an instance of GpsService
            var gpsService = new GpsService();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Remove Avalonia data validation to avoid duplicate validations
                BindingPlugins.DataValidators.RemoveAt(0);

                // Pass gpsService to MainViewModel
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainViewModel(gpsService)
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                // Pass gpsService to MainViewModel for single view platforms (e.g., mobile)
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = new MainViewModel(gpsService)
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
