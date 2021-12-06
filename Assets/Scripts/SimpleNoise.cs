using UnityEngine;

public class SimpleNoise : MonoBehaviour
{
    [SerializeField]
    private ComputeShader _shader = default;

    [SerializeField]
    private int _texResolution = 256;

    [SerializeField]
    private Renderer _renderer = default;

    private RenderTexture _outputTexture;

    private int _kernelHandle;

    // Use this for initialization
    void Start()
    {
        _outputTexture = new RenderTexture(_texResolution, _texResolution, 0);
        _outputTexture.enableRandomWrite = true;
        _outputTexture.Create();

        _renderer.enabled = true;

        InitShader();
    }

    void Update()
    {
        DispatchShader(_texResolution / 8, _texResolution / 8);
    }

    private void InitShader()
    {
        _kernelHandle = _shader.FindKernel("SimpleNoise");

        _shader.SetInt("texResolution", _texResolution);
        _shader.SetTexture(_kernelHandle, "Result", _outputTexture);

        _renderer.material.SetTexture("_MainTex", _outputTexture);
    }

    private void DispatchShader(int x, int y)
    {
        _shader.SetFloat("time", Time.time);
        _shader.Dispatch(_kernelHandle, x, y, 1);
    }
}
