using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Custom/KaleidosCope")]
public sealed class Kaleidoscope : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the number of mirrorings.")]
    public ClampedIntParameter numberOfMirrorings = new ClampedIntParameter(4, 0, 20);
    public ClampedFloatParameter offset = new ClampedFloatParameter(0, 0, 20f);
    public ClampedFloatParameter roll = new ClampedFloatParameter(0, 0, 360);
    public ClampedFloatParameter zoom = new ClampedFloatParameter(0.8f, 0.2f, 11f);

    Material m_Material;
    
    public bool IsActive() => m_Material != null && numberOfMirrorings.value > 0;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/Kaleidoscope") != null)
            m_Material = new Material(Shader.Find("Hidden/Shader/Kaleidoscope"));
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;

        m_Material.SetInt("_NumberOfMirrorings", numberOfMirrorings.value);
        m_Material.SetFloat("_Offset", offset.value * Mathf.Deg2Rad);
        m_Material.SetFloat("_Zoom", zoom.value);
        m_Material.SetFloat("_Roll", roll.value * Mathf.Deg2Rad);
        m_Material.SetTexture("_InputTexture", source);
        HDUtils.DrawFullScreen(cmd, m_Material, destination);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
