using UnityEngine;
using Unity.Netcode;
using NUnit.Framework.Constraints;

public struct FollowMoveCommand : INetworkSerializable
{
    public Vector3 Direction;
    public float Time;
    public DirectionType directionType;

    public FollowMoveCommand(DirectionType directionType, float time)
    {
        this.directionType = directionType;
        this.Time = time;

        switch (directionType)
        {
            case DirectionType.Forward:
                this.Direction = Vector3.forward;
                break;
            case DirectionType.Backward:
                this.Direction = Vector3.back;
                break;
            case DirectionType.Left:
                this.Direction = Vector3.left;
                break;
            case DirectionType.Right:
                this.Direction = Vector3.right;
                break;
            default:
                this.Direction = Vector3.zero;
                break;
        }
    }

    public static FollowMoveCommand None => new FollowMoveCommand(DirectionType.None, 0);

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Direction);
        serializer.SerializeValue(ref Time);
        serializer.SerializeValue(ref directionType);
    }

    public enum DirectionType
    {
        None,
        Forward,
        Backward,
        Left,
        Right
    }
}
