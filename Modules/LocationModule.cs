
using NetTopologySuite.Geometries;
using ProjNet;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System;
using System.Collections.Generic;

namespace POPNetwork.Modules;
public static class LocationModule
{
    private static readonly CoordinateSystemServices _coordinateSystemServices
           = new CoordinateSystemServices(
               new Dictionary<int, string>
               {
                // Coordinate systems:

                [4326] = GeographicCoordinateSystem.WGS84.WKT,

                // This coordinate system covers the area of our data.
                // Different data requires a different coordinate system.
                [2855] =
                       @"
                    PROJCS[""NAD83(HARN) / Washington North"",
                        GEOGCS[""NAD83(HARN)"",
                            DATUM[""NAD83_High_Accuracy_Regional_Network"",
                                SPHEROID[""GRS 1980"",6378137,298.257222101,
                                    AUTHORITY[""EPSG"",""7019""]],
                                AUTHORITY[""EPSG"",""6152""]],
                            PRIMEM[""Greenwich"",0,
                                AUTHORITY[""EPSG"",""8901""]],
                            UNIT[""degree"",0.01745329251994328,
                                AUTHORITY[""EPSG"",""9122""]],
                            AUTHORITY[""EPSG"",""4152""]],
                        PROJECTION[""Lambert_Conformal_Conic_2SP""],
                        PARAMETER[""standard_parallel_1"",48.73333333333333],
                        PARAMETER[""standard_parallel_2"",47.5],
                        PARAMETER[""latitude_of_origin"",47],
                        PARAMETER[""central_meridian"",-120.8333333333333],
                        PARAMETER[""false_easting"",500000],
                        PARAMETER[""false_northing"",0],
                        UNIT[""metre"",1,
                            AUTHORITY[""EPSG"",""9001""]],
                        AUTHORITY[""EPSG"",""2855""]]
                "
               });

    public static Point ProjectTo(this Point geometry, int srid)
    {
        var transformation = _coordinateSystemServices.CreateTransformation(geometry.SRID, srid);

        Point result = (Point) geometry.Copy();
        result.Apply(new MathTransformFilter(transformation.MathTransform));

        return result;
    }

    public static Geometry ProjectTo(this Geometry geometry, int srid)
    {
        var transformation = _coordinateSystemServices.CreateTransformation(geometry.SRID, srid);

        Geometry result = (Point)geometry.Copy();
        result.Apply(new MathTransformFilter(transformation.MathTransform));

        return result;
    }

    private class MathTransformFilter : ICoordinateSequenceFilter
    {
        private readonly MathTransform _transform;

        public MathTransformFilter(MathTransform transform)
            => _transform = transform;

        public bool Done => false;
        public bool GeometryChanged => true;

        public void Filter(CoordinateSequence seq, int i)
        {
            var x = seq.GetX(i);
            var y = seq.GetY(i);
            var z = seq.GetZ(i);
            _transform.Transform(ref x, ref y, ref z);
            seq.SetX(i, x);
            seq.SetY(i, y);
            seq.SetZ(i, z);
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
