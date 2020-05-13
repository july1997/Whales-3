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
        };
        return arr;
    }

    public int Length => 1;
#endif
    public static int FindGhostType<T>()
        where T : struct, ISnapshotData<T>
    {
        if (typeof(T) == typeof(WhaleSnapshotData))
            return 0;
        return -1;
    }

    public void BeginSerialize(ComponentSystemBase system)
    {
        m_WhaleGhostSerializer.BeginSerialize(system);
    }

    public int CalculateImportance(int serializer, ArchetypeChunk chunk)
    {
        switch (serializer)
        {
            case 0:
                return m_WhaleGhostSerializer.CalculateImportance(chunk);
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int GetSnapshotSize(int serializer)
    {
        switch (serializer)
        {
            case 0:
                return m_WhaleGhostSerializer.SnapshotSize;
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
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    private WhaleGhostSerializer m_WhaleGhostSerializer;
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
