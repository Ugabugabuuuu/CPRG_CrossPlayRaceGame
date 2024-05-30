using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable {


    public ulong clientId;
    public int colorId;
    public float finishTime;
    public int finishPlace;
    public FixedString64Bytes playerName;
    public FixedString64Bytes playerId;


    public bool Equals(PlayerData other) {
        return
            clientId == other.clientId &&
            colorId == other.colorId &&
            finishTime == other.finishTime &&
            finishPlace == other.finishPlace &&
            playerName == other.playerName &&
            playerId == other.playerId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
        serializer.SerializeValue(ref finishTime);
        serializer.SerializeValue(ref finishPlace);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerId);
    }

}