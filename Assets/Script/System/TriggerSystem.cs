using Unity.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class TriggerSystem : JobComponentSystem
{
    EntityQuery playerGroup;
    EntityQuery boidsGroup;
    EntityQuery connectionGroup;
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
        connectionGroup = GetEntityQuery(typeof(CommandTargetComponent));
    }

    private struct TriggerJob : ITriggerEventsJob
    {
        // Jobの完了時に自動的にDispose
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> boidsEntities;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> playerEntities;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> connectionEntities;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<CommandTargetComponent> connectionComponent;
        public EntityCommandBuffer CommandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.Entities.EntityA;
            var entityB = triggerEvent.Entities.EntityB;
            var targetEntity = new Entity();

            // プレイヤーのエンティティか確認
            if (playerEntities.Contains(entityA))
            {
                // 魚のエンティティか
                if (boidsEntities.Contains(entityB))
                {
                    // 魚を消す
                    CommandBuffer.DestroyEntity(entityB);

                    targetEntity = entityA;
                }
            } 
            else if(playerEntities.Contains(entityB))
            {
                if(boidsEntities.Contains(entityA))
                {
                    CommandBuffer.DestroyEntity(entityA);

                    targetEntity = entityB;
                }
            }

            for(var i = 0; i < connectionComponent.Length; i++)
            {
                if(connectionComponent[i].targetEntity == targetEntity)
                {
                    var req = CommandBuffer.CreateEntity();
                    CommandBuffer.AddComponent<ScoreRequest>(req, new ScoreRequest {addPoints = 1} );
                    CommandBuffer.AddComponent(req, new SendRpcCommandRequestComponent { TargetConnection = connectionEntities[i] });
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
            connectionEntities = connectionGroup.ToEntityArray(Allocator.TempJob),
            connectionComponent = connectionGroup.ToComponentDataArray<CommandTargetComponent>(Allocator.TempJob),
            CommandBuffer = _bufferSystem.CreateCommandBuffer()
        }.Schedule(_stepPhysicsWorldSystem.Simulation, ref _buildPhysicsWorldSystem.PhysicsWorld, inputDeps);

        _bufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}
