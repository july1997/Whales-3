using Unity.Entities;
using Unity.NetCode;

[GenerateAuthoringComponent]
public struct PlayerCommandData : IComponentData
{
    [GhostDefaultField]
    public int PlayerId;
}

