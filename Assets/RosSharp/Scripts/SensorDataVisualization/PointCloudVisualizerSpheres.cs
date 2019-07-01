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

namespace RosSharp.SensorVisualization
{
    public class PointCloudVisualizerSpheres : PointCloudVisualizer
    {
        [Range(0.01f, 0.1f)]
        public float objectWidth;

        private GameObject[] PointCloud;
        private bool IsCreated = false;

	    public Material matVertex;

        private void Create(int numOfSpheres)
        {
            PointCloud = new GameObject[numOfSpheres];

            for (int i = 0; i < numOfSpheres; i++)
            {
                PointCloud[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                DestroyImmediate(PointCloud[i].GetComponent<Collider>());                    
                PointCloud[i].name = "PointCloudSpheres";
                PointCloud[i].transform.parent = transform;
                //PointCloud[i].GetComponent<Renderer>().material = new Material(Shader.Find("Particles/Additive"));
                PointCloud[i].GetComponent<Renderer>().material = matVertex;
            }
            IsCreated = true;
        }

        protected override void Visualize()
        {
            if (!IsCreated)
                Create(directions.Length);

            for (int i = 0; i < directions.Length; i++)
            {
                PointCloud[i].SetActive(ranges[i] != 0);
                //PointCloud[i].GetComponent<Renderer>().material.SetColor("_TintColor", GetColor(ranges[i]));
                PointCloud[i].GetComponent<Renderer>().material.SetColor("_TintColor", colors[i]);
                PointCloud[i].transform.localScale = objectWidth * Vector3.one;
                PointCloud[i].transform.localPosition = ranges[i] * directions[i];
            }
        }

        protected override void DestroyObjects()
        {
            for (int i = 0; i < PointCloud.Length; i++)
                Destroy(PointCloud[i]);
            IsCreated = false;
        }

    }
}
