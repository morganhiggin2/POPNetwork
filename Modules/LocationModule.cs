
using NetTopologySuite.Geometries;
using POPNetwork.Controllers;
using POPNetwork.Models;
using ProjNet;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace POPNetwork.Modules;
public static class LocationModule
{
    public static void DeleteOldLocations(ApplicationDbContext context)
    {
        //default location
        Point defaultLocation = new Point(90, 0) { SRID = 4326 };

        //get datetime 6 month limit
        DateTime limit = DateTime.Now;
        limit = limit.AddMonths(-6);

        List<FriendUser> friendUsers = context.FriendUsers.Where(u => u.lastActive <= limit && u.location != defaultLocation).ToList();

        foreach(FriendUser friendUser in friendUsers)
        {
            friendUser.location = defaultLocation;
        }
    }

    /// <summary>
    /// Calculate haversine distance between two latitude and longitude points.
    /// X is latitiude and Y is longitude.
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns>double</returns>
    public static double haversineDistance(Point p1, Point p2)
    {
        double p = Math.PI / 180;
        double a = 0.5 - Math.Cos((p2.Y - p1.Y) * p) / 2 + Math.Cos(p1.Y * p) * Math.Cos(p2.Y * p) * (1 - Math.Cos((p2.X - p1.X) * p)) / 2;

        return 7926.3812 * Math.Asin(Math.Sqrt(a)); //2*R*asin... R = 6371 km


        //one liner:
        //7926.3812 * Math.Asin(Math.Sqrt(0.5 - Math.Cos((p2.X - p1.X) * Math.PI / 180) / 2 + Math.Cos(p1.X * Math.PI / 180) * Math.Cos(p2.X * Math.PI / 180) * (1 - Math.Cos((p2.Y - p1.Y) * Math.PI / 180)) / 2));
    }

    public class GeoCoordinate
    {
        /// <summary>
        /// latitude
        /// </summary>
        public double latitude { get; set; }

        /// <summary>
        /// longitude
        /// </summary>
        public double longitude { get; set; } 

        public GeoCoordinate(double _latitude, double _longitude)
        {
            this.latitude = _latitude;
            this.longitude = _longitude;
        }

        /// <summary>
        /// gets distance from this coordinate to other coordinate
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double distance(GeoCoordinate other)
        {
            double p = Math.PI / 180;
            double a = 0.5 - Math.Cos((other.latitude - this.latitude) * p) / 2 + Math.Cos(this.latitude * p) * Math.Cos(other.latitude * p) * (1 - Math.Cos((other.longitude - this.longitude) * p)) / 2;
            return 7926.3812 * Math.Asin(Math.Sqrt(a)); //2*R*asin... R = 6371 km
        }

        /// <summary>
        /// gets distance from this coordinate to other coordinate
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        public double distance(double lat, double lon)
        {
            double p = Math.PI / 180;
            double a = 0.5 - Math.Cos((lat - this.latitude) * p) / 2 + Math.Cos(this.latitude * p) * Math.Cos(lat * p) * (1 - Math.Cos((lon - this.longitude) * p)) / 2;
            return 7926.3812 * Math.Asin(Math.Sqrt(a)); //2*R*asin... R = 6371 km
        }

        public override bool Equals(object obj) => this.Equals(obj as GeoCoordinate);

        public bool Equals(GeoCoordinate other)
        {
            if (other is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != other.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (this.latitude == other.latitude) && (this.longitude == other.longitude);
        }

        public override int GetHashCode()
        {
            return latitude.GetHashCode() * longitude.GetHashCode();
        }

        public static bool operator == (GeoCoordinate lhs, GeoCoordinate rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator != (GeoCoordinate lhs, GeoCoordinate rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return !lhs.Equals(rhs);
        }
    }
}
