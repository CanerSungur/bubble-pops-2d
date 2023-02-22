using UnityEngine;

namespace BubblePops
{
    public class UiManager : MonoBehaviour
    {
        #region COMPONENTS
        private GameManager _gameManager;
        #endregion

        [Header("-- SETUP --")]
        [SerializeField] private Hud _hud;
        [SerializeField] private SettingsCanvas _settings;
        [SerializeField] private LevelUpCanvas _levelUp;

        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;

            _hud.Init(this);
            _levelUp.Init(this);
            _settings.Init(this);
        }
    }
}
