using System.Collections;
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

        public float Latitude;
        public float VerticalError;
        public float Longtitude;
        public float HorizontalError;
        public float Altitude;

        public GeoLocation(double timestamp, float latitude, float verticalError, float longtitude, float horizontalError, float altitude)
        {
            Timestamp = timestamp;
            Latitude = latitude;
            VerticalError = verticalError;
            Longtitude = longtitude ;
            HorizontalError = horizontalError;
            Altitude = altitude;
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
