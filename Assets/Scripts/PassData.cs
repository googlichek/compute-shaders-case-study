using UnityEngine;

public class PassData : MonoBehaviour
{
    [SerializeField]
    private ComputeShader _shader = default;

    [SerializeField]
    private int _texResolution = 1024;

    [SerializeField, Space]
    private Color _clearColor = Color.clear;

    [SerializeField]
    private Color _circleColor = Color.clear;

    private Renderer _renderer;

    private RenderTexture _outputTexture;

    private int _circlesHandle = 0;
    private int _clearHandle = 0;

    // Use this for initialization
    void Start()
    {
        _outputTexture = new RenderTexture(_texResolution, _texResolution, 0);
        _outputTexture.enableRandomWrite = true;
        _outputTexture.Create();

        _renderer = GetComponent<Renderer>();
        _renderer.enabled = true;

        InitShader();
    }

    void Update()
    {
        DispatchKernels(10);
    }

    private void InitShader()
    {
        _circlesHandle = _shader.FindKernel("Circles");
        _clearHandle = _shader.FindKernel("Clear");

        _shader.SetInt( "texResolution", _texResolution);

        _shader.SetTexture( _circlesHandle, "Result", _outputTexture);
        _shader.SetTexture( _clearHandle, "Result", _outputTexture);

        _shader.SetVector("clearColor", _clearColor);
        _shader.SetVector("circleColor", _circleColor);

        _renderer.material.SetTexture("_MainTex", _outputTexture);
    }

    private void DispatchKernels(int count)
    {
        _shader.Dispatch(_clearHandle, _texResolution / 8, _texResolution / 8, 1);
        _shader.SetFloat("time", Time.time);
        _shader.Dispatch(_circlesHandle, count, 1, 1);
    }
}

