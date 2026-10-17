Shader "Colonization/Hex Artwork Fit"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _ArtworkTex ("Original Artwork", 2D) = "white" {}
        _RowSpans ("Illustration Row Bounds", 2D) = "white" {}
        _Canvas ("Canvas Width, Height, PPU, Ground Height", Vector) = (256,384,100,256)
        _UVToCanvasX ("Sprite UV To Canvas X", Vector) = (1,0,0,0)
        _UVToCanvasY ("Sprite UV To Canvas Y", Vector) = (0,1,0,0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "CanUseSpriteAtlas"="False" }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _ArtworkTex;
            sampler2D _RowSpans;
            float4 _Canvas;
            float4 _UVToCanvasX;
            float4 _UVToCanvasY;

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };
            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 tile : TEXCOORD0;
                fixed4 color : COLOR;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.position = UnityObjectToClipPos(input.vertex);
                float3 uv = float3(input.texcoord, 1);
                float2 spriteUV = float2(dot(uv, _UVToCanvasX.xyz), dot(uv, _UVToCanvasY.xyz));
                output.tile = spriteUV * float2(1, _Canvas.y / _Canvas.w);
                output.color = input.color;
                return output;
            }

            fixed4 frag(Varyings input) : SV_Target
            {
                float y = input.tile.y;
                clip(y);
                clip(1.0 - y);
                // Point-up hex matching Grid: shoulders at 1/4 and 3/4 height.
                float halfWidth = min(0.5, min(y, 1.0 - y) * 2.0);
                float across = (input.tile.x - 0.5) / max(halfWidth * 2.0, 0.00001) + 0.5;
                clip(across);
                clip(1.0 - across);

                float4 span = tex2D(_RowSpans, float2(0.5, y));
                fixed3 rgb = tex2D(_ArtworkTex, float2(lerp(span.r, span.g, across), span.b)).rgb;
                // The artwork is a filled terrain tile. Its old transparent margin
                // must not become a hole inside the canonical hex footprint.
                fixed alpha = input.color.a;
                return fixed4(rgb * input.color.rgb * alpha, alpha);
            }
            ENDCG
        }
    }
}
