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
            "BoidGhostSerializer",
            "WhaleGhostSerializer",
        };
        return arr;
    }

    public int Length => 2;
#endif
    public void Initialize(World world)
    {
        var curBoidGhostSpawnSystem = world.GetOrCreateSystem<BoidGhostSpawnSystem>();
        m_BoidSnapshotDataNewGhostIds = curBoidGhostSpawnSystem.NewGhostIds;
        m_BoidSnapshotDataNewGhosts = curBoidGhostSpawnSystem.NewGhosts;
        curBoidGhostSpawnSystem.GhostType = 0;
        var curWhaleGhostSpawnSystem = world.GetOrCreateSystem<WhaleGhostSpawnSystem>();
        m_WhaleSnapshotDataNewGhostIds = curWhaleGhostSpawnSystem.NewGhostIds;
        m_WhaleSnapshotDataNewGhosts = curWhaleGhostSpawnSystem.NewGhosts;
        curWhaleGhostSpawnSystem.GhostType = 1;
    }

    public void BeginDeserialize(JobComponentSystem system)
    {
        m_BoidSnapshotDataFromEntity = system.GetBufferFromEntity<BoidSnapshotData>();
        m_WhaleSnapshotDataFromEntity = system.GetBufferFromEntity<WhaleSnapshotData>();
    }
    public bool Deserialize(int serializer, Entity entity, uint snapshot, uint baseline, uint baseline2, uint baseline3,
        ref DataStreamReader reader, NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                return GhostReceiveSystem<WhaleGhostDeserializerCollection>.InvokeDeserialize(m_BoidSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            case 1:
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
                m_BoidSnapshotDataNewGhostIds.Add(ghostId);
                m_BoidSnapshotDataNewGhosts.Add(GhostReceiveSystem<WhaleGhostDeserializerCollection>.InvokeSpawn<BoidSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            case 1:
                m_WhaleSnapshotDataNewGhostIds.Add(ghostId);
                m_WhaleSnapshotDataNewGhosts.Add(GhostReceiveSystem<WhaleGhostDeserializerCollection>.InvokeSpawn<WhaleSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }

    private BufferFromEntity<BoidSnapshotData> m_BoidSnapshotDataFromEntity;
    private NativeList<int> m_BoidSnapshotDataNewGhostIds;
    private NativeList<BoidSnapshotData> m_BoidSnapshotDataNewGhosts;
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
