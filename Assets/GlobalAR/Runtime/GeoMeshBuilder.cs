using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalAR
{
    public class GeoMeshBuilder : MonoBehaviour
    {
        public Material GeoMeshMaterial;

        private GeoPosition _originGeoPos;

        void Start()
        {
            GlobalARSessionManager.Session.OnNewGeoDataLoaded.AddListener(this.AddMesh);
        }

        public void AddMesh(GeoData newGeoData)
        {
            if (_originGeoPos == null)
            {
                _originGeoPos = newGeoData.LowerCorner;
            }

            var baseObj = new GameObject(newGeoData.GeoMeshCode.ToString());
            baseObj.transform.parent = this.transform;

            var vertices = new List<Vector3>();
            var indices = new List<int>();
            foreach (var bldg in newGeoData.Buildings)
            {
                var go = new GameObject(bldg.BuildingId);
                go.transform.parent = baseObj.transform;
                var mesh = new Mesh();
                var mf = go.AddComponent<MeshFilter>();
                var mr = go.AddComponent<MeshRenderer>();
                mr.material = GeoMeshMaterial;

                vertices.Clear();
                indices.Clear();

                foreach(var surface in bldg.Lod1Solid)
                {
                    if(GeoMeshGenerator.GeoSurfaceToMesh(surface, _originGeoPos, out var verts, out var idxes))
                    {
                        vertices.AddRange(verts);
                        indices.AddRange(idxes);
                    }
                }

                mesh.SetVertices(vertices);
                mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
                mf.sharedMesh = mesh;
            }
        }
    }
}
