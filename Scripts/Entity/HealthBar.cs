using Godot;

namespace GameProject
{
    public partial class HealthBar : Control
    {
        private Label _label;
        private ProgressBar _progressBar;
		
        public override void _Ready()
        {
            _label = GetNode<Label>("Label");
            _progressBar = GetNode<ProgressBar>("ProgressBar");
            _progressBar.Value = _progressBar.MaxValue;
        }

        public void SetHealth(int currentHealth, int maxHealth)
        {
            if (_progressBar == null) return;
            if (maxHealth <= 0) maxHealth = 1;
			
            _progressBar.MaxValue = maxHealth;
            _progressBar.Value = currentHealth;
			
            if (_label != null)
            {
                //_label.Text = $"{currentHealth}/{maxHealth}";
            }
        }
    }
}