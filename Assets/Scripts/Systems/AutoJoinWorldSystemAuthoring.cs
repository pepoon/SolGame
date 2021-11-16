using DOTSNET;
using System;
using UnityEngine;
namespace SolGame
{
    public class AutoJoinWorldSystemAuthoring : MonoBehaviour, SelectiveSystemAuthoring
    {
        // add system if Authoring is used
        public Type GetSystemType() => typeof(AutoJoinWorldSystem);
    }
}