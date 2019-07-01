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

namespace RosSharp.RosBridgeClient
{
    public class PointCloudWriter : MonoBehaviour
    {
        private bool isReceived = false;
        private int height;
        private int width;
        private Messages.Sensor.PointField[] fields;
        private bool is_bigendian;
        private int point_step;
        private int row_step;
        private byte[] data;
        private bool is_dense;
        private PointCloudVisualizer[] PointCloudVisualizers;

        private Vector3[] points;
        private Color[] colors;

        private int offset_x;
        private int offset_y;
        private int offset_z;
        private int offset_rgb;

        private void Update()
        {
            PointCloudVisualizers = GetComponents<PointCloudVisualizer>();
            if (isReceived)
                if(PointCloudVisualizers != null)
                    foreach(PointCloudVisualizer PointCloudVisualizer in PointCloudVisualizers)
                        PointCloudVisualizer.SetSensorData(transform.position, transform.rotation, points, colors, height, width);
            
            isReceived = false;
        }

        public void Write(Messages.Sensor.PointCloud2 PointCloud)
        {
            int step = 1;
            if(PointCloud.width == 160){
                step = 1;
            }else if(PointCloud.width == 320){
                step = 1;
            }else if(PointCloud.width == 640){
                step = 4;
            }else if(PointCloud.width == 1280){
                step = 8;
            }

            height = PointCloud.height/step;
            width = PointCloud.width/step;

            points = new Vector3[height*width];
            colors = new Color[height*width];

            for (int i = 0; i < PointCloud.fields.Length; i++){
                if(PointCloud.fields[i].name == "x")
                    offset_x = PointCloud.fields[i].offset;
                else if(PointCloud.fields[i].name == "y")
                    offset_y = PointCloud.fields[i].offset;
                else if(PointCloud.fields[i].name == "z")
                    offset_z = PointCloud.fields[i].offset;
                else if(PointCloud.fields[i].name == "rgb")
                    offset_rgb = PointCloud.fields[i].offset;
                else
                    Debug.Log("Unknown field name");
            }

            int size = 4;
            byte[] array = new byte[size];
            int cnt = 0;
            float x, y, z;
            float r, g, b, a;
            for (int i = 0; i < PointCloud.width; i+=step)
            {
                for (int j = 0; j < PointCloud.height; j+=step)
                {
                    for (int k = 0; k < size; k++)
                        array[k] = PointCloud.data[PointCloud.row_step * j + PointCloud.point_step * i + offset_x + k];
                    x = System.BitConverter.ToSingle(array, 0);

                    for (int k = 0; k < size; k++)
                        array[k] = PointCloud.data[PointCloud.row_step * j + PointCloud.point_step * i + offset_y + k];
                    y = System.BitConverter.ToSingle(array, 0);

                    for (int k = 0; k < size; k++)
                        array[k] = PointCloud.data[PointCloud.row_step * j + PointCloud.point_step * i + offset_z + k];
                    z = System.BitConverter.ToSingle(array, 0);

                    b = PointCloud.data[PointCloud.row_step * j + PointCloud.point_step * i + offset_rgb + 0] / 255.0f;
                    g = PointCloud.data[PointCloud.row_step * j + PointCloud.point_step * i + offset_rgb + 1] / 255.0f;
                    r = PointCloud.data[PointCloud.row_step * j + PointCloud.point_step * i + offset_rgb + 2] / 255.0f;
                    a = PointCloud.data[PointCloud.row_step * j + PointCloud.point_step * i + offset_rgb + 3] / 255.0f;

                    points[cnt] = new Vector3(-x, y, z);
                    colors[cnt] = new Color(r, g, b, a);

                    cnt++;
                }
            }

            isReceived = true;
        }

    }
}
