
// ref: https://github.com/ksasao/PlateauCityGmlSharp/blob/3bec09115b1c537b3f8809a1ee21baec7af3e781/src/PlateauCityGml/Position.cs

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalAR
{
    public static class GlobalARCommon
    {
        public const string AssetPath = "Assets/GlobalAR/Configs/";
    }

    public enum GARResult
    {
        SUCCESS = 0,
        ERROR = 1,
    }

    [System.Serializable]
    public class GeoPosition
    {
        public double Latitude;
        public double Longtitude;
        public float Altitude;

        public GeoPosition(double latitude, double longtitude, float altitude)
        {
            this.Latitude = latitude;
            this.Longtitude = longtitude;
            this.Altitude = altitude;
        }
        /// <summary>
        /// 指定した origin を原点とした位置をX,Y,Z(m)に変換します。
        /// </summary>
        /// <param name="origin">原点</param>
        /// <returns>左手系 Y-up (Xが東方向を正、Yが上方向を正、Zが北方向を正)</returns>
        public Vector3 ToVector3(GeoPosition origin)
        {
            return new Vector3(
                       -(float)(GeoPosition.Distance(origin.Latitude, this.Longtitude, origin.Latitude, origin.Longtitude) * Math.Sign(this.Longtitude - origin.Longtitude)),
                       this.Altitude - origin.Altitude,
                       (float)(GeoPosition.Distance(this.Latitude, origin.Longtitude, origin.Latitude, origin.Longtitude) * Math.Sign(Latitude - origin.Latitude))
                   );
        }

        public double Distance(GeoPosition pos)
        {
            return GeoPosition.Distance(pos.Latitude, pos.Longtitude, this.Latitude, this.Longtitude);
        }

        static public double Distance(GeoPosition pos1, GeoPosition pos2)
        {
            return GeoPosition.Distance(pos1.Latitude, pos1.Longtitude, pos2.Latitude, pos2.Longtitude);
        }

        static public double Distance(double lat1, double lon1, double lat2, double lon2)
        {
            return Haversine(lat1, lon1, lat2, lon2);
        }

        /// <summary>
        /// Haversineの式により2点間の距離を求める
        /// </summary>
        /// <param name="latitude1">緯度1(°)</param>
        /// <param name="longitude1">経度1(°)</param>
        /// <param name="latitude2">緯度2(°)</param>
        /// <param name="longitude2">経度2(°)</param>
        /// <returns>2点間の距離(m)</returns>
        static private double Haversine(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            // https://ja.wikipedia.org/wiki/%E5%A4%A7%E5%86%86%E8%B7%9D%E9%9B%A2
            const double earthRadius = 6371009;
            const double deg2rad = Math.PI / 180.0;
            double lat1 = latitude1 * deg2rad;
            double lat2 = latitude2 * deg2rad;
            double lon1 = longitude1 * deg2rad;
            double lon2 = longitude2 * deg2rad;

            double dlat = Math.Abs(lat1 - lat2);
            double dlon = Math.Abs(lon1 - lon2);
            double ds = 2.0 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(dlat / 2.0), 2.0)
                                                  + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dlon / 2.0), 2.0)
                                                 ));
            return earthRadius * ds;
        }
    }

    [System.Serializable]
    public struct GeoLocation
    {
        public double Timestamp;
        public GeoPosition GeoPos;
        public float VerticalError;
        public float HorizontalError;

        public GeoLocation(double timestamp, GeoPosition geoPos, float verticalError, float horizontalError)
        {
            this.Timestamp = timestamp;
            this.GeoPos = geoPos;
            this.VerticalError = verticalError;
            this.HorizontalError = horizontalError;
        }
    }

    [System.Serializable]
    public struct GeoSurface
    {
        public List<GeoPosition> Points;
        public GeoSurface(List<GeoPosition> points)
        {
            this.Points = points;
        }

        public GeoPosition this[int idx]
        {
            set { this.Points[idx] = value; }
            get { return this.Points[idx]; }
        }
    }

    [System.Serializable]
    public struct GeoBuilding
    {
        public string GmlId;
        public string BuildingId;
        public GeoSurface Lod0FootPrint;
        public List<GeoSurface> Lod1Solid;
    }

    [System.Serializable]
    public struct GeoData
    {
        /// <summary>
        /// 3rd mesh code
        /// </summary>
        public int GeoMeshCode;
        public GeoPosition LowerCorner;
        public GeoPosition UpperCorner;
        public List<GeoBuilding> Buildings;

        public GeoData(int geoMeshCode, GeoPosition lowerCorner, GeoPosition upperCorner, List<GeoBuilding> buildings)
        {
            this.GeoMeshCode = geoMeshCode;
            this.LowerCorner = lowerCorner;
            this.UpperCorner = upperCorner;
            this.Buildings = buildings;
        }
    }
}
