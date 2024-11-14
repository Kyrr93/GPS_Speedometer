using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GPS_Speedometer.Services
{
    public class GpsService
    {
        public async Task<(Location location, string errorMessage)> GetCurrentLocationAsync()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    return (location, null); // Successful location retrieval
                }
                else
                {
                    return (null, "Location data is not available.");
                }
            }
            catch (FeatureNotSupportedException)
            {
                return (null, "GPS functionality is not supported on this device.");
            }
            catch (PermissionException)
            {
                return (null, "Location permissions are not granted. Please enable permissions in settings.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to get location: {ex.Message}");
                return (null, $"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
