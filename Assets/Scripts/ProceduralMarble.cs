using UnityEngine;
using System.Collections;

public class ProceduralMarble : MonoBehaviour
{
    [SerializeField]
    private ComputeShader _shader = default;

    [SerializeField]
    private int texResolution = 1024;

    [SerializeField]
    private Renderer _renderer = default;

    private RenderTexture _outputTexture = default;

    private int _kernelHandle = 0;
    private bool _marble = true;

    // Use this for initialization
    void Start()
    {
        _outputTexture = new RenderTexture(texResolution, texResolution, 0);
        _outputTexture.enableRandomWrite = true;
        _outputTexture.Create();

        _renderer.enabled = true;

        InitShader();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.U))
        {
            _shader.SetBool("marble", _marble);
            _marble = !_marble;
            DispatchShader(texResolution / 8, texResolution / 8);
        }
    }

    private void InitShader()
    {
        _kernelHandle = _shader.FindKernel("CSMain");

        _shader.SetInt("texResolution", texResolution);
        _shader.SetTexture(_kernelHandle, "Result", _outputTexture);

        _renderer.material.SetTexture("_MainTex", _outputTexture);

        _shader.SetBool("marble", _marble);
        _marble = !_marble;

        DispatchShader(texResolution / 8, texResolution / 8);
    }

    private void DispatchShader(int x, int y)
    {
        _shader.Dispatch(_kernelHandle, x, y, 1);
    }
}

