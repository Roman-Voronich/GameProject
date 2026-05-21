using Godot;

namespace GameProject
{
    public partial class HealthBar : Control
    {
        private ProgressBar _progressBar;
        private ProgressBar _delayBar;
        private Tween tween;
        public override void _Ready()
        {
            _progressBar = GetNode<ProgressBar>("ProgressBar");
            _delayBar = GetNode<ProgressBar>("DelayBar");
            _progressBar.Value = _progressBar.MaxValue;
            _delayBar.Value = _delayBar.MaxValue;
        }

        public void SetHealth(int currentHealth, int maxHealth)
        {
            if (_progressBar == null) return;
            if (maxHealth <= 0) maxHealth = 1;
            if (!Visible) Visible = true;

            _progressBar.MaxValue = maxHealth;
            _progressBar.Value = currentHealth;
            tween = CreateTween();
            tween.TweenProperty(_delayBar,"value", currentHealth, 1.2f )
                .SetDelay(0.2f).SetEase(Tween.EaseType.InOut)
                .SetTrans(Tween.TransitionType.Expo);
        }
    }
}