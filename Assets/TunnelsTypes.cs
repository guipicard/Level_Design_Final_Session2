using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TunnelsTypes : ScriptableObject
{
    [System.Serializable]
    public struct TunnelType
    {
        public string name;
        public float width;
        public float height;
        public Sprite sprite;
    }
     
    public List<TunnelType> _tunnelsTypesList;
}
