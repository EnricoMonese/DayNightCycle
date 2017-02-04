Shader "Custom/GradientVertexShader" {
Properties
{}
    SubShader
    {     
        Pass {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct vertexInput
                {
                    float4 vertex : POSITION;
                    fixed4 color : COLOR;
                };


               
                struct v2f {
                    float4 uv : SV_POSITION;
                    fixed4 color : COLOR;
                };
           
           
                v2f vert(vertexInput v)
                {
                    v2f o;
                    o.uv = UnityObjectToClipPos(v.vertex);
                    o.color = v.color;
                    return o;
                }
                fixed4 frag(v2f IN) : COLOR
                {
                    return IN.color;
                }
            ENDCG
        }
    }
}
