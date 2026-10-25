// RecoveredNavMeshSource.cs
// Editor-only record of what a recovered navmesh was built from, so it can be rebuilt with
// different settings without re-matching. Saved as Assets/RecoveredNavMesh/<scene>_Source.asset.
// Nothing references this at runtime, so it never ends up in a player build.
//
// C# 4.0 compatible.

using UnityEngine;

public class RecoveredNavMeshSource : ScriptableObject
{
    public Mesh[] sourceMeshes;
    public int[] sourceAreas;
    public string sourceAssetPath;
    public float originalAgentHeight;
    public float originalAgentRadius;
    public float originalAgentClimb;
}
