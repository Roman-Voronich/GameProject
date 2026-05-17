using Godot;
using System;

public partial class ResSlot : Node
{
	public ResourceData Data { get; set; }
	private TextureRect _textureRect;
	private Label _label;
	

	public override void _Ready()
	{
		_textureRect = GetNode<TextureRect>("Texture");
		_label = GetNode<Label>("Count");
	}

	public void Setup(ResourceData data, int count)
	{
		Data = data;
		_textureRect.Texture = data.Texture;
		_label.Text = count.ToString();
	}

	public void Update(int count)
	{
		_label.Text = count.ToString();
	}
}
