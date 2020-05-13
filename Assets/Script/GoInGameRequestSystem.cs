using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Burst;


// The system that makes the RPC request component transfer
public class GoInGameRequestSystem : RpcCommandRequestSystem<GoInGameRequest>
{
}

[BurstCompile]
public struct GoInGameRequest : IRpcCommand
{
    public void Deserialize(ref DataStreamReader reader)
    {
    }

    public void Serialize(ref DataStreamWriter writer)
    {
    }
    [BurstCompile]
    private static void InvokeExecute(ref RpcExecutor.Parameters parameters)
    {
        RpcExecutor.ExecuteCreateRequestComponent<GoInGameRequest>(ref parameters);
    }

    static PortableFunctionPointer<RpcExecutor.ExecuteDelegate> InvokeExecuteFunctionPointer =
        new PortableFunctionPointer<RpcExecutor.ExecuteDelegate>(InvokeExecute);
    public PortableFunctionPointer<RpcExecutor.ExecuteDelegate> CompileExecute()
    {
        return InvokeExecuteFunctionPointer;
    }
}

public class NetCubeSendCommandSystem : CommandSendSystem<InputCommandData>
{
}
public class NetCubeReceiveCommandSystem : CommandReceiveSystem<InputCommandData>
{
}

[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class GoInGameClientSystem : ComponentSystem
{
    protected override void OnCreate()
    {
    }

    protected override void OnUpdate()
    {
        Entities.WithNone<NetworkStreamInGame>().ForEach((Entity ent, ref NetworkIdComponent id) =>
        {
            PostUpdateCommands.AddComponent<NetworkStreamInGame>(ent);
            var req = PostUpdateCommands.CreateEntity();
            PostUpdateCommands.AddComponent<GoInGameRequest>(req);
            PostUpdateCommands.AddComponent(req, new SendRpcCommandRequestComponent { TargetConnection = ent });
        });
    }
}

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class GoInGameServerSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithNone<SendRpcCommandRequestComponent>().ForEach((Entity reqEnt, ref GoInGameRequest req, ref ReceiveRpcCommandRequestComponent reqSrc) =>
        {
            PostUpdateCommands.AddComponent<NetworkStreamInGame>(reqSrc.SourceConnection);
            UnityEngine.Debug.Log(string.Format("Server setting connection {0} to in game", EntityManager.GetComponentData<NetworkIdComponent>(reqSrc.SourceConnection).Value));
            var ghostCollection = GetSingleton<GhostPrefabCollectionComponent>();
            var ghostId = WhaleGhostSerializerCollection.FindGhostType<WhaleSnapshotData>();
            var prefab = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection.serverPrefabs)[ghostId].Value;
            // プレイヤー（キューブを作成）
            var player = EntityManager.Instantiate(prefab);

            EntityManager.SetComponentData(player, new PlayerCommandData { PlayerId = EntityManager.GetComponentData<NetworkIdComponent>(reqSrc.SourceConnection).Value});
            PostUpdateCommands.AddBuffer<InputCommandData>(player);

            PostUpdateCommands.SetComponent(reqSrc.SourceConnection, new CommandTargetComponent {targetEntity = player});

            PostUpdateCommands.DestroyEntity(reqEnt);
        });
    }
}
