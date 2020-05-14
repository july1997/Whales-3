using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Mathematics;

public struct InputCommandData : ICommandData<InputCommandData>
{
    public uint Tick => tick;
    public uint tick;
    public float angleH;
    public float angleV;
    public float3 posVector;
    public float speed;

    public void Deserialize(uint tick,ref DataStreamReader reader)
    {
        this.tick = tick;
        angleH = reader.ReadFloat();
        angleV = reader.ReadFloat();
        speed = reader.ReadFloat();
    }

    public void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteFloat(angleH);
        writer.WriteFloat(angleV);
        writer.WriteFloat(speed);
    }

    public void Deserialize(uint tick,ref DataStreamReader reader, InputCommandData baseline,
        NetworkCompressionModel compressionModel)
    {
        Deserialize(tick,ref reader);
    }

    public void Serialize(ref DataStreamWriter writer, InputCommandData baseline, NetworkCompressionModel compressionModel)
    {
        Serialize(ref writer);
    }
}
