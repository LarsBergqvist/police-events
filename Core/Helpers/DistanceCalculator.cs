using System;
namespace Core.Helpers
{
    public static class DistanceCalculator
    {
        public static double GetDistanceKm(double fromLat, double fromLng, double toLat, double toLng)
        {
            const double R = 6371;
            var dLon = ToRad(toLng - fromLng);
            var lat1 = ToRad(fromLat);
            var lat2 = ToRad(toLat);
            var d = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(dLon)) * R;
            return Math.Round(d);
        }

        private static double ToRad(double x)
        {
            return (x * Math.PI) / 180;
        }
    }
}
