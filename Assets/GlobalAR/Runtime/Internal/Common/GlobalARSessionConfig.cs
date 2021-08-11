using UnityEngine;
using UnityEditor;

namespace GlobalAR
{
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