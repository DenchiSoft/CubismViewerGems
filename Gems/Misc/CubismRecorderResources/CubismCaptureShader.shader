// Modified version of CameraCapture.shader, originally written by Keijiro Takahashi for FFmpegOut:
// https://github.com/keijiro/FFmpegOut
// 
// Copyright by Keijiro Takahashi, licensed under the MIT license:
// https://github.com/keijiro/FFmpegOut/blob/master/LICENSE.md

Shader "FFmpegOutCubism/CubismCameraCapture"
{
    Properties
    {
        _MainTex("", 2D) = "white" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;

    fixed4 frag_flip(v2f_img i) : SV_Target
    {
        float2 uv = i.uv;
        uv.y = 1 - uv.y;
        return tex2D(_MainTex, uv);
    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_flip
            ENDCG
        }
    }
}
