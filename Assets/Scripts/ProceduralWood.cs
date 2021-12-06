using UnityEngine;

public class ProceduralWood : MonoBehaviour
{
    [SerializeField]
    private ComputeShader _shader = default;

    [SerializeField]
    private int texResolution = 1024;

    [SerializeField]
    private Renderer _renderer = default;

    [SerializeField]
    private Color paleColor = new Color(0.733f, 0.565f, 0.365f, 1);

    [SerializeField]
    private Color darkColor = new Color(0.49f, 0.286f, 0.043f, 1);

    [SerializeField]
    private float frequency = 2.0f;

    [SerializeField]
    private float noiseScale = 6.0f;

    [SerializeField]
    private float ringScale = 0.6f;

    [SerializeField]
    private float contrast = 4.0f;

    private RenderTexture outputTexture = default;

    private int kernelHandle = 0;

    // Use this for initialization
    void Start()
    {
        outputTexture = new RenderTexture(texResolution, texResolution, 0);
        outputTexture.enableRandomWrite = true;
        outputTexture.Create();

        _renderer.enabled = true;

        InitShader();
    }

    private void InitShader()
    {
        kernelHandle = _shader.FindKernel("CSMain");

        _shader.SetInt("texResolution", texResolution);

        _shader.SetVector("paleColor", paleColor);
        _shader.SetVector("darkColor", darkColor);
        _shader.SetFloat("frequency", frequency);
        _shader.SetFloat("noiseScale", noiseScale);
        _shader.SetFloat("ringScale", ringScale);
        _shader.SetFloat("contrast", contrast);

        _shader.SetTexture(kernelHandle, "Result", outputTexture);

        _renderer.material.SetTexture("_MainTex", outputTexture);

        DispatchShader(texResolution / 8, texResolution / 8);
    }

    private void DispatchShader(int x, int y)
    {
        _shader.Dispatch(kernelHandle, x, y, 1);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.U))
        {
            DispatchShader(texResolution / 8, texResolution / 8);
        }
    }
}
