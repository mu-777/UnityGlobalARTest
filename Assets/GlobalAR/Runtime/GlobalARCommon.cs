
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
    public struct GeoLocation
    {
        public double Timestamp;

        public double Latitude;
        public float VerticalError;
        public double Longtitude;
        public float HorizontalError;
        public float Altitude;

        public GeoLocation(double timestamp, double latitude, float verticalError, double longtitude, float horizontalError, float altitude)
        {
            Timestamp = timestamp;
            Latitude = latitude;
            VerticalError = verticalError;
            Longtitude = longtitude ;
            HorizontalError = horizontalError;
            Altitude = altitude;
        }

        /// <summary>
        /// 指定した origin を原点とした位置をX,Y,Z(m)に変換します。
        /// </summary>
        /// <param name="origin">原点</param>
        /// <returns>左手系 Y-up (Xが東方向を正、Yが上方向を正、Zが北方向を正)</returns>
        public Vector3 ToVector3(GeoLocation origin)
        {
            return new Vector3(
                       -(float)(GeoLocation.Distance(origin.Latitude, this.Longtitude, origin.Latitude, origin.Longtitude) * Math.Sign(this.Longtitude - origin.Longtitude)),
                       this.Altitude - origin.Altitude,
                       (float)(GeoLocation.Distance(this.Latitude, origin.Longtitude, origin.Latitude, origin.Longtitude) * Math.Sign(Latitude - origin.Latitude))
                   );
        }

        public double Distance(GeoLocation loc)
        {
            return GeoLocation.Distance(loc.Latitude, loc.Longtitude, this.Latitude, this.Longtitude);
        }

        static public double Distance(GeoLocation loc1, GeoLocation loc2)
        {
            return GeoLocation.Distance(loc1.Latitude, loc1.Longtitude, loc2.Latitude, loc2.Longtitude);
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
    public struct GeoBuilding
    {
        public string gmlId;
        public string buildingId;
        public List<GeoLocation> lod0FootPrint;
        public List<List<GeoLocation>> lod1Solid;
    }

    [System.Serializable]
    public struct GeoData
    {
        public GeoLocation lowerCorner;
        public GeoLocation upperCorner;
        public List<GeoBuilding> buildings;
    }
}
