using UnityEngine;
using Color = UnityEngine.Color;

public class BufferJoy : MonoBehaviour
{
    private struct Circle
    {
        public Vector2 Origin;
        public Vector2 Velocity;

        public float Radius;
    }

    [SerializeField]
    private ComputeShader _shader = default;

    [SerializeField]
    private int _texResolution = 1024;

    [SerializeField, Space]
    private Color _clearColor = Color.clear;

    [SerializeField]
    private Color _circleColor = Color.clear;

    private Renderer _renderer = default;

    private RenderTexture _outputTexture = default;

    private int count = 10;

    private int _circlesHandle = 0;
    private int _clearHandle = 0;

    private Circle[] _circleData;
    private ComputeBuffer _buffer;

    // Use this for initialization
    void Start()
    {
        _outputTexture = new RenderTexture(_texResolution, _texResolution, 0);
        _outputTexture.enableRandomWrite = true;
        _outputTexture.Create();

        _renderer = GetComponent<Renderer>();
        _renderer.enabled = true;

        InitData();
        InitShader();
    }

    void Update()
    {
        DispatchKernels(count);
    }

    private void InitData()
    {
        _circlesHandle = _shader.FindKernel("Circles");

        _shader.GetKernelThreadGroupSizes(_circlesHandle, out var threadGroupSizeX, out _, out _);

        var total = (int) threadGroupSizeX * count;
        _circleData = new Circle[total];
        var speed = 100f;
        var halfSpeed = speed * 0.5f;
        var minRadius = 10f;
        var maxRadius = 30f;
        var radiusRange = maxRadius - minRadius;

        for (var i = 0; i < total; i++)
        {
            var circle = _circleData[i];
            circle.Origin.x = Random.value * _texResolution;
            circle.Origin.y = Random.value * _texResolution;
            circle.Velocity.x = (Random.value * speed) - halfSpeed;
            circle.Velocity.y = (Random.value * speed) - halfSpeed;
            circle.Radius = Random.value * radiusRange + minRadius;
            _circleData[i] = circle;
        }
    }

    private void InitShader()
    {
        _clearHandle = _shader.FindKernel("Clear");

        _shader.SetInt( "texResolution", _texResolution);

        _shader.SetTexture( _circlesHandle, "Result", _outputTexture);
        _shader.SetTexture( _clearHandle, "Result", _outputTexture);

        _shader.SetVector("clearColor", _clearColor);
        _shader.SetVector("circleColor", _circleColor);

        var stride = (2 + 2 + 1) * sizeof(float);
        _buffer = new ComputeBuffer(_circleData.Length, stride);
        _buffer.SetData(_circleData);
        _shader.SetBuffer(_circlesHandle, "circlesBuffer", _buffer);

        _renderer.material.SetTexture("_MainTex", _outputTexture);
    }

    private void DispatchKernels(int count)
    {
        _shader.Dispatch(_clearHandle, _texResolution / 8, _texResolution / 8, 1);
        _shader.SetFloat("time", Time.time);
        _shader.Dispatch(_circlesHandle, count, 1, 1);
    }
}
