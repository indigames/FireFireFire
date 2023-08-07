Shader "Custom/ButtonLighting"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ShadowStrengh("ShadowStrengh", float) = 0.2
        _ButtonColor("ButtonColor", Color) = (1,1,1,1)
        _PlatformColor("PlatformColor", Color) = (1,1,1,1)

    }
    SubShader
    {
        Tags { "RenderType"="Opaque"
                "LightMode"="ForwardBase"
                "PassFlags"="OnlyDirectional" }
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

                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ButtonColor;
            float4 _PlatformColor;
            float _ShadowStrengh;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 GetLight(fixed4 color, v2f i)
            {
                float3 normal = normalize(i.worldNormal);
                float NdotL = dot(_WorldSpaceLightPos0, normal);


                return color * max(NdotL, _ShadowStrengh);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 buttonColor = _ButtonColor * col.r;
                fixed4 platformColor = _PlatformColor * col.g;

                col = buttonColor + platformColor;

                col = GetLight(col, i);

                return col;
            }
            ENDCG
        }
    }
}
