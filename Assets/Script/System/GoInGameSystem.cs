using Unity.Entities;
using Unity.NetCode;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

// The system that makes the RPC request component transfer
public class GoInGameRequestSystem : RpcCommandRequestSystem<GoInGameRequest>
{
}

public class ScoreRequestSystem : RpcCommandRequestSystem<ScoreRequest>
{
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
    private ScoreMonoBehavior _counter;
    protected override void OnCreate()
    {
        _counter = GameObject.FindObjectOfType<ScoreMonoBehavior>();
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

        Entities.WithNone<SendRpcCommandRequestComponent>().ForEach((Entity reqEnt, ref ScoreRequest req, ref ReceiveRpcCommandRequestComponent reqSrc) =>
        {
            var score = GetSingleton<ScoreCompoment>();

            score.value += req.addPoints;
            _counter.SetCount(score.value);

            SetSingleton<ScoreCompoment>(score);
            PostUpdateCommands.DestroyEntity(reqEnt);
        });
    }
}

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class GoInGameServerSystem : ComponentSystem
{
    private bool first = false;
    protected override void OnCreate()
    {

    }
    protected override void OnUpdate()
    {
        // リクエストが有った時にやられる
        Entities.WithNone<SendRpcCommandRequestComponent>().ForEach((Entity reqEnt, ref GoInGameRequest req, ref ReceiveRpcCommandRequestComponent reqSrc) =>
        {
            PostUpdateCommands.AddComponent<NetworkStreamInGame>(reqSrc.SourceConnection);
            UnityEngine.Debug.Log(string.Format("Server setting connection {0} to in game", EntityManager.GetComponentData<NetworkIdComponent>(reqSrc.SourceConnection).Value));
            var ghostCollection = GetSingleton<GhostPrefabCollectionComponent>();

            var playerGhostId = WhaleGhostSerializerCollection.FindGhostType<WhaleSnapshotData>();
            var playerPrefab = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection.serverPrefabs)[playerGhostId].Value;
            var player = EntityManager.Instantiate(playerPrefab);
            EntityManager.SetComponentData(player, new PlayerCommandData { PlayerId = EntityManager.GetComponentData<NetworkIdComponent>(reqSrc.SourceConnection).Value});
            PostUpdateCommands.AddBuffer<InputCommandData>(player);

            // 最初に接続が有った時のみ
            if(!first)
            {
                // ランダムの初期化（Unity.Mathematics の利用）
                var random = new Unity.Mathematics.Random(853);

                // ゴーストコレクションを取得
                //var ghostCollection = GetSingleton<GhostPrefabCollectionComponent>();

                // ゴーストプレハブを取得
                var ghostId = WhaleGhostSerializerCollection.FindGhostType<BoidSnapshotData>();
                var prefab = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection.serverPrefabs)[ghostId].Value;

                // カウント分、エンティティの生成とコンポーネントの初期化
                for (int i = 0; i < Bootstrap.Boid.count; ++i)
                {
                    var boid = EntityManager.Instantiate(prefab);

                    // 位置
                    EntityManager.SetComponentData(boid, new Translation {Value = random.NextFloat3(1f)});
                    // 回転値
                    EntityManager.SetComponentData(boid, new Rotation { Value = quaternion.identity });
                    // 大きさ
                    //EntityManager.SetComponentData(boid, new NonUniformScale { Value = Bootstrap.Boid.scale });
                    // EntityManagerからComponentDataを追加する(Prefabに設定してもOK、その場合はSetComponentDataを使う)
                    EntityManager.AddComponentData(boid, new Velocity { Value = random.NextFloat3Direction() * Bootstrap.Param.initSpeed });
                    EntityManager.AddComponentData(boid, new Acceleration { Value = float3.zero });
                    // Dynamic Buffer の追加
                    PostUpdateCommands.AddBuffer<NeighborsEntityBuffer>(boid);
                }
                first = true;
            }

            PostUpdateCommands.SetComponent(reqSrc.SourceConnection, new CommandTargetComponent {targetEntity = player});

            PostUpdateCommands.DestroyEntity(reqEnt);
        });
    }
}
