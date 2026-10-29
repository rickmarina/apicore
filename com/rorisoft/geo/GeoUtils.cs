using System;
using System.Collections.Generic;
using System.Text;

namespace com.rorisoft.geo
{
    public static class GeoUtils
    {

        private const int EARTH_RADIUS = 6371; // Radius of the earth in km



        public static double geoDistanciaKm(double lon1, double lat1, double lon2, double lat2)
        {
            //return geoDistanciaKm(utils.ParseUtils.parseDecimal(lon1), utils.ParseUtils.parseDecimal(lat1), utils.ParseUtils.parseDecimal(lon2), utils.ParseUtils.parseDecimal(lat2));
            double dlat = deg2rad(lat2 - lat1);
            double dlon = deg2rad(lon2 - lon1);
            double a = Math.Sin(dlat / 2) * Math.Sin(dlat / 2) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Sin(dlon / 2) * Math.Sin(dlon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = EARTH_RADIUS * c;

            return d;
        }

        public static int geoDistanciaMeters(double lon1, double lat1, double lon2, double lat2)
        {
            return (int)(GeoUtils.geoDistanciaKm(lon1, lat1, lon2, lat2) * 1000);
        }

        public static double geoDistanciaKm(decimal lon1, decimal lat1, decimal lon2, decimal lat2)
        {
            //$dLat = deg2rad($lat2 - $lat1);
            double dlat = deg2rad((double)(lat2 - lat1));
            //$dLon = deg2rad($lon2 - $lon1);
            double dlon = deg2rad((double)(lon2 - lon1));

            //$a = sin($dLat / 2) * sin($dLat / 2) + cos(deg2rad($lat1)) * cos(deg2rad($lat2)) * sin($dLon / 2) * sin($dLon / 2);
            double a = Math.Sin(dlat / 2) * Math.Sin(dlat / 2) + Math.Cos(deg2rad((double)lat1)) * Math.Cos(deg2rad((double)lat2)) * Math.Sin(dlon / 2) * Math.Sin(dlon / 2);

            //$c = 2 * atan2(sqrt($a), sqrt(1 -$a));
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            //$d = $R * $c; // Distance in km
            double d = EARTH_RADIUS * c;

            return d;
        }

        /// <summary>
        /// Calcula el lado de un punto respecto a una línea definida por dos puntos
        /// </summary>
        /// <param name="x1">Coordenada X del primer punto de la línea</param>
        /// <param name="y1">Coordenada Y del primer punto de la línea</param>
        /// <param name="x2">Coordenada X del segundo punto de la línea</param>
        /// <param name="y2">Coordenada Y del segundo punto de la línea</param>
        /// <param name="x">Coordenada X del punto a evaluar</param>
        /// <param name="y">Coordenada Y del punto a evaluar</param>
        /// <returns></returns>
        public static SIDES Side(double x1, double y1, double x2, double y2, double x, double y) => (SIDES)Math.Sign(Cross2D(x2 - x1, y2 - y1, x - x1, y - y1));

        /// <summary>
        /// Calcula el producto cruzado de dos vectores 2D
        /// </summary>
        /// <param name="x1">Coordenada X del primer vector</param>
        /// <param name="y1">Coordenada Y del primer vector</param>
        /// <param name="x2">Coordenada X del segundo vector</param>
        /// <param name="y2">Coordenada Y del segundo vector</param>
        /// <returns>El producto cruzado de los dos vectores</returns>
        public static double Cross2D(double x1, double y1, double x2, double y2) => x1 * y2 - y1 * x2;

        /// <summary>
        /// Convierte grados a radianes
        /// </summary>
        /// <param name="n">Valor en grados</param>
        /// <returns>Valor en radianes</returns>
        private static double deg2rad(double n) { return Math.PI * n / 180; }


    }
}
