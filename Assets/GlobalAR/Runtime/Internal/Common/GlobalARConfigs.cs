using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace GlobalAR
{
    public class MockGeoLocationEstimatorConfig : ScriptableObject
    {
        public GeoLocation MockData;

        [MenuItem("ScriptableObjects/MockGeoLocationEstimatorConfig")]
        private static void Create()
        {
            var confData = CreateInstance<MockGeoLocationEstimatorConfig>();
            var assetName = $"{GlobalARCommon.AssetPath}/MockGeoLocationEstimatorConfig.asset";
            AssetDatabase.CreateAsset(confData, assetName);
            AssetDatabase.Refresh();
        }
    }

    public class LocalGeoDataLoaderConfig : ScriptableObject
    {
        public string GmlDirPath;

        [MenuItem("ScriptableObjects/LocalGeoDataLoaderConfig")]
        private static void Create()
        {
            var confData = CreateInstance<LocalGeoDataLoaderConfig>();
            var assetName = $"{GlobalARCommon.AssetPath}/LocalGeoDataLoaderConfig.asset";
            AssetDatabase.CreateAsset(confData, assetName);
            AssetDatabase.Refresh();
        }
    }

    public class GlobalARSessionConfig : ScriptableObject
    {
        public float GeoLocUpdateIntervalSec = 3.0f;
        public float GeoLocConvergenceErrThreshold = 0.1f;
        public float CoordAlignmentTimeoutSec = 5f;

        [MenuItem("ScriptableObjects/GlobalARSessionConfig")]
        private static void Create()
        {
            var confData = CreateInstance<GlobalARSessionConfig>();
            var assetName = $"{GlobalARCommon.AssetPath}/GlobalARSessionConfig.asset";
            AssetDatabase.CreateAsset(confData, assetName);
            AssetDatabase.Refresh();
        }
    }
}