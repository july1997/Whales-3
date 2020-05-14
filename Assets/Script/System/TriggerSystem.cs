using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class TriggerSystem : JobComponentSystem
{
    EntityQuery playerGroup;
    EntityQuery boidsGroup;
    private BuildPhysicsWorld _buildPhysicsWorldSystem;
    private StepPhysicsWorld _stepPhysicsWorldSystem;
    private EntityCommandBufferSystem _bufferSystem;

    protected override void OnCreate()
    {
        _buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
        _bufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        playerGroup = GetEntityQuery(typeof(Translation), typeof(PlayerCommandData));
        boidsGroup = GetEntityQuery(typeof(Translation), typeof(Velocity), typeof(Acceleration));
    }

    private struct TriggerJob : ITriggerEventsJob
    {
        // Jobの完了時に自動的にDispose
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> boidsEntities;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> playerEntities;
        public EntityCommandBuffer CommandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.Entities.EntityA;
            var entityB = triggerEvent.Entities.EntityB;

            // プレイヤーのエンティティか確認
            if (playerEntities.Contains(entityA))
            {
                // 魚のエンティティか
                if (boidsEntities.Contains(entityB))
                {
                    // 魚を消す
                    CommandBuffer.DestroyEntity(entityB);
                }
            } 
            else if(playerEntities.Contains(entityB))
            {
                if(boidsEntities.Contains(entityA))
                {
                    CommandBuffer.DestroyEntity(entityA);
                }
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var jobHandle = new TriggerJob
        {
            boidsEntities = boidsGroup.ToEntityArray(Allocator.TempJob),
            playerEntities = playerGroup.ToEntityArray(Allocator.TempJob),
            CommandBuffer = _bufferSystem.CreateCommandBuffer()
        }.Schedule(_stepPhysicsWorldSystem.Simulation, ref _buildPhysicsWorldSystem.PhysicsWorld, inputDeps);

        _bufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}
