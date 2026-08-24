using System;
using System.Collections.Generic;
using System.Text;

namespace com.rorisoft.structures.tree.quadtree
{
    /// <summary>
    /// Interfaz genérica de puntos de interés, por defecto un id y su geoposición (longitud y latitud)
    /// </summary>
    public interface IPoi
    {

        public string getId();
        public double getLatitude();
        public double getLongitude(); 
    }
}
