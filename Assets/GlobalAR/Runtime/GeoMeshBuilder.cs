using System;
using System.Linq;
using System.Collections;
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
            StartCoroutine(MeshingCoroutine(newGeoData.Buildings, baseObj));
        }

        private IEnumerator MeshingCoroutine(List<GeoBuilding> buildings, GameObject baseObj)
        {
            foreach (var bldg in buildings)
            {
                var bldgObj = new GameObject(bldg.BuildingId);
                bldgObj.transform.parent = baseObj.transform;

                var localOriginPos = bldg.LocalOriginInGeoCoord.ToVector3(GeoLocationManager.Instance.OriginInGeoCoord);
                bldgObj.transform.localPosition = localOriginPos;
                var mf = bldgObj.AddComponent<MeshFilter>();
                var mr = bldgObj.AddComponent<MeshRenderer>();
                mr.material = GeoMeshMaterial;

                var combine = new CombineInstance[bldg.Lod1Solid.Count];
                foreach (var (surface, index) in bldg.Lod1Solid.Select((surf, idx) => (surf, idx)))
                {
                    if (GeoMeshGenerator.GeoSurfaceToMesh(surface, bldg.LocalOriginInGeoCoord, out var verts, out var idxes, out var normals))
                    {
                        var mesh = new Mesh();
                        mesh.SetVertices(verts);
                        mesh.SetIndices(idxes.ToArray(), MeshTopology.Triangles, 0);
                        mesh.SetNormals(normals);
                        combine[index].mesh = mesh;
                        combine[index].transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                        //combine[index].transform = Matrix4x4.TRS(localOriginPos, Quaternion.identity, Vector3.one);
                    }
                    else
                    {
                        Debug.LogWarning("Meshing Failed");
                    }
                }
                mf.mesh = new Mesh();
                mf.mesh.CombineMeshes(combine, true, true);
                yield return null;
            }
        }
    }
}
