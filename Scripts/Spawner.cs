using Godot;
using System;
namespace GameProject;

public partial class Spawner:Node2D
{
    [Export] public PackedScene MobToSpawn { get; set;}
    [Export] public float SpawnInterval { get; set; } = 5f;
    [Export] public int MaxMobs { get; set; } = 10;
    [Export] public int MaxSpawns { get; set; } = 50;
    [Export] public float SpawnRadius { get; set; } = 30f;

    [Export] public float WalkAwayDistance { get; set; } = 200f;
    
    
    private float _timer = 0f;
    private int _spawnedCount = 0;
    private int _totalSpawned = 0;
    public override void _Process(double delta)
    {
        if (_spawnedCount >= MaxSpawns || MobToSpawn == null) return;
        if (_spawnedCount <= MaxMobs)
        {
            _timer += (float)delta;
            if (_timer >= SpawnInterval)
            {
                _timer = 0f;
                SpawnMob();
            }
        }
    }

    private void SpawnMob()
    {
        if (MobToSpawn == null) return;
        var offset = new Vector2(
            (float)GD.RandRange(-SpawnRadius, SpawnRadius), 
            (float)GD.RandRange(-SpawnRadius, SpawnRadius));
        var mob = MobToSpawn.Instantiate<Mob>();
        GetTree().Root.AddChild(mob);
        mob.GlobalPosition = offset + GlobalPosition;
        var direction = GetWalkAwayDirection(offset);
        var target = GlobalPosition + direction * WalkAwayDistance;
        mob.MoveTo(target);
        _totalSpawned++;
    }

    private Vector2 GetWalkAwayDirection(Vector2 offset)
    {
        if (offset.Length() <= 5f)
        {
            var angle = GD.RandRange(0, Math.PI*2);
            return new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));
        }
        return offset.Normalized();
    }
}