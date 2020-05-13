using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.NetCode;

public struct WhaleGhostDeserializerCollection : IGhostDeserializerCollection
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
    public void Initialize(World world)
    {
        var curWhaleGhostSpawnSystem = world.GetOrCreateSystem<WhaleGhostSpawnSystem>();
        m_WhaleSnapshotDataNewGhostIds = curWhaleGhostSpawnSystem.NewGhostIds;
        m_WhaleSnapshotDataNewGhosts = curWhaleGhostSpawnSystem.NewGhosts;
        curWhaleGhostSpawnSystem.GhostType = 0;
    }

    public void BeginDeserialize(JobComponentSystem system)
    {
        m_WhaleSnapshotDataFromEntity = system.GetBufferFromEntity<WhaleSnapshotData>();
    }
    public bool Deserialize(int serializer, Entity entity, uint snapshot, uint baseline, uint baseline2, uint baseline3,
        ref DataStreamReader reader, NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                return GhostReceiveSystem<WhaleGhostDeserializerCollection>.InvokeDeserialize(m_WhaleSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    public void Spawn(int serializer, int ghostId, uint snapshot, ref DataStreamReader reader,
        NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                m_WhaleSnapshotDataNewGhostIds.Add(ghostId);
                m_WhaleSnapshotDataNewGhosts.Add(GhostReceiveSystem<WhaleGhostDeserializerCollection>.InvokeSpawn<WhaleSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }

    private BufferFromEntity<WhaleSnapshotData> m_WhaleSnapshotDataFromEntity;
    private NativeList<int> m_WhaleSnapshotDataNewGhostIds;
    private NativeList<WhaleSnapshotData> m_WhaleSnapshotDataNewGhosts;
}
public struct EnableWhaleGhostReceiveSystemComponent : IComponentData
{}
public class WhaleGhostReceiveSystem : GhostReceiveSystem<WhaleGhostDeserializerCollection>
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnableWhaleGhostReceiveSystemComponent>();
    }
}
