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
    public ClampedIntParameter zoom = new ClampedIntParameter(1, 1, 5);
    [Tooltip("E.g. if you haven an aspect ration of 16:9, set the aspectWidth to 16.")]
    public IntParameter aspectWidth = new IntParameter(0);
    public IntParameter aspectHeight = new IntParameter(0);

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
        if (aspectWidth == null || aspectHeight == null || aspectWidth == 0 || aspectHeight == 0)
        {
            Debug.LogError("aspectWidth or aspectHeight in an Object of type " + this.GetType().ToString() + " were either null or 0. Setting them to 16 and 9.");
            aspectHeight.value = 9;
            aspectWidth.value = 16; 
        }

        m_Material.SetInt("_NumberOfMirrorings", numberOfMirrorings.value);
        m_Material.SetFloat("_Offset", offset.value * Mathf.Deg2Rad);
        m_Material.SetInt("_Zoom", zoom.value);
        m_Material.SetFloat("_Roll", roll.value * Mathf.Deg2Rad);
        m_Material.SetInt("_AspectWidth", aspectWidth.value);
        m_Material.SetInt("_AspectHeight", aspectHeight.value);
        m_Material.SetTexture("_InputTexture", source);
        HDUtils.DrawFullScreen(cmd, m_Material, destination);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
