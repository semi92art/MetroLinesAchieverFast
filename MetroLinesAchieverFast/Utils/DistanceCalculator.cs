using System;

namespace MetroLinesAchieverFast.Utils
{
    public static class DistanceCalculator
    {
        public static double Calculate(double _Lat1, double _Lon1, double _Lat2, double _Lon2)
        {
            const double r = 6371; // средний радиус Земли в километрах

            double lat1Rad = ToRadians(_Lat1);
            double lon1Rad = ToRadians(_Lon1);
            double lat2Rad = ToRadians(_Lat2);
            double lon2Rad = ToRadians(_Lon2);

            double dLat = lat2Rad - lat1Rad;
            double dLon = lon2Rad - lon1Rad;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Asin(Math.Sqrt(a));

            return Math.Round(r * c, 2);
        }

        private static double ToRadians(double _Degrees)
        {
            return _Degrees * Math.PI / 180;
        }
    }

}