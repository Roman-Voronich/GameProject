using Godot;
using System;

namespace GameProject
{

	public partial class HealthBar : Control
	{
		private Label _label;
		private ProgressBar _progressBar;
		private Tween _tween;
		public override void _Ready()
		{
			_label = GetNode<Label>("Label");
			_progressBar = GetNode<ProgressBar>("ProgressBar");
			
		}

		public void SetHealth(int currentHealth, int maxHealth)
		{
			_progressBar.MaxValue = maxHealth;
			//_label.Text = $"{currentHealth / maxHealth*100}";
			AnimategHealthChange(currentHealth);
		}

		private void AnimategHealthChange(int newValue)
		{
			if (_tween != null && _tween.IsValid()) _tween.Kill();
			_tween = CreateTween();
			
			_tween.TweenProperty(_progressBar,
				"Value",
				(double)newValue,
				0.3f
				).SetTrans((Tween.TransitionType.Sine))
				.SetEase(Tween.EaseType.Out);
		}
	}
}