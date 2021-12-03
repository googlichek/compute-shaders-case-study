using UnityEngine;

public class OrbitingStars : MonoBehaviour
{
    [SerializeField]
    private int _starCount = 17;

    [SerializeField]
    private ComputeShader _shader = default;

    [SerializeField]
    private GameObject _prefab = default;

    private int _kernelHandle = 0;
    private uint _threadGroupSizeX = 0;
    private int _groupSizeX = 0;
    
    private Transform[] _stars = default;
    private ComputeBuffer _resultBuffer = default;
    private Vector3[] _output = default;

    void Start()
    {
        _kernelHandle = _shader.FindKernel("OrbitingStars");
        _shader.GetKernelThreadGroupSizes(_kernelHandle, out _threadGroupSizeX, out _, out _);
        _groupSizeX = (int)((_starCount + _threadGroupSizeX - 1) / _threadGroupSizeX);

        _resultBuffer = new ComputeBuffer(_starCount, sizeof(float) * 3);
        _shader.SetBuffer(_kernelHandle, "Result", _resultBuffer);
        _output = new Vector3[_starCount];

        _stars = new Transform[_starCount];
        for (var i = 0; i < _starCount; i++)
        {
            _stars[i] = Instantiate(_prefab, transform).transform;
        }
    }

    void Update()
    {
        _shader.SetFloat("time", Time.time);
        _shader.Dispatch(_kernelHandle, _groupSizeX, 1, 1);

        _resultBuffer.GetData(_output);
        for (int i = 0; i < _starCount; i++)
        {
            _stars[i].localPosition = _output[i];
        }
    }

    void OnDestroy()
    {
        _resultBuffer.Dispose();
    }
}
