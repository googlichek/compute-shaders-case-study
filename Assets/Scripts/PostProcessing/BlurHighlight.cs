using UnityEngine;

[ExecuteInEditMode]
public class BlurHighlight : BasePostProcessing
{
    [SerializeField] [Range(0, 50)]
    protected int blurRadius = 20;

    [SerializeField] [Range(0.0f, 100.0f)]
    protected float radius = 10;

    [SerializeField] [Range(0.0f, 100.0f)]
    protected float softenEdge = 30;

    [SerializeField] [Range(0.0f, 1.0f)]
    protected float shade = 0.5f;

    [SerializeField]
    protected Transform trackedObject;

    protected Vector4 center = Vector4.zero;

    protected RenderTexture horzOutput = default;

    protected int kernelHorzPassID;

    protected void OnValidate()
    {
        if(!isInitialized)
            Init();

        SetProperties();
    }

    protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (shader == null)
        {
            Graphics.Blit(source, destination);
        }
        else
        {
            if (trackedObject && thisCamera)
            {
                Vector3 pos = thisCamera.WorldToScreenPoint(trackedObject.position);
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

    protected override void Init()
    {
        center = new Vector4();
        kernelName = "Highlight";
        base.Init();
        kernelHorzPassID = shader.FindKernel("HorzPass");
    }

    protected override void CreateTextures()
    {
        base.CreateTextures();

        shader.SetTexture(kernelHorzPassID, "source", renderedSource);

        CreateTexture(ref horzOutput);

        shader.SetTexture(kernelHorzPassID, "horzOutput", horzOutput);
        shader.SetTexture(kernelHandle, "horzOutput", horzOutput);
    }

    protected override void DispatchWithSource(ref RenderTexture source, ref RenderTexture destination)
    {
        if (!isInitialized)
            return;

        Graphics.Blit(source, renderedSource);

        shader.Dispatch(kernelHorzPassID, groupSize.x, groupSize.y, 1);
        shader.Dispatch(kernelHandle, groupSize.x, groupSize.y, 1);

        Graphics.Blit(output, destination);
    }

    protected void SetProperties()
    {
        var rad = (radius / 100.0f) * texSize.y;
        shader.SetFloat("radius", rad);
        shader.SetFloat("edgeWidth", rad * softenEdge / 100.0f);
        shader.SetFloat("shade", shade);
        shader.SetInt("blurRadius", blurRadius);
    }
}
