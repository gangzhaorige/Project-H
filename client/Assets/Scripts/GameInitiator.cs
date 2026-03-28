using UnityEngine;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using ProjectH.Core;
using ProjectH.UI;

public class GameInitiator : MonoBehaviour
{
    [Header("Infrastructure")]
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Light _mainDirectionalLight;
    [SerializeField] private EventSystem _mainEventSystem;
    [SerializeField] private GameObject _mainBoard;
    [SerializeField] private LoadingScreen _loadingScreen;

    [Header("Views (UI Roots)")]
    [SerializeField] private WorldCanvasView _worldCanvasView;
    [SerializeField] private CanvasView _canvasView;

    [Header("Managers")]
    [SerializeField] private GameplayManager _gameplayManager;
    [SerializeField] private ChampionSetup _championSetup;
    [SerializeField] private HandManager _handManager;
    [SerializeField] private AnimationController _animationController;
    [SerializeField] private CardAnimationManager _cardAnimationManager;
    [SerializeField] private CardTargetSelector _cardTargetSelector;
    [SerializeField] private PlayFieldManager _playFieldManager;
    [SerializeField] private UIController _uiController;
    [SerializeField] private GameObject _spriteManager; // Generic if no script exists yet
    [SerializeField] private SkillActivationManager _skillActivationManager;
    [SerializeField] private TargetSelectionManager _targetSelectionManager;
    [SerializeField] private SelectPanelManager _selectPanelManager;

    private ObjectResolver _resolver;

    private async void Start()
    {
        _resolver = new ObjectResolver();

        BindObjects();
        using (var loadingScreenDisposable = new ShowLoadingScreenDisposable(_loadingScreen))
        {
            loadingScreenDisposable.SetLoadingPercent(0);
            await InitializeObjects();
            loadingScreenDisposable.SetLoadingPercent(0.33f);
            await CreateObjects();
            loadingScreenDisposable.SetLoadingPercent(0.66f);
            PrepareGame();
            loadingScreenDisposable.SetLoadingPercent(1f);
        }

        await BeginGame();
    }

    private void BindObjects()
    {
        // 1. Infrastructure
        Instantiate(_mainCamera);
        Instantiate(_mainDirectionalLight);
        Instantiate(_mainEventSystem);
        if (_mainBoard != null) Instantiate(_mainBoard);
        _loadingScreen = Instantiate(_loadingScreen);

        // 2. Views
        var worldCanvasView = Instantiate(_worldCanvasView);
        _resolver.RegisterInstance(worldCanvasView);

        var canvasView = Instantiate(_canvasView);
        _resolver.RegisterInstance(canvasView);

        // 3. Managers
        var gameplayManager = Instantiate(_gameplayManager);
        _resolver.RegisterInstance(gameplayManager);

        var championSetup = Instantiate(_championSetup);
        _resolver.RegisterInstance(championSetup);

        var handManager = Instantiate(_handManager);
        _resolver.RegisterInstance(handManager);

        AnimationController animationController = null;
        if (_animationController != null)
        {
            animationController = Instantiate(_animationController);
            _resolver.RegisterInstance(animationController);
        }

        var animationManager = Instantiate(_cardAnimationManager);
        _resolver.RegisterInstance(animationManager);
        
        // AnimationController often lives on the same prefab
        if (animationController == null && animationManager.TryGetComponent<AnimationController>(out var animController))
        {
            animationController = animController;
            _resolver.RegisterInstance(animationController);
        }

        var targetSelector = Instantiate(_cardTargetSelector);
        _resolver.RegisterInstance(targetSelector);

        var targetSelectionManager = Instantiate(_targetSelectionManager);
        _resolver.RegisterInstance(targetSelectionManager);

        var playFieldManager = Instantiate(_playFieldManager);
        _resolver.RegisterInstance(playFieldManager);

        var uiController = Instantiate(_uiController);
        _resolver.RegisterInstance(uiController);

        var skillActivationManager = Instantiate(_skillActivationManager);
        _resolver.RegisterInstance(skillActivationManager);

        var selectPanelManager = Instantiate(_selectPanelManager);
        _resolver.RegisterInstance(selectPanelManager);

        if (_spriteManager != null) Instantiate(_spriteManager);
    }

    private async UniTask InitializeObjects()
    {
        Debug.Log("Initializing systems...");
        
        _resolver.TryResolve<WorldCanvasView>(out var worldCanvasView);
        _resolver.TryResolve<CanvasView>(out var canvasView);
        _resolver.TryResolve<AnimationController>(out var animationController);
        _resolver.TryResolve<CardAnimationManager>(out var cardAnimationManager);
        _resolver.TryResolve<HandManager>(out var handManager);
        _resolver.TryResolve<UIController>(out var uiController);
        _resolver.TryResolve<CardTargetSelector>(out var cardTargetSelector);
        _resolver.TryResolve<TargetSelectionManager>(out var targetSelectionManager);
        _resolver.TryResolve<SkillActivationManager>(out var skillActivationManager);
        _resolver.TryResolve<SelectPanelManager>(out var selectPanelManager);
        _resolver.TryResolve<ChampionSetup>(out var championSetupInstance);
        _resolver.TryResolve<PlayFieldManager>(out var playFieldManager);
        _resolver.TryResolve<GameplayManager>(out var gameplayManager);

        // --- Execute Init Sequence ---

        if (animationController != null) animationController.Init();
        if (cardAnimationManager != null) cardAnimationManager.Init();

        if (championSetupInstance != null)
            championSetupInstance.Init(handManager, cardTargetSelector, canvasView);

        if (handManager != null) 
            handManager.Init(cardAnimationManager, animationController, worldCanvasView);

        if (playFieldManager != null) 
            playFieldManager.Init(cardAnimationManager, animationController, handManager, worldCanvasView);

        if (uiController != null) 
            uiController.Init(handManager, cardTargetSelector, canvasView);

        if (skillActivationManager != null) 
            skillActivationManager.Init(canvasView);

        if (selectPanelManager != null)
            selectPanelManager.Init(canvasView != null ? canvasView.selectPanelView : null);

        if (targetSelectionManager != null) 
            targetSelectionManager.Init(uiController, cardTargetSelector, canvasView.targetSelectionPanelView);

        if (cardTargetSelector != null) 
            cardTargetSelector.Init(uiController, targetSelectionManager, handManager);

        if (gameplayManager != null) 
            gameplayManager.Init(handManager, cardTargetSelector, canvasView, championSetupInstance);

        await UniTask.Delay(1000); 
        Debug.Log("Systems Ready.");
    }

    private async UniTask CreateObjects()
    {
        Debug.Log("Spawning objects...");
        await UniTask.Delay(500); 
        Debug.Log("Objects Created.");
    }

    private void PrepareGame() { }

    private async UniTask BeginGame() { }
}
