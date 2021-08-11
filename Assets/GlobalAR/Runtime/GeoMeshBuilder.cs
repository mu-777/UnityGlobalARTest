using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalAR
{
    public class GeoMeshBuilder : MonoBehaviour
    {
        public Transform origin;

        void Start()
        {
            GlobalARSessionManager.Session.OnNewGeoDataLoaded.AddListener(this.AddMesh);
        }

        public void AddMesh(GeoData newGeoData)
        {

        }
    }
}
