Shader "Hidden/Shader/Kaleidoscope"
{
    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }

    // List of properties to control your post process effect
    int _NumberOfMirrorings;
	float _Offset;
	float _Roll;
	float _Zoom;
    TEXTURE2D_X(_InputTexture);
	

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
		/*****
		This method was inspired by:
		Teflo: https://www.shadertoy.com/view/4sXBRl,
		Keijro: https://github.com/keijiro/KinoMirror/blob/master/Assets/Kino/Mirror/Shader/Mirror.shader
		*****/

        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float2 fragmentCoordinates = float2(input.texcoord.xy);
		fragmentCoordinates.x = fragmentCoordinates.x * 2 / 9 * 16 - (1.0 * 16 / 9);
		fragmentCoordinates.y = fragmentCoordinates.y * 2 - 1;

	   	float2 position = fragmentCoordinates;
		float2 offset = float2(_Offset, _Offset);

		float r = length(position);
		float angle = atan2(position.x, position.y);

		if (angle < 0) {
			angle = angle + 2 * 3.14159;
		}

		float slices = _NumberOfMirrorings;
		float slice = 6.28 / slices;  

		angle = fmod(angle, slice);
		angle = abs(angle - 0.5 * slice);
		angle += _Roll;

		position = float2(cos(angle), sin(angle)) * r;
		position = max(min(position, 2.0 - position), -position);

		return SAMPLE_TEXTURE2D_X(_InputTexture, s_linear_clamp_sampler, (position + _Offset) * _Zoom);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "Kaleidoscope"

            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM
                #pragma fragment CustomPostProcess
                #pragma vertex Vert
            ENDHLSL
        }
    }
    Fallback Off
}
