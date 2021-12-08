using UnityEngine;

[ExecuteInEditMode]
public class NightVision : BasePostProcessing
{
    [SerializeField] [Range(0.0f, 100.0f)]
    protected float radius = 70;

    [SerializeField] [Range(0.0f, 1.0f)]
    protected float tintStrength = 0.7f;

    [SerializeField] [Range(0.0f, 100.0f)]
    protected float softenEdge = 3;

    [SerializeField]
    protected Color tint = Color.green;

    [SerializeField] [Range(50, 500)]
    protected int lines = 100;

    protected void OnValidate()
    {
        if(!isInitialized)
            Init();
           
        SetProperties();
    }

    protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        shader.SetFloat("time", Time.time);
        base.OnRenderImage(source, destination);
    }

    protected void SetProperties()
    {
        var rad = (radius / 100.0f) * texSize.y;
        shader.SetFloat("radius", rad);
        shader.SetFloat("edgeWidth", rad * softenEdge / 100.0f);
        shader.SetVector("tintColor", tint);
        shader.SetFloat("tintStrength", tintStrength);
        shader.SetInt("lines", lines);
    }
}
