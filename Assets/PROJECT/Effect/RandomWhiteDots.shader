Shader"Custom/RandomWhiteDots"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1, 1, 1, 1)
        _DotColor ("Dot Color", Color) = (1, 1, 1, 1)
        _DotDensity ("Dot Density", Range(0, 1)) = 0.5
        _DotSize ("Dot Size", Range(0.001, 0.1)) = 0.01
        _TimeScale ("Time Scale", Range(0.1, 10.0)) = 1.0
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "UnityCG.cginc"

struct appdata_t
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    float4 color : COLOR;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
    float4 color : COLOR;
};

fixed4 _MainColor;
fixed4 _DotColor;
float _DotDensity;
float _DotSize;
float _TimeScale;
sampler2D _MainTex;

v2f vert(appdata_t v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    o.color = v.color * _MainColor;
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
    float time = _Time * _TimeScale;

                // Sample the texture
    fixed4 texColor = tex2D(_MainTex, i.uv);

                // Random noise based on UV coordinates and time
    float noise = frac(sin(dot(i.uv * float2(12.9898, 78.233) + time, float2(12.9898, 78.233))) * 43758.5453);
    float threshold = _DotDensity;

                // If noise value is above threshold, draw a dot
    float dot = step(1.0 - _DotSize, noise) * step(threshold, noise);

                // Combine the main color, texture color, and dot color
    fixed4 col = i.color * texColor;
    col.rgb = lerp(col.rgb, _DotColor.rgb, dot);
    col.a = i.color.a * texColor.a;

    return col;
}
            ENDCG
        }
    }
FallBack"Diffuse"
}
