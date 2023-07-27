Shader "Unlit/IcePlatform"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SnowColor("Snow Color", Color) = (1,1,1,1)
        _SnowOffset("Snow Offset", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 objNormal: NORMAL0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _SnowColor;
            fixed _SnowOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.objNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed snowPatterm = saturate(i.objNormal.y + _SnowOffset.xxx);
                col = lerp(col,(_SnowColor * snowPatterm), snowPatterm);
                return col;
            }
            ENDCG
        }
    }
}
