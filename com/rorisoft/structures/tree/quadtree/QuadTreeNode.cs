using com.rorisoft.structures.tree.quadtree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace com.rorisoft.structures.tree.quadtree
{

    public struct Boundary
    {
        public double x;
        public double y;
        public double w;
        public double h;

        public Boundary(double longitude, double latitude, double longitudeRange, double latitudeRange) 
        {
            this.x = longitude;
            this.y = latitude;
            this.w = longitudeRange;
            this.h = latitudeRange;
        }
        public bool contains(Boundary r2)
        {
            if (
                (r2.x + r2.w) <= (this.x + this.w)
                && (r2.x >= this.x)
                && (r2.y >= this.y)
                && (r2.y + r2.h) <= (this.y + this.h)
               )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool contains(double pX, double pY)
        {
            return (pX > this.x && pX < this.x + this.w)
                    && (pY > this.y && pY < this.y + this.h);
        }

        public bool intersects(Boundary rect)
        {
            return !(this.x > rect.x + rect.w) && !(this.x + this.w < rect.x) && !(this.y > rect.y + rect.h) && !(this.y + this.h < rect.y);
            /*
            // If one rectangle is on left side of other  
            if (this.x >= rect.x + rect.w || rect.x >= this.x + this.w)
            {
                return false;
            }

            // If one rectangle is above other  
            if (this.y <= rect.y + rect.h || rect.y <= this.y + this.h)
            {
                return false;
            }
            return true;
            */
        }

    }

    public class QuadTreeNode<TData>
    {

        protected Boundary mBounds;

        /**
     * Represents the top left node of this node
     * ---------
     * | x |   |
     * |---|---|
     * |   |   |
     * ---------
     */
        protected QuadTreeNode<TData> mTopLeftNode;

        /**
         * Represents the top right node of this node
         * ---------
         * |   | x |
         * |---|---|
         * |   |   |
         * ---------
         */
        protected QuadTreeNode<TData> mTopRightNode;

        /**
         * Represents the bottom left node of this node
         * ---------
         * |   |   |
         * |---|---|
         * | x |   |
         * ---------
         */
        protected QuadTreeNode<TData> mBottomLeftNode;

        /**
         * Represents the bottom right node of this node
         * ---------
         * |   |   |
         * |---|---|
         * |   | x |
         * ---------
         */
        protected QuadTreeNode<TData> mBottomRightNode;

        /**
         *  List of points of interest A.K.A neighbours inside this node
         *  this list is only filled in the deepest nodes
         */
        protected List<POI<TData>> mNeighbours = new List<POI<TData>>();

        /**
        * Creates a new node
        * @param latitude node's Y start point
        * @param longitude node's X start point
        * @param latitudeRange node's height
        * @param longitudeRange node's width
        */
        public QuadTreeNode(double latitude, double longitude, double latitudeRange, double longitudeRange)
        {
            mBounds = new Boundary(longitude, latitude, longitudeRange, latitudeRange);
        }

        /**
         * Adds a neighbour in the quadtree.
         * This method will navigate and create nodes if necessary, until the smallest (deepest) node is reached
         * @param neighbour
         */
        public void addNeighbour(POI<TData> neighbour, double deepestNodeSize)
        {
            double halfSize = mBounds.w * .5f;
            if (halfSize < deepestNodeSize)
            {
                mNeighbours.Add(neighbour);
                return;
            }

            QuadTreeNode<TData> node = locateAndCreateNodeForPoint(neighbour.getLatitude(), neighbour.getLongitude());
            node.addNeighbour(neighbour, deepestNodeSize);
        }

        /**
        * Removes a neighbour from the quadtree
        * @param id the neighbour's id
        * @return if the neighbour existed and was removed
        */
        public bool removeNeighbour(string id)
        {
            foreach (POI<TData> neighbor in mNeighbours)
            {
                if (id == neighbor.getId())
                {
                    mNeighbours.Remove(neighbor);
                    return true;
                }
            }

            if (mTopLeftNode != null)
            {
                if (mTopLeftNode.removeNeighbour(id))
                    return true;
            }

            if (mBottomLeftNode != null)
            {
                if (mBottomLeftNode.removeNeighbour(id))
                    return true;
            }

            if (mTopRightNode != null)
            {
                if (mTopRightNode.removeNeighbour(id))
                    return true;
            }

            if (mBottomRightNode != null)
            {
                if (mBottomRightNode.removeNeighbour(id))
                    return true;
            }

            return false;
        }


        /**
         * Recursively search for neighbours inside the given rectangle
         * @param neighbourSet a set to be filled by this method
         * @param rangeAsRectangle the area of interest
         */
        public void findNeighboursWithinRectangle(List<POI<TData>> neighbourSet, Boundary rangeAsRectangle)
        {
            bool end;

            // In case of containing the whole area of interest
            //if (mBounds.contains(rangeAsRectangle))
            if (rangeAsRectangle.contains(mBounds)) //De esta manera podemos asegurar que todos los POIS deben añadirse
            {
                end = true;

                // If end is true, it means that we are on the deepest node
                // otherwise we should keep going deeper

                if (mTopLeftNode != null)
                {
                    mTopLeftNode.findNeighboursWithinRectangle(neighbourSet, rangeAsRectangle);
                    end = false;
                }

                if (mBottomLeftNode != null)
                {
                    mBottomLeftNode.findNeighboursWithinRectangle(neighbourSet, rangeAsRectangle);
                    end = false;
                }

                if (mTopRightNode != null)
                {
                    mTopRightNode.findNeighboursWithinRectangle(neighbourSet, rangeAsRectangle);
                    end = false;
                }

                if (mBottomRightNode != null)
                {
                    mBottomRightNode.findNeighboursWithinRectangle(neighbourSet, rangeAsRectangle);
                    end = false;
                }


                if (end)
                    addNeighbors(true, neighbourSet, rangeAsRectangle);

                return;
            }

            // In case of intersection with the area of interest
            if (mBounds.intersects(rangeAsRectangle))
            {
                end = true;

                // If end is true, it means that we are on the deepest node
                // otherwise we should keep going deeper

                if (mTopLeftNode != null)
                {
                    mTopLeftNode.findNeighboursWithinRectangle(neighbourSet, rangeAsRectangle);
                    end = false;
                }

                if (mBottomLeftNode != null)
                {
                    mBottomLeftNode.findNeighboursWithinRectangle(neighbourSet, rangeAsRectangle);
                    end = false;
                }

                if (mTopRightNode != null)
                {
                    mTopRightNode.findNeighboursWithinRectangle(neighbourSet, rangeAsRectangle);
                    end = false;
                }

                if (mBottomRightNode != null)
                {
                    mBottomRightNode.findNeighboursWithinRectangle(neighbourSet, rangeAsRectangle);
                    end = false;
                }

                if (end)
                    addNeighbors(false, neighbourSet, rangeAsRectangle);
            }
        }

        /**
        * Adds neighbours to the found set
        * @param contains if the rangeAsRectangle is contained inside the node
        * @param neighborSet a set to be filled by this method
        * @param rangeAsRectangle the area of interest
        */
        private void addNeighbors(bool contains, List<POI<TData>> neighborSet, Boundary rangeAsRectangle)
        {
            
            if (contains)
            {
                neighborSet.AddRange(mNeighbours);
                return;
            }
            

            findAll(neighborSet, rangeAsRectangle);
        }

        /**
         * If the rangeAsRectangle is not contained inside this node we must
         * search for neighbours that are contained inside the rangeAsRectangle
         * @param neighborSet a set to be filled by this method
         * @param rangeAsRectangle the area of interest
         */
        private void findAll(List<POI<TData>> neighborSet, Boundary rangeAsRectangle)
        {
            foreach (POI<TData> neighbor in mNeighbours)
            {
                if (rangeAsRectangle.contains(neighbor.getLongitude(), neighbor.getLatitude()))
                    neighborSet.Add(neighbor);
            }
        }

        /**
        * This methods finds and returns in which of the 4 child nodes the latitude and longitude is located.
        * If the node does not exist, it is created.
        *
        * @param latitude
        * @param longitude
        * @return the node that contains the desired latitude and longitude
        */
        protected QuadTreeNode<TData> locateAndCreateNodeForPoint(double latitude, double longitude)
        {
            double halfWidth = mBounds.w * .5f;
            double halfHeight = mBounds.h * .5f;

            if (longitude < mBounds.x + halfWidth)
            {
                if (latitude < mBounds.y + halfHeight)
                    return mTopLeftNode != null ? mTopLeftNode : (mTopLeftNode = new QuadTreeNode<TData>(mBounds.y, mBounds.x, halfHeight, halfWidth));

                return mBottomLeftNode != null ? mBottomLeftNode : (mBottomLeftNode = new QuadTreeNode<TData>(mBounds.y + halfHeight, mBounds.x, halfHeight, halfWidth));
            }

            if (latitude < mBounds.y + halfHeight)
                return mTopRightNode != null ? mTopRightNode : (mTopRightNode = new QuadTreeNode<TData>(mBounds.y, mBounds.x + halfWidth, halfHeight, halfWidth));

            return mBottomRightNode != null ? mBottomRightNode : (mBottomRightNode = new QuadTreeNode<TData>(mBounds.y + halfHeight, mBounds.x + halfWidth, halfHeight, halfWidth));
        }

        public List<POI<TData>> GetAllPOIs()
        {
            List<POI<TData>> pois = [];
            
            if (mTopLeftNode is not null)
            {
                pois.AddRange(mTopLeftNode.GetAllPOIs());
            }
            if (mTopRightNode is not null)
            {
                pois.AddRange(mTopRightNode.GetAllPOIs());
            }
            if (mBottomLeftNode is not null)
            {
                pois.AddRange(mBottomLeftNode.GetAllPOIs());
            }
            if (mBottomRightNode is not null)
            {
                pois.AddRange(mBottomRightNode.GetAllPOIs());
            }

            pois.AddRange(mNeighbours);

            return pois;
        }

        protected double getLongitude()
        {
            return mBounds.x;
        }

        protected double getLatitude()
        {
            return mBounds.y;
        }

        protected double getWidth()
        {
            return mBounds.w;
        }

        protected double getHeight()
        {
            return mBounds.h;
        }

    }
}
