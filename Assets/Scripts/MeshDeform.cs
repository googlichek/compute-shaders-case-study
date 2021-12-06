using UnityEngine;

public class MeshDeform : MonoBehaviour
{
    private struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;

        public Vertex(Vector3 position, Vector3 normal)
        {
            Position.x = position.x;
            Position.y = position.y;
            Position.z = position.z;

            Normal.x = normal.x;
            Normal.y = normal.y;
            Normal.z = normal.z;
        }
    }

    [SerializeField]
    private ComputeShader _shader;

    [SerializeField] [Range(0.5f, 2.0f)]
    private float _radius;

    [SerializeField]
    private MeshFilter _meshFilter = default;

    private Vertex[] _vertexArray;
    private Vertex[] _initialArray;

    private ComputeBuffer _vertexBuffer;
    private ComputeBuffer _initialBuffer;

    private int _kernelHandle;
    private Mesh _mesh;

    // Use this for initialization
    void Start()
    {
        if (InitData())
        {
            InitShader();
        }
    }

    void Update()
    {
        if (_shader)
        {
            _shader.SetFloat("radius", _radius);
            var delta = (Mathf.Sin(Time.time) + 1) / 2;

            _shader.SetFloat("delta", delta);
            _shader.Dispatch(_kernelHandle, _vertexArray.Length, 1, 1);

            GetVerticesFromGPU();
        }
    }

    void OnDestroy()
    {
        _vertexBuffer.Dispose();
        _initialBuffer.Dispose();
    }

    private bool InitData()
    {
        _kernelHandle = _shader.FindKernel("CSMain");

        if (_meshFilter == null)
        {
            Debug.Log("No MeshFilter found");
            return false;
        }

        InitVertexArrays(_meshFilter.mesh);
        InitGPUBuffers();

        _mesh = _meshFilter.mesh;

        return true;
    }

    private void InitShader()
    {
        _shader.SetFloat("radius", _radius);
    }

    private void InitVertexArrays(Mesh mesh)
    {
        _vertexArray = new Vertex[mesh.vertices.Length];
        _initialArray = new Vertex[mesh.vertices.Length];

        for (var i = 0; i < _vertexArray.Length; i++)
        {
            var v1 = new Vertex(mesh.vertices[i], mesh.normals[i]);
            _vertexArray[i] = v1;

            var v2 = new Vertex(mesh.vertices[i], mesh.normals[i]);
            _initialArray[i] = v2;
        }
    }

    private void InitGPUBuffers()
    {
        _vertexBuffer = new ComputeBuffer(_vertexArray.Length, sizeof(float) * 6);
        _vertexBuffer.SetData(_vertexArray);

        _initialBuffer = new ComputeBuffer(_initialArray.Length, sizeof(float) * 6);
        _initialBuffer.SetData(_initialArray);

        _shader.SetBuffer(_kernelHandle, "vertexBuffer", _vertexBuffer);
        _shader.SetBuffer(_kernelHandle, "initialBuffer", _initialBuffer);
    }

    void GetVerticesFromGPU()
    {
        _vertexBuffer.GetData(_vertexArray);

        var vertices = new Vector3[_vertexArray.Length];
        var normals  = new Vector3[_vertexArray.Length];

        for (var i = 0; i < _vertexArray.Length; i++)
        {
            vertices[i] =
                new Vector3(_vertexArray[i].Position.x, _vertexArray[i].Position.y, _vertexArray[i].Position.z);
            normals[i] =
                new Vector3(_vertexArray[i].Normal.x, _vertexArray[i].Normal.y, _vertexArray[i].Normal.z);
        }

        _mesh.vertices = vertices;
        _mesh.normals = normals;
    }
}
