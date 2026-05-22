using Godot;
using System;

public partial class ResSlot : Node
{
	public ResourceData Data { get; set; }
	private TextureRect _textureRect;
	private Label _count;
	private Label _passiveIncome;
	
	

	public override void _Ready()
	{
		_textureRect = GetNode<TextureRect>("Texture");
		_count = GetNode<Label>("Count");
		_passiveIncome = GetNode<Label>("PassiveIncome");
	}

	public void Setup(ResourceData data, int count)
	{
		Data = data;
		_textureRect.Texture = data.Texture;
		_count.Text = count.ToString();
	}

	public void Update(int count)
	{
		_count.Text = count.ToString();
	}

	public void UpdateIncome(float count)
	{
		_passiveIncome.Text = "+" + Math.Round(count,3).ToString();
	}
}
