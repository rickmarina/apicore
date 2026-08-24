using com.rorisoft.structures.tree.quadtree;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.rorisoft.structures.tree.quadtree
{


    public class POI<T> :IPoi //where T : class?, new()
    {

        public string id { get; set; }
        public string? rotulo { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }

        public T data { get; set; }

        public POI(string id, double latitude, double longitude, T data)
        {
            this.id = id;
            this.latitude = latitude;
            this.longitude = longitude;

            this.data = data;
        }

        public string getId() => this.id;

        public double getLatitude() => this.latitude;

        public double getLongitude() => this.longitude;

        public double UnnormalizeLatitude() => this.latitude - QuadTreeConstants.NORMALIZE_Y;

        public double UnnormalizeLongitude() => this.longitude - QuadTreeConstants.NORMALIZE_X;


    }
}
