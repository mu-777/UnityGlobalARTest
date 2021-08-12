using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalAR
{
    public class GeoMeshBuilder : MonoBehaviour
    {
        public Material GeoMeshMaterial;

        void Start()
        {
            GlobalARSessionManager.Session.OnNewGeoDataLoaded.AddListener(this.AddMesh);
        }

        public void AddMesh(GeoData newGeoData)
        {
            var baseObj = new GameObject(newGeoData.GeoMeshCode.ToString());
            baseObj.transform.parent = this.transform;

            foreach(var bldg in newGeoData.Buildings)
            {
                var originInGeoCoord = GeoLocationManager.Instance.OriginInGeoCoord;
                var go = new GameObject(bldg.BuildingId);
                go.transform.parent = baseObj.transform;
                var mf = go.AddComponent<MeshFilter>();
                var mr = go.AddComponent<MeshRenderer>();
                mr.material = GeoMeshMaterial;

                var combine = new CombineInstance[bldg.Lod1Solid.Count];
                foreach(var(surface, index) in bldg.Lod1Solid.Select((surf, idx) => (surf, idx)))
                {
                    if(GeoMeshGenerator.GeoSurfaceToMesh(surface, bldg.LocalOriginInGeoCoord, out var verts, out var idxes, out var normals))
                    {
                        var mesh = new Mesh();
                        mesh.SetVertices(verts);
                        mesh.SetIndices(idxes.ToArray(), MeshTopology.Triangles, 0);
                        mesh.SetNormals(normals);
                        combine[index].mesh = mesh;
                        combine[index].transform = Matrix4x4.TRS(bldg.LocalOriginInGeoCoord.ToVector3(originInGeoCoord),
                                                                 Quaternion.identity,
                                                                 Vector3.one);
                    }
                    else
                    {
                        Debug.LogWarning("Meshing Failed");
                    }
                }
                mf.mesh = new Mesh();
                mf.mesh.CombineMeshes(combine, true, true);
            }
        }
    }
}
