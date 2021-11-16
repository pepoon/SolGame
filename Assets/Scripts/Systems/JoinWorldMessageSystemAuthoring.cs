using DOTSNET;
using System;
using UnityEngine;

namespace SolGame
{
    public class JoinWorldMessageSystemAuthoring : MonoBehaviour, SelectiveSystemAuthoring
    {
        // add system if Authoring is used
        public Type GetSystemType() => typeof(JoinWorldMessageSystem);
    }
}