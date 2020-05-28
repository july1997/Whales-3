using Unity.Entities;

[GenerateAuthoringComponent]
public struct ScoreCompoment : IComponentData
{
    public int value;
}