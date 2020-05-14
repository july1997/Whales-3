using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using Unity.Rendering;

public struct WhaleGhostSerializer : IGhostSerializer<WhaleSnapshotData>
{
    private ComponentType componentTypePlayerCommandData;
    private ComponentType componentTypePhysicsCollider;
    private ComponentType componentTypePhysicsGravityFactor;
    private ComponentType componentTypePhysicsMass;
    private ComponentType componentTypePhysicsVelocity;
    private ComponentType componentTypeLocalToWorld;
    private ComponentType componentTypeRotation;
    private ComponentType componentTypeTranslation;
    private ComponentType componentTypeLinkedEntityGroup;
    // FIXME: These disable safety since all serializers have an instance of the same type - causing aliasing. Should be fixed in a cleaner way
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<PlayerCommandData> ghostPlayerCommandDataType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Rotation> ghostRotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Translation> ghostTranslationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkBufferType<LinkedEntityGroup> ghostLinkedEntityGroupType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Rotation> ghostChild0RotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Translation> ghostChild0TranslationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Rotation> ghostChild1RotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Translation> ghostChild1TranslationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Rotation> ghostChild2RotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Translation> ghostChild2TranslationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Rotation> ghostChild3RotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Translation> ghostChild3TranslationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Rotation> ghostChild4RotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Translation> ghostChild4TranslationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Rotation> ghostChild5RotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Translation> ghostChild5TranslationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Rotation> ghostChild6RotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Translation> ghostChild6TranslationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Rotation> ghostChild7RotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Translation> ghostChild7TranslationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Rotation> ghostChild8RotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Translation> ghostChild8TranslationType;


    public int CalculateImportance(ArchetypeChunk chunk)
    {
        return 1;
    }

    public int SnapshotSize => UnsafeUtility.SizeOf<WhaleSnapshotData>();
    public void BeginSerialize(ComponentSystemBase system)
    {
        componentTypePlayerCommandData = ComponentType.ReadWrite<PlayerCommandData>();
        componentTypePhysicsCollider = ComponentType.ReadWrite<PhysicsCollider>();
        componentTypePhysicsGravityFactor = ComponentType.ReadWrite<PhysicsGravityFactor>();
        componentTypePhysicsMass = ComponentType.ReadWrite<PhysicsMass>();
        componentTypePhysicsVelocity = ComponentType.ReadWrite<PhysicsVelocity>();
        componentTypeLocalToWorld = ComponentType.ReadWrite<LocalToWorld>();
        componentTypeRotation = ComponentType.ReadWrite<Rotation>();
        componentTypeTranslation = ComponentType.ReadWrite<Translation>();
        componentTypeLinkedEntityGroup = ComponentType.ReadWrite<LinkedEntityGroup>();
        ghostPlayerCommandDataType = system.GetArchetypeChunkComponentType<PlayerCommandData>(true);
        ghostRotationType = system.GetArchetypeChunkComponentType<Rotation>(true);
        ghostTranslationType = system.GetArchetypeChunkComponentType<Translation>(true);
        ghostLinkedEntityGroupType = system.GetArchetypeChunkBufferType<LinkedEntityGroup>(true);
        ghostChild0RotationType = system.GetComponentDataFromEntity<Rotation>(true);
        ghostChild0TranslationType = system.GetComponentDataFromEntity<Translation>(true);
        ghostChild1RotationType = system.GetComponentDataFromEntity<Rotation>(true);
        ghostChild1TranslationType = system.GetComponentDataFromEntity<Translation>(true);
        ghostChild2RotationType = system.GetComponentDataFromEntity<Rotation>(true);
        ghostChild2TranslationType = system.GetComponentDataFromEntity<Translation>(true);
        ghostChild3RotationType = system.GetComponentDataFromEntity<Rotation>(true);
        ghostChild3TranslationType = system.GetComponentDataFromEntity<Translation>(true);
        ghostChild4RotationType = system.GetComponentDataFromEntity<Rotation>(true);
        ghostChild4TranslationType = system.GetComponentDataFromEntity<Translation>(true);
        ghostChild5RotationType = system.GetComponentDataFromEntity<Rotation>(true);
        ghostChild5TranslationType = system.GetComponentDataFromEntity<Translation>(true);
        ghostChild6RotationType = system.GetComponentDataFromEntity<Rotation>(true);
        ghostChild6TranslationType = system.GetComponentDataFromEntity<Translation>(true);
        ghostChild7RotationType = system.GetComponentDataFromEntity<Rotation>(true);
        ghostChild7TranslationType = system.GetComponentDataFromEntity<Translation>(true);
        ghostChild8RotationType = system.GetComponentDataFromEntity<Rotation>(true);
        ghostChild8TranslationType = system.GetComponentDataFromEntity<Translation>(true);
    }

    public void CopyToSnapshot(ArchetypeChunk chunk, int ent, uint tick, ref WhaleSnapshotData snapshot, GhostSerializerState serializerState)
    {
        snapshot.tick = tick;
        var chunkDataPlayerCommandData = chunk.GetNativeArray(ghostPlayerCommandDataType);
        var chunkDataRotation = chunk.GetNativeArray(ghostRotationType);
        var chunkDataTranslation = chunk.GetNativeArray(ghostTranslationType);
        var chunkDataLinkedEntityGroup = chunk.GetBufferAccessor(ghostLinkedEntityGroupType);
        snapshot.SetPlayerCommandDataPlayerId(chunkDataPlayerCommandData[ent].PlayerId, serializerState);
        snapshot.SetRotationValue(chunkDataRotation[ent].Value, serializerState);
        snapshot.SetTranslationValue(chunkDataTranslation[ent].Value, serializerState);
        snapshot.SetChild0RotationValue(ghostChild0RotationType[chunkDataLinkedEntityGroup[ent][1].Value].Value, serializerState);
        snapshot.SetChild0TranslationValue(ghostChild0TranslationType[chunkDataLinkedEntityGroup[ent][1].Value].Value, serializerState);
        snapshot.SetChild1RotationValue(ghostChild1RotationType[chunkDataLinkedEntityGroup[ent][2].Value].Value, serializerState);
        snapshot.SetChild1TranslationValue(ghostChild1TranslationType[chunkDataLinkedEntityGroup[ent][2].Value].Value, serializerState);
        snapshot.SetChild2RotationValue(ghostChild2RotationType[chunkDataLinkedEntityGroup[ent][3].Value].Value, serializerState);
        snapshot.SetChild2TranslationValue(ghostChild2TranslationType[chunkDataLinkedEntityGroup[ent][3].Value].Value, serializerState);
        snapshot.SetChild3RotationValue(ghostChild3RotationType[chunkDataLinkedEntityGroup[ent][4].Value].Value, serializerState);
        snapshot.SetChild3TranslationValue(ghostChild3TranslationType[chunkDataLinkedEntityGroup[ent][4].Value].Value, serializerState);
        snapshot.SetChild4RotationValue(ghostChild4RotationType[chunkDataLinkedEntityGroup[ent][5].Value].Value, serializerState);
        snapshot.SetChild4TranslationValue(ghostChild4TranslationType[chunkDataLinkedEntityGroup[ent][5].Value].Value, serializerState);
        snapshot.SetChild5RotationValue(ghostChild5RotationType[chunkDataLinkedEntityGroup[ent][6].Value].Value, serializerState);
        snapshot.SetChild5TranslationValue(ghostChild5TranslationType[chunkDataLinkedEntityGroup[ent][6].Value].Value, serializerState);
        snapshot.SetChild6RotationValue(ghostChild6RotationType[chunkDataLinkedEntityGroup[ent][7].Value].Value, serializerState);
        snapshot.SetChild6TranslationValue(ghostChild6TranslationType[chunkDataLinkedEntityGroup[ent][7].Value].Value, serializerState);
        snapshot.SetChild7RotationValue(ghostChild7RotationType[chunkDataLinkedEntityGroup[ent][8].Value].Value, serializerState);
        snapshot.SetChild7TranslationValue(ghostChild7TranslationType[chunkDataLinkedEntityGroup[ent][8].Value].Value, serializerState);
        snapshot.SetChild8RotationValue(ghostChild8RotationType[chunkDataLinkedEntityGroup[ent][9].Value].Value, serializerState);
        snapshot.SetChild8TranslationValue(ghostChild8TranslationType[chunkDataLinkedEntityGroup[ent][9].Value].Value, serializerState);
    }
}
