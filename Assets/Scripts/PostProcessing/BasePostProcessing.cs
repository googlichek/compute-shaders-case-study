using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BasePostProcessing : MonoBehaviour
{
    [SerializeField]
    protected ComputeShader shader = default;

    [SerializeField]
    protected Camera thisCamera = default;

    protected Vector2Int texSize = new Vector2Int(0, 0);
    protected Vector2Int groupSize = new Vector2Int();

    protected RenderTexture output = default;
    protected RenderTexture renderedSource = default;

    protected string kernelName = "CSMain";

    protected int kernelHandle = -1;
    protected bool isInitialized = false;

    protected virtual void OnEnable()
    {
        Init();
    }

    protected virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!isInitialized || shader == null)
        {
            Graphics.Blit(source, destination);
        }
        else
        {
            CheckResolution(out _);
            DispatchWithSource(ref source, ref destination);
        }
    }

    protected virtual void OnDisable()
    {
        ClearTextures();
        isInitialized = false;
    }

    protected virtual void OnDestroy()
    {
        ClearTextures();
        isInitialized = false;
    }

    protected virtual void Init()
    {
        if (!SystemInfo.supportsComputeShaders)
        {
            Debug.LogError("It seems your target Hardware does not support Compute Shaders.");
            return;
        }

        if (!shader)
        {
            Debug.LogError("No shader");
            return;
        }

        kernelHandle = shader.FindKernel(kernelName);

        if (!thisCamera)
        {
            Debug.LogError("Object has no Camera");
            return;
        }

        CreateTextures();

        isInitialized = true;
    }

    protected virtual void ClearTextures()
    {
        ClearTexture(ref output);
        ClearTexture(ref renderedSource);
    }

    protected virtual void CreateTextures()
    {
        texSize.x = thisCamera.pixelWidth;
        texSize.y = thisCamera.pixelHeight;

        if (shader)
        {
            shader.GetKernelThreadGroupSizes(kernelHandle, out var x, out var y, out _);
            groupSize.x = Mathf.CeilToInt((float)texSize.x / x);
            groupSize.y = Mathf.CeilToInt((float)texSize.y / y);
        }

        CreateTexture(ref output);
        CreateTexture(ref renderedSource);

        shader.SetTexture(kernelHandle, "source", renderedSource);
        shader.SetTexture(kernelHandle, "output", output);
    }

    protected virtual void DispatchWithSource(ref RenderTexture source, ref RenderTexture destination)
    {
        Graphics.Blit(source, renderedSource);

        shader.Dispatch(kernelHandle, groupSize.x, groupSize.y, 1);

        Graphics.Blit(output, destination);
    }

    protected void CreateTexture(ref RenderTexture textureToMake, int divide = 1)
    {
        textureToMake = new RenderTexture(texSize.x / divide, texSize.y / divide, 0);
        textureToMake.enableRandomWrite = true;
        textureToMake.Create();
    }

    protected void ClearTexture(ref RenderTexture textureToClear)
    {
        if (null != textureToClear)
        {
            textureToClear.Release();
            textureToClear = null;
        }
    }

    protected void CheckResolution(out bool resChange)
    {
        resChange = false;

        if (texSize.x != thisCamera.pixelWidth ||
            texSize.y != thisCamera.pixelHeight)
        {
            resChange = true;

            CreateTextures();
        }
    }
}
