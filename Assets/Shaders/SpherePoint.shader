// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SpherePoint" {
    Properties {
        _Radius ("Sphere Radius", float) = 1.0
    }
    SubShader {
    Pass {
        LOD 200
              
                 
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma geometry geom
        #pragma target 4.0                  // Use shader model 3.0 target, to get nicer looking lighting
        #include "UnityCG.cginc"
  
        struct VertexInput {
            float4 pos : POSITION;
            float4 color: COLOR;
        };
         
        struct VertexOutput {
            float4 pos : SV_POSITION;
            float4 color : COLOR;
            float3 normal : NORMAL;
            float r : TEXCOORD0; // not sure if this is good to do lol
        };

        struct geomOut {
            float4 pos : POSITION;
            float4 color : COLO0R;
            float3 normal : NORMAL;
        };
     
        float rand(float3 p){
            return frac(sin(dot(p.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
        }
        float2x2 rotate2d(float a){
            float s = sin(a);
            float c = cos(a);
            return float2x2(c,-s,s,c);
        }
         
        //Vertex shader: computes normal wrt camera
        VertexOutput vert (VertexInput i) {
            VertexOutput o;
            o.pos = UnityObjectToClipPos(i.pos);
            o.color = i.color;
            o.normal = ObjSpaceViewDir(o.pos);
            o.r = rand(i.pos);// calc random value based on object space pos
            // from world space instead (particles will spin when mesh moves, kinda funny lol)
            //o.r = rand(mul(unity_ObjectToWorld,i.pos));
            return o;
        }
     
        float _Radius;  
        //Geometry shaders: Creates an equilateral triangle with the original vertex in the orthocenter
        [maxvertexcount(3)]
        void geom(point VertexOutput IN[1], inout TriangleStream<geomOut> OutputStream)
        {
            float2 dim = float2(_Radius,_Radius);
     
            float2 p[3];    // equilateral tri
            p[0] = float2(-dim.x, dim.y * .57735026919);
            p[1] = float2(0., -dim.y * 1.15470053838);
            p[2] = float2(dim.x, dim.y * .57735026919);
     
            float2x2 r = rotate2d(IN[0].r * 3.14159);
     
            geomOut OUT;
            OUT.color = IN[0].color;
            OUT.normal = IN[0].normal;
     
            for (int i = 0; i < 3; i++) {
                p[i] = mul(r,p[i]);    // apply rotation
                //p[i] = UnityObjectToClipPos(p[i]);    // apply rotation
                p[i].x *= _ScreenParams.y / _ScreenParams.x; // make square
                OUT.pos = IN[0].pos + float4(p[i],0,0) / 2.;
                OutputStream.Append(OUT);
            }
        }
         
        float4 frag(VertexOutput o) : COLOR {
            return o.color;
            // could do some additional lighting calculation here based on normal
        }
 
        ENDCG
        } 
    }
}