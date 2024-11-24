using UnityEngine;
using System.Collections.Generic;

public class FogOfWar : MonoBehaviour
{
    [SerializeField] private Material fogMaterial;
    [SerializeField] private Transform player;
    [SerializeField] private float viewRadius = 5f;
    [SerializeField] private float raycastExtension = 2f; // How far beyond view radius to cast
    [SerializeField] private float wallPeekRadius = 1f; // How far beyond walls to reveal
    [SerializeField] private float fogSmoothing = 1f;
    [SerializeField] private int rayCount = 720;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float shadowSoftness = 2f;
    [SerializeField] private int samplesPerRay = 4;

    private static readonly int PlayerPos = Shader.PropertyToID("_PlayerPos");
    private static readonly int ViewRadius = Shader.PropertyToID("_ViewRadius");
    private static readonly int WallPeekRadius = Shader.PropertyToID("_WallPeekRadius");
    private static readonly int FogSmoothing = Shader.PropertyToID("_FogSmoothing");
    private static readonly int ShadowPoints = Shader.PropertyToID("_ShadowPoints");
    private static readonly int ShadowCount = Shader.PropertyToID("_ShadowCount");
    private static readonly int ShadowSoftness = Shader.PropertyToID("_ShadowSoftness");

    private Vector4[] shadowPoints = new Vector4[2880];
    private List<Vector4> dynamicShadowPoints = new List<Vector4>();

    private void Update()
    {
        Vector3 playerWorldPos = player.position;
        Vector2 playerPos = new Vector2(playerWorldPos.x, playerWorldPos.y);

        int actualShadowCount = CastViewRays(playerPos);

        fogMaterial.SetVector(PlayerPos, playerPos);
        fogMaterial.SetFloat(ViewRadius, viewRadius);
        fogMaterial.SetFloat(WallPeekRadius, wallPeekRadius);
        fogMaterial.SetFloat(FogSmoothing, fogSmoothing);
        fogMaterial.SetVectorArray(ShadowPoints, shadowPoints);
        fogMaterial.SetInt(ShadowCount, actualShadowCount);
        fogMaterial.SetFloat(ShadowSoftness, shadowSoftness);
    }

    private int CastViewRays(Vector2 origin)
    {
        dynamicShadowPoints.Clear();
        float angleStep = 360f / rayCount;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * angleStep;

            for (int j = 0; j < samplesPerRay; j++)
            {
                float subAngle = angle + (angleStep * j / samplesPerRay);
                Vector2 direction = Quaternion.Euler(0, 0, subAngle) * Vector2.right;

                // Cast ray beyond view radius
                RaycastHit2D hit = Physics2D.Raycast(origin, direction, viewRadius + raycastExtension, obstacleLayer);
                if (hit.collider != null)
                {
                    // Store additional data for wall peeking
                    Vector4 shadowPoint = new Vector4(
                        hit.point.x,
                        hit.point.y,
                        hit.normal.x,
                        hit.normal.y
                    );

                    dynamicShadowPoints.Add(shadowPoint);

                    // Add interpolated points for smoother transitions
                    if (dynamicShadowPoints.Count > 1)
                    {
                        Vector4 prevPoint = dynamicShadowPoints[dynamicShadowPoints.Count - 2];
                        Vector2 interpolated = Vector2.Lerp(
                            new Vector2(prevPoint.x, prevPoint.y),
                            new Vector2(shadowPoint.x, shadowPoint.y),
                            0.5f
                        );
                        Vector2 interpolatedNormal = Vector2.Lerp(
                            new Vector2(prevPoint.z, prevPoint.w),
                            new Vector2(shadowPoint.z, shadowPoint.w),
                            0.5f
                        ).normalized;

                        dynamicShadowPoints.Add(new Vector4(
                            interpolated.x,
                            interpolated.y,
                            interpolatedNormal.x,
                            interpolatedNormal.y
                        ));
                    }
                }
            }
        }

        dynamicShadowPoints.Sort((a, b) =>
        {
            float angleA = Mathf.Atan2(a.y - origin.y, a.x - origin.x);
            float angleB = Mathf.Atan2(b.y - origin.y, b.x - origin.x);
            return angleA.CompareTo(angleB);
        });

        int count = Mathf.Min(dynamicShadowPoints.Count, shadowPoints.Length);
        for (int i = 0; i < count; i++)
        {
            shadowPoints[i] = dynamicShadowPoints[i];
        }

        return count;
    }
}