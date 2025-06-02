using System;
using System.Collections.Generic;
using System.Linq;
using Player.Checkpoints.Serialization;
using UnityEngine;

public class CheckpointManager : Utilities.Singletons.RegulatorSingleton<CheckpointManager>
{
    [SerializeField] private Utilities.Serializables.InterfaceReference<IRespawnable> _player;
    
    public IRespawnable Player => _player.Value;


    private void FindStateObjects()
    {
    }
   


    protected override void Initialize()
    {
        if (_player == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null || !player.TryGetComponent(out IRespawnable component))
            {
                throw new System.Exception("No player found with a respawnable component.");
            }

            _player.UnderlyingValue = player;
            _player.Value = component;
        }
    }
}