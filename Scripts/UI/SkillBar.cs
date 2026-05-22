using Godot;
using System;
using System.Collections.Generic;

public partial class SkillBar : HBoxContainer
{
	[Export] public SkillData[] Skills { get; set; } = new SkillData[0];
	private PackedScene _slotScene = GD.Load<PackedScene>("res://Scenes/UI/SkillSlot.tscn");
	private Dictionary<Key, SkillSlot> _hotkeyMap = new();
	private ButtonGroup _group = new();
	[Signal]public delegate void SkillUsedEventHandler(SkillData skill, bool isToggledOn);
	
	void OnSlotPressed(SkillData slot, bool isToggledOn)
	{
		GD.Print(slot.Name + " Pressed");
		EmitSignal(SignalName.SkillUsed, slot,isToggledOn);
	}

	
	public void ReBuild()
	{
		foreach (var child in GetChildren())
		{
			RemoveChild(child);
			child.QueueFree();
		}
		
		_hotkeyMap.Clear();

		foreach (var skill in Skills)
		{
			if (skill == null) continue;

			var slot = _slotScene.Instantiate<SkillSlot>();
			slot.ButtonGroup = _group;
			AddChild(slot);

			slot.Setup(skill);
			slot.SkillClicked += OnSlotPressed;
			RegisterHotkey(slot);
		}
	}
	public override void _Ready()
	{
		ReBuild();
	}
	
	private void RegisterHotkey(SkillSlot slot)
	{
		if (string.IsNullOrEmpty(slot.Data.Keybind)) return;

		Key key = ParseKey(slot.Data.Keybind);
		if (key != Key.None && !_hotkeyMap.ContainsKey(key))
		{
			_hotkeyMap[key] = slot;
		}
	}
	
	private static Key ParseKey(string str)
	{
		if (string.IsNullOrEmpty(str)) return Key.None;
    
		string upper = str.ToUpperInvariant();
    
		if (upper.StartsWith("F") && upper.Length > 1 && int.TryParse(upper.Substring(1), out int fNum))
		{
			if (fNum >= 1 && fNum <= 12)
				return Key.F1 + (fNum - 1);
		}
    
		if (upper.Length == 1 && upper[0] >= '0' && upper[0] <= '9')
		{
			return (Key)((int)Key.Key0 + (upper[0] - '0'));
		}
    
		if (upper.Length == 1 && upper[0] >= 'A' && upper[0] <= 'Z')
		{
			return (Key)((int)Key.A + (upper[0] - 'A'));
		}
    
		if (upper.StartsWith("KP") && upper.Length > 2 && int.TryParse(upper.Substring(2), out int kpNum))
		{
			if (kpNum >= 0 && kpNum <= 9)
				return (Key)((int)Key.Kp0 + kpNum);
		}
    
		return Key.None;
	}
	public bool TryHandleHotkey(InputEventKey keyEvent)
	{
		if (!keyEvent.Pressed || keyEvent.Echo) return false;

		if (_hotkeyMap.TryGetValue(keyEvent.Keycode, out var slot))
		{
			slot.ButtonPressed = !slot.ButtonPressed;
			return true;
		}

		return false;
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent)
		{
			if (TryHandleHotkey(keyEvent))
			{
				AcceptEvent();
			}
		}
	}
}
