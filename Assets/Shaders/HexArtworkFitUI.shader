Shader "Colonization/Hex Artwork Fit UI"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _ArtworkTex ("Original Artwork", 2D) = "white" {}
        _RowSpans ("Illustration Row Bounds", 2D) = "white" {}
        _Canvas ("Canvas Width Height PPU Ground", Vector) = (256,384,100,256)
        _UVToCanvasX ("UV To Canvas X", Vector) = (1,0,0,0)
        _UVToCanvasY ("UV To Canvas Y", Vector) = (0,1,0,0)
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "CanUseSpriteAtlas"="True" }
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            sampler2D _ArtworkTex;
            sampler2D _RowSpans;
            float4 _Canvas, _UVToCanvasX, _UVToCanvasY, _ClipRect;
            struct Input { float4 vertex : POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };
            struct Output { float4 vertex : SV_POSITION; float2 tile : TEXCOORD0; fixed4 color : COLOR; float2 local : TEXCOORD1; };
            Output vert(Input input)
            {
                Output output;
                output.vertex = UnityObjectToClipPos(input.vertex);
                output.local = input.vertex.xy;
                float3 uv = float3(input.uv, 1);
                output.tile = float2(dot(uv, _UVToCanvasX.xyz), dot(uv, _UVToCanvasY.xyz)) * float2(1, _Canvas.y / _Canvas.w);
                output.color = input.color;
                return output;
            }
            fixed4 frag(Output input) : SV_Target
            {
                float y = input.tile.y;
                clip(y); clip(1 - y);
                float halfWidth = min(0.5, min(y, 1 - y) * 2);
                float across = (input.tile.x - 0.5) / max(halfWidth * 2, 0.00001) + 0.5;
                clip(across); clip(1 - across);
                float4 span = tex2D(_RowSpans, float2(0.5, y));
                fixed3 rgb = tex2D(_ArtworkTex, float2(lerp(span.r, span.g, across), span.b)).rgb;
                fixed alpha = input.color.a;
                #ifdef UNITY_UI_CLIP_RECT
                alpha *= UnityGet2DClipping(input.local, _ClipRect);
                #endif
                return fixed4(rgb * input.color.rgb * alpha, alpha);
            }
            ENDCG
        }
    }
}
