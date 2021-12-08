using UnityEngine;

[ExecuteInEditMode]
public class RingHighlight : BasePostProcessing
{
    [SerializeField] [Range(0.0f, 100.0f)]
    protected float radius = 10;

    [SerializeField] [Range(0.0f, 100.0f)]
    protected float softenEdge;

    [SerializeField] [Range(0.0f, 1.0f)]
    protected float shade;

    [SerializeField]
    protected Transform trackedObject;

    protected Vector4 center = Vector4.zero;

    protected override void Init()
    {
        kernelName = "Highlight";
        base.Init();
    }

    private void OnValidate()
    {
        if(!isInitialized)
            Init();

        SetProperties();
    }

    protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!isInitialized || shader == null)
        {
            Graphics.Blit(source, destination);
        }
        else
        {
            if (trackedObject && thisCamera)
            {
                var pos = thisCamera.WorldToScreenPoint(trackedObject.position);
                center.x = pos.x;
                center.y = pos.y;
                shader.SetVector("center", center);
            }

            var resChange = false;
            CheckResolution(out resChange);

            if (resChange)
                SetProperties();

            DispatchWithSource(ref source, ref destination);
        }
    }

    protected void SetProperties()
    {
        float rad = (radius / 100.0f) * texSize.y;
        shader.SetFloat("radius", rad);
        shader.SetFloat("edgeWidth", rad * softenEdge / 100.0f);
        shader.SetFloat("shade", shade);
    }
}
