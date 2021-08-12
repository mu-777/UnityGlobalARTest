
// ref: https://github.com/ksasao/PlateauCityGmlSharp/blob/3bec09115b1c537b3f8809a1ee21baec7af3e781/src/PlateauCityGml/Position.cs

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalAR
{
    public static class GARCommon
    {
        public const string AssetPath = "Assets/GlobalAR/Configs/";
    }

    public static class GARUtils
    {
        public static int Mod(int i, int length)
        {
            var ret = i % length;
            return (ret < 0) ? (length + ret) % length : ret;
        }
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
                       (float)(GeoPosition.Distance(origin.Latitude, this.Longtitude, origin.Latitude, origin.Longtitude) * Math.Sign(this.Longtitude - origin.Longtitude)),
                       this.Altitude - origin.Altitude,
                       (float)(GeoPosition.Distance(this.Latitude, origin.Longtitude, origin.Latitude, origin.Longtitude) * Math.Sign(this.Latitude - origin.Latitude))
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

        public void Translate(Vector3 translation)
        {
            // https://ja.wikipedia.org/wiki/%E5%A4%A7%E5%86%86%E8%B7%9D%E9%9B%A2
            const double earthRadius = 6371009;
            const double deg2rad = Math.PI / 180.0;
            const double rad2deg = 1.0 / deg2rad;

            var offsetX = Mathf.Abs(translation.x);
            var offsetZ = Mathf.Abs(translation.z);

            this.Latitude += Mathf.Sign(translation.z) * rad2deg * (offsetZ / earthRadius);
            this.Longtitude += Mathf.Sign(translation.x) * rad2deg * 2.0 * Math.Asin(Math.Sin(offsetX / (2.0 * earthRadius)) / Math.Abs(Math.Cos(this.Longtitude * deg2rad)));
            this.Altitude += translation.y;
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
        public GeoPosition LocalOriginInGeoCoord;
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

    //ref: https://tech.atware.co.jp/mesh-system/
    //ref: https://qiita.com/yuusei/items/549402a80efd7e7192ef
    public static class GeoDataUtils
    {
        private static int FloorToInt(double x)
        {
            return (int)(Math.Floor(x));
        }

        public static int GeoPositionToMeshCode1st(GeoPosition geoPose)
        {
            return FloorToInt(geoPose.Latitude * 1.5) * 100 + FloorToInt(geoPose.Longtitude - 100);
        }

        public static int GeoPositionToMeshCode2nd(GeoPosition geoPose)
        {
            return GeoPositionToMeshCode1st(geoPose) * 100
                   + FloorToInt(geoPose.Latitude * 12f % 8) * 10 + FloorToInt((geoPose.Longtitude - 100f) * 8) % 8;
        }

        public static int GeoPositionToMeshCode3rd(GeoPosition geoPose)
        {
            return GeoPositionToMeshCode2nd(geoPose) * 100
                   + FloorToInt(geoPose.Latitude * 120f % 10) * 10 + FloorToInt((geoPose.Longtitude - 100f) * 80) % 10;
        }

        /// <summary>
        /// Japan only, support 1st, 2nd and 3rd code
        /// </summary>
        /// <param name="meshCode"></param>
        /// <param name="offsetDown"> north to south offset </param>
        /// <param name="offsetRight"> west to east offset </param>
        /// <returns></returns>
        public static bool OffsetMeshCode(int meshCode, int offsetDown, int offsetRight, out int offsetedMeshCode)
        {
            offsetedMeshCode = 0;
            var digit = meshCode.ToString().Length;
            if(digit == 4) // 1st
            {
                return OffsetMeshCode1st(meshCode, offsetDown, offsetRight, out offsetedMeshCode);
            }
            if(digit == 6) // 2nd
            {
                return OffsetMeshCode2nd(meshCode, offsetDown, offsetRight, out offsetedMeshCode);
            }
            if(digit == 8) // 3rd
            {
                return OffsetMeshCode3rd(meshCode, offsetDown, offsetRight, out offsetedMeshCode);
            }
            return false;
        }

        private static bool OffsetMeshCode1st(int meshCode1st, int offsetDown, int offsetRight, out int offsetedMeshCode)
        {
            // Japan in [3022, 6853]
            if(((meshCode1st % 100) + offsetRight - 22) < (53 - 22)
                    && ((int)(meshCode1st * 0.01) - offsetDown - 30 < (68 - 30)))
            {
                offsetedMeshCode = meshCode1st + offsetRight - offsetDown * 100;
                return true;
            }
            offsetedMeshCode = 0;
            return false;
        }

        private static bool OffsetMeshCode2nd(int meshCode2nd, int offsetDown, int offsetRight, out int offsetedMeshCode)
        {
            var offsetDown2nd = offsetDown % 8;
            var offsetRight2nd = offsetRight % 8;

            var last1stDigit = (meshCode2nd % 10) + offsetRight2nd;
            var last2ndDigit = ((int)(meshCode2nd * 0.1f) % 10) - offsetDown2nd;

            var offsetDown1st = (offsetDown / 8) - Math.Sign(last2ndDigit) * ((Math.Abs(last2ndDigit) / 8 + (last2ndDigit < 0 ? 1 : 0)));
            var offsetRight1st = (offsetRight / 8) + Math.Sign(last1stDigit) * ((Math.Abs(last1stDigit) / 8 + (last1stDigit < 0 ? 1 : 0)));

            if(!OffsetMeshCode1st((int)(meshCode2nd * 0.01f), offsetDown1st, offsetRight1st, out var offseted1st))
            {
                offsetedMeshCode = 0;
                return false;
            }
            offsetedMeshCode = offseted1st * 100 + GARUtils.Mod(last2ndDigit, 8) * 10 + GARUtils.Mod(last1stDigit, 8);
            return true;
        }

        private static bool OffsetMeshCode3rd(int meshCode3rd, int offsetDown, int offsetRight, out int offsetedMeshCode)
        {
            var offsetDown3rd = offsetDown % 10;
            var offsetRight3rd = offsetRight % 10;

            var last1stDigit = (meshCode3rd % 10) + offsetRight3rd;
            var last2ndDigit = ((int)(meshCode3rd * 0.1f) % 10) - offsetDown3rd;

            var offsetDown2nd = (offsetDown / 10) - Math.Sign(last2ndDigit) * ((Math.Abs(last2ndDigit) / 10 + (last2ndDigit < 0 ? 1 : 0)));
            var offsetRight2nd = (offsetRight / 10) + Math.Sign(last1stDigit) * ((Math.Abs(last1stDigit) / 10 + (last1stDigit < 0 ? 1 : 0)));

            if(!OffsetMeshCode2nd((int)(meshCode3rd * 0.01f), offsetDown2nd, offsetRight2nd, out var offseted2nd))
            {
                offsetedMeshCode = 0;
                return false;
            }
            offsetedMeshCode = offseted2nd * 100 + GARUtils.Mod(last2ndDigit, 10) * 10 + GARUtils.Mod(last1stDigit, 10);
            return true;
        }
    }
}
