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

public abstract class PointCloudVisualizer : MonoBehaviour
{
    protected Vector3 origin;
    protected Quaternion rotation;
    protected Vector3[] directions;
    protected float range_min;
    protected float range_max;
    protected float[] ranges;
    protected int height;
    protected int width;
    protected Vector3[] points;
    protected Color[] colors;

    protected bool IsNewSensorDataReceived;
    protected bool IsVisualized = false;

    abstract protected void Visualize();
    abstract protected void DestroyObjects();

    protected void Update()
    {
        if (!IsNewSensorDataReceived)
            return;

        IsNewSensorDataReceived = false;
        Visualize();
    }

    protected void OnDisable()
    {
        DestroyObjects();
    }

    public void SetSensorData(Vector3 _origin, Quaternion _rotation, Vector3[] _points, Color[] _colors, int _height, int _width)
    {
        origin = _origin;
        rotation = _rotation;
        height = _height;
        width = _width;
        ranges = new float[_points.Length];
        directions = new Vector3[_points.Length];
        points = new Vector3[_points.Length];
        colors = new Color[_colors.Length];

        range_min = 0.5f;
        range_max = 4.0f;

        for (int i = 0; i < _points.Length; i++)
        {
            points[i] = _points[i];
            ranges[i] = points[i].magnitude;
            directions[i] = points[i].normalized;
        }
        for (int i = 0; i < _colors.Length; i++)
        {
            colors[i] = _colors[i];
        }
        IsNewSensorDataReceived = true;
    }

    protected Color GetColor(float distance)
    {
        float h_min = (float)0;
        float h_max = (float)0.5;

        float h = (float)(h_min + (distance - range_min) / (range_max - range_min) * (h_max - h_min));
        float s = (float)1.0;
        float v = (float)1.0;

        return Color.HSVToRGB(h, s, v);
    }

}
