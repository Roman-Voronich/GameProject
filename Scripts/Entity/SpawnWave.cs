using Godot;
using System;
using System.Collections.Generic;
namespace GameProject;
using Godot.Collections;

public partial class SpawnWave : Node2D
{
    [Export] public PackedScene MobPrefab { get; set; }
    [Export] public Node2D Player { get; set; }
    
    // Настройки волн
    [Export] public Array<int> EnemiesPerWave = new Array<int>(); // [5, 8, 12, 15]
    [Export] public Array<float> WaveDelays = new Array<float>(); // [3, 5, 4, 6]
    
    // Настройки спавна
    [Export] public float SpawnDelay = 0.5f; // Задержка между мобами
    
    // Статика
    private static float _globalTimer = 0f;
    private static int _currentWave = 0;
    private static bool _waveActive = false;
    private static int _aliveEnemies = 0;
    private static List<SpawnWave> _spawners = new List<SpawnWave>();
    
    private int _spawnedCount = 0;
    private int _needToSpawn = 0;

    private IEntity oo = BuildingManager.Instance.GetEntityAt(new Vector2I(Map.width / 2 - 2, Map.height / 2 - 2))
        .Entity;
    public override void _Ready()
    {
        _spawners.Add(this);
        
        if (_spawners.Count > 1 && WaveDelays.Count > 0)
        {
            _globalTimer = WaveDelays[0];
            GD.Print($"Первая волна через {_globalTimer} сек");
        }
    }
    
    public override void _Process(double delta)
    {
        if (_waveActive) return;
        
        _globalTimer -= (float)delta;
        if (_globalTimer <= 0 && _currentWave < EnemiesPerWave.Count)
        {
            StartWave();
        }
    }
    
    private void StartWave()
    {
        _waveActive = true;
        
        // Считаем всех врагов в волне
        _aliveEnemies = 0;
        foreach (var s in _spawners)
        {
            if (_currentWave < s.EnemiesPerWave.Count)
                _aliveEnemies += s.EnemiesPerWave[_currentWave];
        }
        
        GD.Print($"Волна {_currentWave + 1}! Всего врагов: {_aliveEnemies}");
        
        // Каждый спавнер начинает спавн
        foreach (var s in _spawners)
        {
            s.StartSpawning();
        }
    }
    
    private void StartSpawning()
    {
        if (_currentWave >= EnemiesPerWave.Count)
        {
            _needToSpawn = 0;
            return;
        }
        
        _needToSpawn = EnemiesPerWave[_currentWave];
        _spawnedCount = 0;
        
        SpawnMob();
    }
    
    private void SpawnMob()
    {
        if (_spawnedCount >= _needToSpawn) return;
        
        // Создаем моба
        Mob mob = MobPrefab.Instantiate<Mob>();
        mob.GlobalPosition = GetRandomOffset();
        
        // Просто говорим мобу кто его цель
         //mob.SetUpTarget(oo);
        
        mob.Death += () => {
            _aliveEnemies--;
            if (_aliveEnemies <= 0) EndWave();
        };
        
        GetTree().CurrentScene.AddChild(mob);
        _spawnedCount++;
        
        // Следующий моб через задержку
        if (_spawnedCount < _needToSpawn)
        {
            GetTree().CreateTimer(SpawnDelay).Timeout += () => SpawnMob();
        }
    }
    
    private void EndWave()
    {
        _waveActive = false;
        _currentWave++;
        
        if (_currentWave < WaveDelays.Count)
        {
            _globalTimer = WaveDelays[_currentWave];
            GD.Print($"Волна {_currentWave} завершена. Следующая через {_globalTimer} сек");
        }
        else
        {
            GD.Print("ПОБЕДА! Все волны пройдены!");
        }
    }
    
    private Vector2 GetRandomOffset()
    {
        return GlobalPosition + new Vector2(
            (float)(GD.Randf() - 0.5f) * 50f,
            (float)(GD.Randf() - 0.5f) * 50f
        );
    }
    
    public override void _ExitTree()
    {
        _spawners.Remove(this);
    }
}