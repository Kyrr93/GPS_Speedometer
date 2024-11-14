using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GPS_Speedometer.Services;
using Xamarin.Essentials;

namespace GPS_Speedometer.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly GpsService _gpsService;

        [ObservableProperty]
        private string _greeting = "Welcome to GPS Speedometer!";

        [ObservableProperty]
        private string _currentSpeedText = "0 km/h";

        [ObservableProperty]
        private string _distanceText = "0 km";

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value); // SetProperty is inherited from ViewModelBase
        }

        private double _totalDistance;
        private Location _lastLocation;

        public MainViewModel(GpsService gpsService)
        {
            _gpsService = gpsService;
            RefreshCommand = new RelayCommand(async () => await UpdateSpeedAndDistanceAsync());
        }

        public IRelayCommand RefreshCommand { get; }

        private async Task UpdateSpeedAndDistanceAsync()
        {
            var (location, errorMessage) = await _gpsService.GetCurrentLocationAsync();

            if (location != null)
            {
                // Clear any previous error message
                StatusMessage = "";

                // Update speed if available
                double speed = location.Speed ?? 0; // speed in m/s
                CurrentSpeedText = $"{speed * 3.6:F1} km/h"; // Convert m/s to km/h

                // Calculate distance if we have a previous location
                if (_lastLocation != null)
                {
                    double distanceIncrement = CalculateDistance(
                        _lastLocation.Latitude, _lastLocation.Longitude, location.Latitude, location.Longitude);
                    _totalDistance += distanceIncrement;
                    DistanceText = $"{_totalDistance:F2} km";
                }

                // Update last known location
                _lastLocation = location;
            }
            else
            {
                // Display the error message if location retrieval failed
                StatusMessage = errorMessage ?? "Unable to retrieve GPS data.";
            }
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Radius of the Earth in kilometers
            double latDistance = DegreesToRadians(lat2 - lat1);
            double lonDistance = DegreesToRadians(lon2 - lon1);

            double a = Math.Sin(latDistance / 2) * Math.Sin(latDistance / 2) +
                       Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                       Math.Sin(lonDistance / 2) * Math.Sin(lonDistance / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // Distance in kilometers
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
