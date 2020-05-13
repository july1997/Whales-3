using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Mathematics;

public struct InputCommandData : ICommandData<InputCommandData>
{
    public uint Tick => tick;
    public uint tick;
    public float horizontal;
    public float vertical;

    public void Deserialize(uint tick,ref DataStreamReader reader)
    {
        this.tick = tick;
        horizontal = reader.ReadFloat();
        vertical = reader.ReadFloat();
    }

    public void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteFloat(horizontal);
        writer.WriteFloat(vertical);
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
