using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealisticCloth : MonoBehaviour
{
    [Header("Cloth Settings")]
    [SerializeField] private int widthSegments = 20;
    [SerializeField] private int heightSegments = 20;
    [SerializeField] private float clothWidth = 2f;
    [SerializeField] private float clothHeight = 2f;
    
    [Header("Physics Settings")]
    [SerializeField] private float mass = 1f;
    [SerializeField] private float damping = 0.99f;
    [SerializeField] private float springStiffness = 1000f;
    [SerializeField] private float springDamping = 10f;
    [SerializeField] private float bendStiffness = 100f;
    
    [Header("Collision Settings")]
    [SerializeField] private LayerMask collisionLayers = -1;
    [SerializeField] private float collisionRadius = 0.05f;
    [SerializeField] private float collisionResponse = 0.8f;
    [SerializeField] private float friction = 0.5f;
    [SerializeField] private float maxCollisionDistance = 1f; // 충돌 검사 최대 거리
    
    [Header("Constraints")]
    [SerializeField] private bool[] pinnedVertices; // Inspector에서 고정할 정점 선택
    
    [Header("Stability Settings")]
    [SerializeField] private float maxVelocity = 50f; // 최대 속도 제한
    [SerializeField] private float maxPositionOffset = 100f; // 최대 위치 오프셋
    [SerializeField] private bool enableDebugGizmos = false; // Gizmos 비활성화로 성능 향상
    
    private Mesh clothMesh;
    private Vector3[] vertices;
    private Vector3[] velocities;
    private Vector3[] previousVertices;
    
    // Spring 구조체
    private struct Spring
    {
        public int vertexA;
        public int vertexB;
        public float restLength;
        public float stiffness;
        public float damping;
    }
    
    private List<Spring> springs = new List<Spring>();
    private List<Collider> colliders = new List<Collider>();
    
    void Start()
    {
        InitializeCloth();
        FindColliders();
    }
    
    void InitializeCloth()
    {
        // Mesh 생성
        clothMesh = new Mesh();
        clothMesh.name = "ClothMesh";
        
        int vertexCount = (widthSegments + 1) * (heightSegments + 1);
        vertices = new Vector3[vertexCount];
        velocities = new Vector3[vertexCount];
        previousVertices = new Vector3[vertexCount];
        
        // 정점 위치 초기화
        for (int y = 0; y <= heightSegments; y++)
        {
            for (int x = 0; x <= widthSegments; x++)
            {
                int index = y * (widthSegments + 1) + x;
                float u = (float)x / widthSegments;
                float v = (float)y / heightSegments;
                
                vertices[index] = new Vector3(
                    (u - 0.5f) * clothWidth,
                    0f,
                    (v - 0.5f) * clothHeight
                );
                previousVertices[index] = vertices[index];
                velocities[index] = Vector3.zero;
            }
        }
        
        // 삼각형 인덱스 생성
        List<int> triangles = new List<int>();
        for (int y = 0; y < heightSegments; y++)
        {
            for (int x = 0; x < widthSegments; x++)
            {
                int i = y * (widthSegments + 1) + x;
                
                triangles.Add(i);
                triangles.Add(i + widthSegments + 1);
                triangles.Add(i + 1);
                
                triangles.Add(i + 1);
                triangles.Add(i + widthSegments + 1);
                triangles.Add(i + widthSegments + 2);
            }
        }
        
        clothMesh.vertices = vertices;
        clothMesh.triangles = triangles.ToArray();
        clothMesh.RecalculateNormals();
        clothMesh.RecalculateBounds();
        
        // MeshFilter와 MeshRenderer 설정
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = clothMesh;
        
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        
        // Spring 생성
        CreateSprings();
        
        // Pinned vertices 배열 초기화
        if (pinnedVertices == null || pinnedVertices.Length != vertexCount)
        {
            pinnedVertices = new bool[vertexCount];
            // 상단 가장자리 고정
            for (int x = 0; x <= widthSegments; x++)
            {
                pinnedVertices[x] = true;
            }
        }
    }
    
    void CreateSprings()
    {
        springs.Clear();
        
        for (int y = 0; y <= heightSegments; y++)
        {
            for (int x = 0; x <= widthSegments; x++)
            {
                int index = y * (widthSegments + 1) + x;
                
                // 구조 스프링 (Structural Springs)
                if (x < widthSegments)
                {
                    AddSpring(index, index + 1, springStiffness, springDamping);
                }
                if (y < heightSegments)
                {
                    AddSpring(index, index + widthSegments + 1, springStiffness, springDamping);
                }
                
                // 대각선 스프링 (Shear Springs)
                if (x < widthSegments && y < heightSegments)
                {
                    AddSpring(index, index + widthSegments + 2, springStiffness * 0.5f, springDamping);
                    AddSpring(index + 1, index + widthSegments + 1, springStiffness * 0.5f, springDamping);
                }
                
                // 굽힘 스프링 (Bend Springs)
                if (x < widthSegments - 1)
                {
                    AddSpring(index, index + 2, bendStiffness, springDamping * 0.5f);
                }
                if (y < heightSegments - 1)
                {
                    AddSpring(index, index + (widthSegments + 1) * 2, bendStiffness, springDamping * 0.5f);
                }
            }
        }
    }
    
    void AddSpring(int a, int b, float stiffness, float damping)
    {
        Spring spring = new Spring
        {
            vertexA = a,
            vertexB = b,
            restLength = Vector3.Distance(vertices[a], vertices[b]),
            stiffness = stiffness,
            damping = damping
        };
        springs.Add(spring);
    }
    
    void FindColliders()
    {
        // 씬의 모든 콜라이더 찾기 (레이어 마스크 적용)
        Collider[] allColliders = FindObjectsOfType<Collider>();
        colliders.Clear();
        
        foreach (Collider col in allColliders)
        {
            // 자기 자신의 콜라이더는 제외
            if (col.gameObject == gameObject)
                continue;
            
            // 레이어 마스크 확인
            if (((1 << col.gameObject.layer) & collisionLayers) != 0)
            {
                colliders.Add(col);
            }
        }
    }
    
    void FixedUpdate()
    {
        if (vertices == null || vertices.Length == 0)
            return;
        
        float deltaTime = Mathf.Min(Time.fixedDeltaTime, 0.02f); // 최대 deltaTime 제한
        
        // 중력 적용
        Vector3 gravity = Physics.gravity;
        for (int i = 0; i < vertices.Length; i++)
        {
            if (pinnedVertices[i])
                continue;
            
            velocities[i] += gravity * deltaTime;
            
            // 속도 제한 (발산 방지)
            if (!IsFinite(velocities[i]) || velocities[i].magnitude > maxVelocity)
            {
                velocities[i] = velocities[i].normalized * maxVelocity;
            }
        }
        
        // Spring 힘 계산 및 적용
        ApplySpringForces(deltaTime);
        
        // Verlet 통합
        for (int i = 0; i < vertices.Length; i++)
        {
            if (pinnedVertices[i])
                continue;
            
            Vector3 temp = vertices[i];
            Vector3 velocity = (vertices[i] - previousVertices[i]) * damping + velocities[i] * deltaTime;
            
            // 값이 유효한지 확인
            if (!IsFinite(velocity) || !IsFinite(vertices[i]))
            {
                // 값이 무한대면 초기 위치로 리셋
                vertices[i] = previousVertices[i];
                velocities[i] = Vector3.zero;
                continue;
            }
            
            vertices[i] = vertices[i] + velocity;
            
            // 위치가 너무 멀리 가지 않도록 제한
            if (vertices[i].magnitude > maxPositionOffset)
            {
                vertices[i] = vertices[i].normalized * maxPositionOffset;
                velocities[i] = Vector3.zero;
            }
            
            previousVertices[i] = temp;
        }
        
        // 충돌 검사 및 응답
        HandleCollisions();
        
        // Mesh 업데이트
        UpdateMesh();
    }
    
    bool IsFinite(Vector3 v)
    {
        return float.IsFinite(v.x) && float.IsFinite(v.y) && float.IsFinite(v.z);
    }
    
    void ApplySpringForces(float deltaTime)
    {
        float invMass = 1f / mass;
        const float maxSpringForce = 10000f; // 최대 스프링 힘 제한
        
        foreach (Spring spring in springs)
        {
            Vector3 posA = vertices[spring.vertexA];
            Vector3 posB = vertices[spring.vertexB];
            
            // 값이 유효한지 확인
            if (!IsFinite(posA) || !IsFinite(posB))
                continue;
            
            Vector3 delta = posB - posA;
            float currentLength = delta.magnitude;
            
            if (currentLength < 0.0001f)
                continue;
            
            float extension = currentLength - spring.restLength;
            Vector3 direction = delta / currentLength;
            
            // Spring 힘 (제한 적용)
            Vector3 springForce = direction * extension * spring.stiffness;
            if (springForce.magnitude > maxSpringForce)
            {
                springForce = springForce.normalized * maxSpringForce;
            }
            
            // Damping 힘
            Vector3 relativeVelocity = velocities[spring.vertexB] - velocities[spring.vertexA];
            Vector3 dampingForce = direction * Vector3.Dot(relativeVelocity, direction) * spring.damping;
            
            Vector3 totalForce = springForce + dampingForce;
            
            // 힘이 유효한지 확인
            if (!IsFinite(totalForce))
                continue;
            
            if (!pinnedVertices[spring.vertexA])
            {
                velocities[spring.vertexA] += totalForce * invMass * deltaTime;
                // 속도 제한
                if (velocities[spring.vertexA].magnitude > maxVelocity)
                {
                    velocities[spring.vertexA] = velocities[spring.vertexA].normalized * maxVelocity;
                }
            }
            if (!pinnedVertices[spring.vertexB])
            {
                velocities[spring.vertexB] -= totalForce * invMass * deltaTime;
                // 속도 제한
                if (velocities[spring.vertexB].magnitude > maxVelocity)
                {
                    velocities[spring.vertexB] = velocities[spring.vertexB].normalized * maxVelocity;
                }
            }
        }
    }
    
    void HandleCollisions()
    {
        float checkDistance = collisionRadius + maxCollisionDistance;
        
        for (int i = 0; i < vertices.Length; i++)
        {
            if (pinnedVertices[i])
                continue;
            
            Vector3 worldPos = transform.TransformPoint(vertices[i]);
            
            // 값이 유효한지 확인
            if (!IsFinite(worldPos))
                continue;
            
            foreach (Collider col in colliders)
            {
                if (col == null || !col.enabled)
                    continue;
                
                // 거리 기반 culling - bounds로 먼저 체크
                float boundsDistance = Vector3.Distance(worldPos, col.bounds.ClosestPoint(worldPos));
                if (boundsDistance > checkDistance)
                    continue;
                
                // 가장 가까운 점 찾기 (모든 콜라이더 타입 지원)
                Vector3 closestPoint = Physics.ClosestPoint(worldPos, col, col.transform.position, col.transform.rotation);
                
                // 값이 유효한지 확인
                if (!IsFinite(closestPoint))
                    continue;
                
                Vector3 direction = worldPos - closestPoint;
                float distance = direction.magnitude;
                
                // 충돌 검사
                if (distance < collisionRadius)
                {
                    if (distance < 0.0001f)
                    {
                        // 콜라이더 내부에 있는 경우, 표면으로 밀어냄
                        direction = (worldPos - col.bounds.center);
                        if (direction.magnitude < 0.0001f)
                        {
                            direction = Vector3.up; // 기본 방향
                        }
                        direction = direction.normalized;
                        distance = collisionRadius;
                    }
                    
                    Vector3 normal = direction.normalized;
                    if (!IsFinite(normal))
                        continue;
                    
                    float penetration = collisionRadius - distance;
                    
                    // 위치 보정
                    Vector3 correction = normal * penetration * collisionResponse;
                    worldPos += correction;
                    
                    // 값이 유효한지 확인
                    if (IsFinite(worldPos))
                    {
                        vertices[i] = transform.InverseTransformPoint(worldPos);
                        
                        // 속도 반응
                        float velocityAlongNormal = Vector3.Dot(velocities[i], normal);
                        if (velocityAlongNormal < 0)
                        {
                            // 반사
                            Vector3 reflection = velocities[i] - 2f * velocityAlongNormal * normal;
                            velocities[i] = reflection * (1f - friction);
                            
                            // 속도 제한
                            if (velocities[i].magnitude > maxVelocity)
                            {
                                velocities[i] = velocities[i].normalized * maxVelocity;
                            }
                        }
                    }
                }
            }
        }
    }
    
    void UpdateMesh()
    {
        if (clothMesh == null || vertices == null)
            return;
        
        // 유효하지 않은 정점이 있는지 확인하고 수정
        for (int i = 0; i < vertices.Length; i++)
        {
            if (!IsFinite(vertices[i]))
            {
                // 무한대 값이면 이전 위치로 복원
                if (previousVertices != null && i < previousVertices.Length && IsFinite(previousVertices[i]))
                {
                    vertices[i] = previousVertices[i];
                }
                else
                {
                    vertices[i] = Vector3.zero;
                }
            }
        }
        
        clothMesh.vertices = vertices;
        clothMesh.RecalculateNormals();
        
        // Bounds가 너무 크면 제한
        Bounds bounds = clothMesh.bounds;
        if (bounds.size.magnitude > maxPositionOffset * 2f)
        {
            clothMesh.bounds = new Bounds(Vector3.zero, Vector3.one * maxPositionOffset * 2f);
        }
        else
        {
            clothMesh.RecalculateBounds();
        }
    }
    
    // 런타임에 콜라이더 추가/제거
    public void AddCollider(Collider col)
    {
        if (col != null && !colliders.Contains(col))
        {
            colliders.Add(col);
        }
    }
    
    public void RemoveCollider(Collider col)
    {
        colliders.Remove(col);
    }
    
    // 정점 고정/해제
    public void PinVertex(int index, bool pin)
    {
        if (index >= 0 && index < pinnedVertices.Length)
        {
            pinnedVertices[index] = pin;
        }
    }
    
    // 디버그용: 정점 위치 시각화 (성능을 위해 비활성화 가능)
    void OnDrawGizmos()
    {
        if (!enableDebugGizmos || vertices == null)
            return;
        
        // 성능을 위해 일부만 표시
        int step = Mathf.Max(1, vertices.Length / 100);
        
        for (int i = 0; i < vertices.Length; i += step)
        {
            if (pinnedVertices != null && i < pinnedVertices.Length && pinnedVertices[i])
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            
            Vector3 worldPos = transform.TransformPoint(vertices[i]);
            if (IsFinite(worldPos))
            {
                Gizmos.DrawSphere(worldPos, 0.02f);
            }
        }
    }
}
