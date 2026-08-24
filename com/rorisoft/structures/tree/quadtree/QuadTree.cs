using com.rorisoft.structures.tree.quadtree;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.rorisoft.structures.tree
{

    

    /// <summary>
    /// Estructura optimizada para almacenar coordenadas gps segmentando cuadrantes, 
    /// para a su vez reducir el número de ciclos necesarios para recuperar los puntos en intersección con un perímetro
    /// </summary>
    public class QuadTree<TData>
    {

        private QuadTreeNode<TData> rootNode;
        public QuadTreeInfoModel info;


        public QuadTree()
        {
            rootNode = new QuadTreeNode<TData>(0, 0, QuadTreeConstants.TOTAL_Y_DEGREES, QuadTreeConstants.TOTAL_X_DEGREES);
            info = new QuadTreeInfoModel() { queries = 0, totalPois = 0, status = QuadTreeInfoModel.STATUS.EMPTY };
        }

        public QuadTree(QuadTreeNode<TData> root)
        {
            rootNode = root;
            info = new QuadTreeInfoModel() { queries = 0, totalPois = 0, status = QuadTreeInfoModel.STATUS.EMPTY };
        }

        public void addPOI(POI<TData> poi)
        {
            poi.latitude = normalizeLatitude(poi.latitude);
            poi.longitude = normalizeLongitude(poi.longitude);

            //POI<genericClass> neighbour = new POI<genericClass>(id, normalizeLatitude(latitude), normalizeLongitude(longitude), data);

            rootNode.addNeighbour(poi, QuadTreeConstants.QUADTREE_LAST_NODE_SIZE_IN_DEGREE());

            info.totalPois++;
        }


        public void removePOI(string id)
        {
            bool r = rootNode.removeNeighbour(id);
            if (r)
                info.totalPois--;
        }

        public long GetTotalPOIS()
        {
            return info.totalPois;
        }

        public List<POI<TData>> findNeighbours(double latitude, double longitude, double rangeInKm)
        {
            List<POI<TData>> neighbourSet = new List<POI<TData>>();
            double rangeInDegrees = QuadTreeConstants.kmToDegree(rangeInKm);
            Boundary areaOfInterest = getRangeAsRectangle(normalizeLatitude(latitude), normalizeLongitude(longitude), rangeInDegrees);
            rootNode.findNeighboursWithinRectangle(neighbourSet, areaOfInterest);
            return neighbourSet;
        }

        public List<POI<TData>> findNeighbours(double minLat, double minLon, double maxLat, double maxLon)
        {
            List<POI<TData>> neighbourSet = new List<POI<TData>>();
            Boundary areaOfInterest = getRectangle(normalizeLatitude(minLat), normalizeLongitude(minLon), normalizeLatitude(maxLat) - normalizeLatitude(minLat), normalizeLongitude(maxLon) - normalizeLongitude(minLon));
            rootNode.findNeighboursWithinRectangle(neighbourSet, areaOfInterest);
            return neighbourSet;
        }

        public  Boundary getRectangle(double lat1, double lon1, double latRange, double lonRange)
        {
            return new Boundary(lon1, lat1, lonRange, latRange);
        }

        private Boundary getRangeAsRectangle(double latitude, double longitude, double range)
        {
            /*
               We need to centralize the point and have the range on every direction
             */
            return new Boundary(Math.Max(longitude - range, 0),
                    Math.Max(latitude - range, 0),
                    Math.Min(range * 2, QuadTreeConstants.TOTAL_X_DEGREES),
                    Math.Min(range * 2, QuadTreeConstants.TOTAL_Y_DEGREES));
        }

        protected QuadTreeNode<TData> getRootNode()
        {
            return rootNode;
        }

        private double normalizeLatitude(double latitude)
        {
            return latitude + QuadTreeConstants.NORMALIZE_Y;
        }

        private double normalizeLongitude(double longitude)
        {
            return longitude + QuadTreeConstants.NORMALIZE_X;
        }

        /// <summary>
        ///  Iterates through all the nodes of the quadtree and returns a list with all the pois 
        ///  (all the neighbors of all quadrants) 
        /// </summary>
        /// <returns></returns>
        public List<POI<TData>> GetAllPOIs()
        {
            return rootNode.GetAllPOIs();
        }

    }


}
