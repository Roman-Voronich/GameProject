using System;
using Godot;
namespace GameProject;

public interface IProducer
{
    bool IsProducing { get; }
    float ProductionProgress { get; }
    void Produce(string entityId);
    bool CanProduce(string entityId);
    event Action<IProducer, string> OnEntityProduced;
}