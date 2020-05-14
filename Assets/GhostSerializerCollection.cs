using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.NetCode;

public struct WhaleGhostSerializerCollection : IGhostSerializerCollection
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public string[] CreateSerializerNameList()
    {
        var arr = new string[]
        {
            "WhaleGhostSerializer",
            "BoidGhostSerializer",
        };
        return arr;
    }

    public int Length => 2;
#endif
    public static int FindGhostType<T>()
        where T : struct, ISnapshotData<T>
    {
        if (typeof(T) == typeof(WhaleSnapshotData))
            return 0;
        if (typeof(T) == typeof(BoidSnapshotData))
            return 1;
        return -1;
    }

    public void BeginSerialize(ComponentSystemBase system)
    {
        m_WhaleGhostSerializer.BeginSerialize(system);
        m_BoidGhostSerializer.BeginSerialize(system);
    }

    public int CalculateImportance(int serializer, ArchetypeChunk chunk)
    {
        switch (serializer)
        {
            case 0:
                return m_WhaleGhostSerializer.CalculateImportance(chunk);
            case 1:
                return m_BoidGhostSerializer.CalculateImportance(chunk);
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int GetSnapshotSize(int serializer)
    {
        switch (serializer)
        {
            case 0:
                return m_WhaleGhostSerializer.SnapshotSize;
            case 1:
                return m_BoidGhostSerializer.SnapshotSize;
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int Serialize(ref DataStreamWriter dataStream, SerializeData data)
    {
        switch (data.ghostType)
        {
            case 0:
            {
                return GhostSendSystem<WhaleGhostSerializerCollection>.InvokeSerialize<WhaleGhostSerializer, WhaleSnapshotData>(m_WhaleGhostSerializer, ref dataStream, data);
            }
            case 1:
            {
                return GhostSendSystem<WhaleGhostSerializerCollection>.InvokeSerialize<BoidGhostSerializer, BoidSnapshotData>(m_BoidGhostSerializer, ref dataStream, data);
            }
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    private WhaleGhostSerializer m_WhaleGhostSerializer;
    private BoidGhostSerializer m_BoidGhostSerializer;
}

public struct EnableWhaleGhostSendSystemComponent : IComponentData
{}
public class WhaleGhostSendSystem : GhostSendSystem<WhaleGhostSerializerCollection>
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnableWhaleGhostSendSystemComponent>();
    }
}
