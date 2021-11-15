using UnityEngine;

public class AssignTexture : MonoBehaviour
{
    [SerializeField]
    private ComputeShader _shader = default;

    [SerializeField]
    private int _texResolution = 256;

    [SerializeField]
    private Renderer _renderer = default;

    private RenderTexture _outputTexture = default;

    private int _kernelHandle = 0;

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
        if (Input.GetKeyDown(KeyCode.U))
        {
            DispatchShader(_texResolution / 8, _texResolution / 8);
        }
    }

    private void InitShader()
    {
        _kernelHandle = _shader.FindKernel("CSMain");
        _shader.SetTexture(_kernelHandle, "Result", _outputTexture);
        _renderer.material.SetTexture("_MainTex", _outputTexture);

        DispatchShader(_texResolution / 16, _texResolution / 16);
    }

    private void DispatchShader(int x, int y)
    {
        _shader.Dispatch(_kernelHandle, x, y, 1);
    }
}
