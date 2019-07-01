/*
© Siemens AG, 2018
Author: Berkay Alp Cakal (berkay_alp.cakal.ct@siemens.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using UnityEngine;

public class PointCloudVisualizerMesh : PointCloudVisualizer
{
    private GameObject PointCloud;
    private Mesh mesh;
    private Vector3[] meshVertices;
    private Color[] meshVertexColors;
    private int[] meshTriangles;
    private Vector2[] meshUV;
    private Vector3[] meshNormals;
    private bool IsCreated = false;

	public Material matVertex;

    private void Create()
    {
        PointCloud = new GameObject("PointCloudMesh");
        PointCloud.transform.position = origin;
        PointCloud.transform.rotation = rotation;
        //PointCloud.transform.parent = gameObject.transform;
        PointCloud.transform.parent = transform;
        PointCloud.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = PointCloud.AddComponent<MeshRenderer>();
        //meshRenderer.material = new Material(Shader.Find("Particles/Additive"));
        meshRenderer.material = matVertex;

        mesh = PointCloud.GetComponent<MeshFilter>().mesh;
        meshVertices = new Vector3[directions.Length];
        meshTriangles = new int[3 * 2 * (height-1) * (width-1)];
        //meshTriangles = new int[3 * 2 * (height) * (width)];
        meshVertexColors = new Color[directions.Length];
        meshUV = new Vector2[directions.Length];
        meshNormals = new Vector3[directions.Length];
        
        IsCreated = true;
    }

    protected override void Visualize()
    {
        if (!IsCreated)
            Create();

		int[] indices = new int[meshVertices.Length];
        for (int i = 0; i < meshVertices.Length; i++)
        {
            meshVertices[i] = ranges[i] * directions[i];
            //meshVertexColors[i] = GetColor(ranges[i]); // To visualize depth color (Red: close, Blue: far)
            meshVertexColors[i] = colors[i]; // To visualize image color (RGB)
			indices[i] = i;
        }

        float rmin = 0.1f; // Minimum range value to include the point in the mesh
        float rmean = 0.0f;
        float emax = 0.05f; // Maximum error (%) to consider two points in the same object
        float dmax = 0.0f;
        float d0 = 0.0f, d1 = 0.0f, d2 = 0.0f;
        int i0 = 0, i1 = 0, i2 = 0;
        int iter = 0;
        for (int i = 0; i < (width-1); i++)
        {
            for (int j = 0; j < (height-1); j++)
            {
                // Indexes for each vertex of the triangle (connexion with previous row)
                i0 = i*height + j;
                i1 = (i+1)*height + j;
                i2 = i*height + j + 1;
                // Check that all three vertexes exist (value above threshold)
                if((ranges[i0] > rmin) && (ranges[i1] > rmin) && (ranges[i2] > rmin))
                {
                    d0 = (meshVertices[i0] - meshVertices[i1]).magnitude;
                    d1 = (meshVertices[i0] - meshVertices[i2]).magnitude;
                    d2 = (meshVertices[i1] - meshVertices[i2]).magnitude;
                    //rmean = 1.0f;
                    //rmean = (ranges[i0] + ranges[i1] + ranges[i2])/3.0f;
                    rmean = Mathf.Min(Mathf.Min(ranges[i0],ranges[i1]),ranges[i2]);
                    // Apply a correction factor 'emax' to the mean range distance 'rmean'
                    // of the three points, in order to generate a new triangle or not
                    dmax = Mathf.Clamp(emax*rmean, 1.0f, 0.05f); // Upper-Lower saturation
                    if((d0 < dmax) && (d1 < dmax) && (d2 < dmax))
                    {
                        meshTriangles[3 * iter + 0] = i0;
                        meshTriangles[3 * iter + 1] = i1;
                        meshTriangles[3 * iter + 2] = i2;
                        iter++;
                    }
                }
                // Indexes for each vertex of the triangle (connexion with next row)
                i0 = i*height + j + 1;
                i1 = (i+1)*height + j;
                i2 = (i+1)*height + j + 1;
                // Check that all three vertexes exist (value above threshold)
                if((ranges[i0] > rmin) && (ranges[i1] > rmin) && (ranges[i2] > rmin))
                {
                    d0 = (meshVertices[i0] - meshVertices[i1]).magnitude;
                    d1 = (meshVertices[i0] - meshVertices[i2]).magnitude;
                    d2 = (meshVertices[i1] - meshVertices[i2]).magnitude;
                    //rmean = 1.0f;
                    //rmean = (ranges[i0] + ranges[i1] + ranges[i2])/3.0f;
                    rmean = Mathf.Min(Mathf.Min(ranges[i0],ranges[i1]),ranges[i2]);
                    // Apply a correction factor 'emax' to the mean range distance 'rmean'
                    // of the three points, in order to generate a new triangle or not
                    dmax = Mathf.Clamp(emax*rmean, 1.0f, 0.05f); // Upper-Lower saturation
                    if((d0 < dmax) && (d1 < dmax) && (d2 < dmax))
                    {
                        meshTriangles[3 * iter + 0] = i0;
                        meshTriangles[3 * iter + 1] = i1;
                        meshTriangles[3 * iter + 2] = i2;
                        iter++;
                    }
                }
            }
        }

        mesh.vertices = meshVertices;
        mesh.triangles = meshTriangles;
        mesh.colors = meshVertexColors;
        //mesh.uv = meshUV;
        //mesh.normals = meshNormals;
        //mesh.SetIndices(indices, MeshTopology.Points, 0);
    }

    protected override void DestroyObjects()
    {
        Destroy(PointCloud);
        IsCreated = false;
    }
}
