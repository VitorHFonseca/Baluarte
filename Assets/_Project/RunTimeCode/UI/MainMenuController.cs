using System;
using GameCore.Data;
using GameCore.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameCore.UI
{
    public class MainMenuController : MonoBehaviour
    {
        private const string GameVersion = "0.0.0.4";

        private readonly Color _paper = new Color(0.82f, 0.73f, 0.55f);
        private readonly Color _ink = new Color(0.12f, 0.08f, 0.04f);
        private readonly Color _dark = new Color(0.12f, 0.07f, 0.04f);
        private readonly Color _panel = new Color(0.2f, 0.12f, 0.07f, 0.94f);
        private readonly Color _gold = new Color(0.92f, 0.66f, 0.22f);
        private readonly Color _green = new Color(0.25f, 0.55f, 0.32f);

        private Canvas _canvas;
        private RectTransform _root;
        private InputField _nameInput;
        private CharacterBodyType _selectedBody = CharacterBodyType.Homem;
        private PlayerStats _draftStats = PlayerStats.CreateEmpty();
        private string _draftName = string.Empty;
        private const int CreationStatPoints = 5;

        private bool _showingCapture;
        private Text _timerText;
        private Text _energyText;
        private Text _captureCountText;
        private Text _creatureText;
        private Text _resistanceText;

        private void Start()
        {
            EnsureRuntimeObjects();
            ShowHome();
        }

        private void Update()
        {
            if (_showingCapture) UpdateCaptureHud();
        }

        private void EnsureRuntimeObjects()
        {
            if (PlayerProfileService.Instance == null)
            {
                new GameObject("PlayerProfileService").AddComponent<PlayerProfileService>();
            }

            if (GameManager.Instance == null)
            {
                new GameObject("GameManager").AddComponent<GameManager>();
            }

            if (FindAnyObjectByType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }

            if (_canvas != null) return;

            GameObject canvasObject = new GameObject("Canvas_MainMenu");
            _canvas = canvasObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();

            _root = CreatePanel("Root", canvasObject.transform, _dark);
            Stretch(_root);
        }

        private void ClearRoot()
        {
            _showingCapture = false;

            for (int i = _root.childCount - 1; i >= 0; i--)
            {
                Destroy(_root.GetChild(i).gameObject);
            }
        }

        private void ShowHome()
        {
            ClearRoot();

            RectTransform frame = CreatePanel("HomeFrame", _root, _paper);
            SetAnchors(frame, new Vector2(0.18f, 0.12f), new Vector2(0.82f, 0.88f));

            Text title = CreateText("Baluarte", frame, 52, FontStyle.Bold, _ink);
            SetAnchors(title.rectTransform, new Vector2(0.08f, 0.78f), new Vector2(0.92f, 0.94f));

            Text subtitle = CreateText("Captura 2D e defesa top-down", frame, 20, FontStyle.Bold, _ink);
            SetAnchors(subtitle.rectTransform, new Vector2(0.1f, 0.68f), new Vector2(0.9f, 0.76f));

            PlayerProfile profile = PlayerProfileService.Instance.CurrentProfile;
            if (profile == null)
            {
                Button createButton = CreateButton("Criar Personagem", frame, _gold);
                SetAnchors(createButton.GetComponent<RectTransform>(), new Vector2(0.32f, 0.52f), new Vector2(0.68f, 0.64f));
                createButton.onClick.AddListener(ShowCharacterCreation);
            }
            else
            {
                Text profileText = CreateText(
                    $"{profile.nome} | {profile.corpo}\nVigor {profile.stats.vigor}   Foco {profile.stats.foco}   Empatia {profile.stats.empatiaAnimal}",
                    frame,
                    18,
                    FontStyle.Normal,
                    _ink);
                SetAnchors(profileText.rectTransform, new Vector2(0.12f, 0.54f), new Vector2(0.88f, 0.66f));

                Button startButton = CreateButton("Iniciar Jornada", frame, _gold);
                SetAnchors(startButton.GetComponent<RectTransform>(), new Vector2(0.28f, 0.39f), new Vector2(0.72f, 0.5f));
                startButton.onClick.AddListener(StartGame);

                Button newProfileButton = CreateButton("Excluir Personagem", frame, _green);
                SetAnchors(newProfileButton.GetComponent<RectTransform>(), new Vector2(0.34f, 0.26f), new Vector2(0.66f, 0.35f));
                newProfileButton.onClick.AddListener(() =>
                {
                    PlayerProfileService.Instance.DeleteProfile();
                    _draftName = string.Empty;
                    _selectedBody = CharacterBodyType.Homem;
                    _draftStats = PlayerStats.CreateEmpty();
                    ShowCharacterCreation();
                });
            }

            Text version = CreateText($"v{GameVersion}", frame, 14, FontStyle.Normal, _ink);
            SetAnchors(version.rectTransform, new Vector2(0.78f, 0.04f), new Vector2(0.94f, 0.1f));
        }

        private void ShowCharacterCreation()
        {
            ClearRoot();

            RectTransform frame = CreatePanel("CharacterCreationFrame", _root, _paper);
            SetAnchors(frame, new Vector2(0.2f, 0.08f), new Vector2(0.8f, 0.92f));

            Text title = CreateText("Criar Personagem", frame, 38, FontStyle.Bold, _ink);
            SetAnchors(title.rectTransform, new Vector2(0.08f, 0.84f), new Vector2(0.92f, 0.94f));

            Text intro = CreateText("Escolha uma base visual e distribua 5 pontos de status.", frame, 18, FontStyle.Normal, _ink);
            SetAnchors(intro.rectTransform, new Vector2(0.12f, 0.74f), new Vector2(0.88f, 0.82f));

            _nameInput = CreateInput(frame, "Nome do personagem");
            _nameInput.text = _draftName;
            _nameInput.onValueChanged.AddListener(value => _draftName = value);
            SetAnchors(_nameInput.GetComponent<RectTransform>(), new Vector2(0.22f, 0.63f), new Vector2(0.78f, 0.72f));

            Button male = CreateButton(_selectedBody == CharacterBodyType.Homem ? "Homem selecionado" : "Homem", frame, _selectedBody == CharacterBodyType.Homem ? _green : _gold);
            SetAnchors(male.GetComponent<RectTransform>(), new Vector2(0.2f, 0.49f), new Vector2(0.47f, 0.6f));
            male.onClick.AddListener(() => SelectBody(CharacterBodyType.Homem));

            Button female = CreateButton(_selectedBody == CharacterBodyType.Mulher ? "Mulher selecionada" : "Mulher", frame, _selectedBody == CharacterBodyType.Mulher ? _green : _gold);
            SetAnchors(female.GetComponent<RectTransform>(), new Vector2(0.53f, 0.49f), new Vector2(0.8f, 0.6f));
            female.onClick.AddListener(() => SelectBody(CharacterBodyType.Mulher));

            int remainingPoints = CreationStatPoints - _draftStats.Total;
            Text points = CreateText($"Pontos restantes: {remainingPoints}", frame, 22, FontStyle.Bold, _ink);
            SetAnchors(points.rectTransform, new Vector2(0.28f, 0.42f), new Vector2(0.72f, 0.49f));

            CreateStatRow(frame, "Vigor", _draftStats.vigor, new Vector2(0.22f, 0.34f), new Vector2(0.78f, 0.41f), () => AddStatPoint("vigor"), () => RemoveStatPoint("vigor"));
            CreateStatRow(frame, "Foco", _draftStats.foco, new Vector2(0.22f, 0.27f), new Vector2(0.78f, 0.34f), () => AddStatPoint("foco"), () => RemoveStatPoint("foco"));
            CreateStatRow(frame, "Empatia", _draftStats.empatiaAnimal, new Vector2(0.22f, 0.2f), new Vector2(0.78f, 0.27f), () => AddStatPoint("empatia"), () => RemoveStatPoint("empatia"));
            CreateStatRow(frame, "Sorte", _draftStats.sorte, new Vector2(0.22f, 0.13f), new Vector2(0.78f, 0.2f), () => AddStatPoint("sorte"), () => RemoveStatPoint("sorte"));
            CreateStatRow(frame, "Arcano", _draftStats.conhecimentoArcano, new Vector2(0.22f, 0.06f), new Vector2(0.78f, 0.13f), () => AddStatPoint("arcano"), () => RemoveStatPoint("arcano"));

            Button create = CreateButton(remainingPoints == 0 ? "Comecar Jornada" : "Distribua os 5 pontos", frame, remainingPoints == 0 ? _gold : new Color(0.45f, 0.42f, 0.34f));
            SetAnchors(create.GetComponent<RectTransform>(), new Vector2(0.34f, 0.005f), new Vector2(0.68f, 0.055f));
            create.onClick.AddListener(CreateProfileAndStart);

            Button back = CreateButton("Voltar", frame, _green);
            SetAnchors(back.GetComponent<RectTransform>(), new Vector2(0.04f, 0.005f), new Vector2(0.18f, 0.055f));
            back.onClick.AddListener(ShowHome);
        }

        private void SelectBody(CharacterBodyType bodyType)
        {
            _draftName = _nameInput != null ? _nameInput.text : _draftName;
            _selectedBody = bodyType;
            ShowCharacterCreation();
        }

        private void CreateStatRow(Transform parent, string label, int value, Vector2 min, Vector2 max, Action add, Action remove)
        {
            RectTransform row = CreatePanel("Stat_" + label, parent, new Color(0.28f, 0.2f, 0.12f, 0.35f));
            SetAnchors(row, min, max);

            Text text = CreateText($"{label}: {value}", row, 18, FontStyle.Bold, _ink);
            text.alignment = TextAnchor.MiddleLeft;
            SetAnchors(text.rectTransform, new Vector2(0.05f, 0f), new Vector2(0.58f, 1f));

            Button minus = CreateButton("-", row, _green);
            SetAnchors(minus.GetComponent<RectTransform>(), new Vector2(0.62f, 0.12f), new Vector2(0.76f, 0.88f));
            minus.onClick.AddListener(() => remove());

            Button plus = CreateButton("+", row, _gold);
            SetAnchors(plus.GetComponent<RectTransform>(), new Vector2(0.82f, 0.12f), new Vector2(0.96f, 0.88f));
            plus.onClick.AddListener(() => add());
        }

        private void AddStatPoint(string stat)
        {
            _draftName = _nameInput != null ? _nameInput.text : _draftName;
            if (_draftStats.Total >= CreationStatPoints) return;

            switch (stat)
            {
                case "vigor":
                    _draftStats.vigor++;
                    break;
                case "foco":
                    _draftStats.foco++;
                    break;
                case "empatia":
                    _draftStats.empatiaAnimal++;
                    break;
                case "sorte":
                    _draftStats.sorte++;
                    break;
                case "arcano":
                    _draftStats.conhecimentoArcano++;
                    break;
            }

            ShowCharacterCreation();
        }

        private void RemoveStatPoint(string stat)
        {
            _draftName = _nameInput != null ? _nameInput.text : _draftName;

            switch (stat)
            {
                case "vigor":
                    if (_draftStats.vigor > 0) _draftStats.vigor--;
                    break;
                case "foco":
                    if (_draftStats.foco > 0) _draftStats.foco--;
                    break;
                case "empatia":
                    if (_draftStats.empatiaAnimal > 0) _draftStats.empatiaAnimal--;
                    break;
                case "sorte":
                    if (_draftStats.sorte > 0) _draftStats.sorte--;
                    break;
                case "arcano":
                    if (_draftStats.conhecimentoArcano > 0) _draftStats.conhecimentoArcano--;
                    break;
            }

            ShowCharacterCreation();
        }

        private void CreateProfileAndStart()
        {
            if (_draftStats.Total != CreationStatPoints)
            {
                ShowCharacterCreation();
                return;
            }

            string playerName = _nameInput != null ? _nameInput.text : string.Empty;
            PlayerProfileService.Instance.CreateProfile(playerName, _selectedBody, _draftStats);
            StartGame();
        }

        private void StartGame()
        {
            PlayerProfileService.Instance.TouchLastPlayed();

            if (CaptureSession.Instance != null && GameManager.Instance != null)
            {
                GameManager.Instance.ChangeState(new CapturingState());
            }

            ShowCaptureHud();
        }

        private void ShowCaptureHud()
        {
            ClearRoot();
            _showingCapture = true;

            RectTransform topBar = CreatePanel("CaptureTopBar", _root, _panel);
            SetAnchors(topBar, new Vector2(0f, 0.82f), new Vector2(1f, 1f));

            _timerText = CreateText("Tempo: --", topBar, 22, FontStyle.Bold, _gold);
            SetAnchors(_timerText.rectTransform, new Vector2(0.04f, 0.2f), new Vector2(0.28f, 0.8f));

            _energyText = CreateText("Energia: --", topBar, 22, FontStyle.Bold, _gold);
            SetAnchors(_energyText.rectTransform, new Vector2(0.38f, 0.2f), new Vector2(0.62f, 0.8f));

            _captureCountText = CreateText("Capturados: 0", topBar, 22, FontStyle.Bold, _gold);
            SetAnchors(_captureCountText.rectTransform, new Vector2(0.72f, 0.2f), new Vector2(0.96f, 0.8f));

            Text title = CreateText("Captura", _root, 44, FontStyle.Bold, _gold);
            SetAnchors(title.rectTransform, new Vector2(0.25f, 0.64f), new Vector2(0.75f, 0.75f));

            _creatureText = CreateText("Criatura detectada", _root, 24, FontStyle.Normal, _paper);
            SetAnchors(_creatureText.rectTransform, new Vector2(0.22f, 0.53f), new Vector2(0.78f, 0.61f));

            _resistanceText = CreateText("Resistencia: --", _root, 22, FontStyle.Bold, _paper);
            SetAnchors(_resistanceText.rectTransform, new Vector2(0.22f, 0.43f), new Vector2(0.78f, 0.51f));

            Button captureButton = CreateButton("Capturar", _root, _gold);
            SetAnchors(captureButton.GetComponent<RectTransform>(), new Vector2(0.35f, 0.23f), new Vector2(0.65f, 0.37f));
            captureButton.onClick.AddListener(() => CaptureSession.Instance?.TratarCliqueDeCaptura());

            UpdateCaptureHud();
        }

        private void UpdateCaptureHud()
        {
            if (CaptureSession.Instance == null) return;

            CaptureSession session = CaptureSession.Instance;
            _timerText.text = $"Tempo: {Mathf.CeilToInt(session.TempoRestanteAtual)}s";
            _energyText.text = $"Energia: {Mathf.CeilToInt(session.EnergiaCapturaAtual)}";
            _captureCountText.text = $"Capturados: {session.CapturadosNaSessao}";

            CreatureInstance creature = session.CriaturaAtual;
            _creatureText.text = creature == null
                ? "Nenhuma criatura ativa"
                : $"{creature.ID_Especie} | {creature.Elemento} | {creature.Raridade}";

            int resistance = Mathf.CeilToInt(session.ProgressoResistenciaAtual * 100f);
            _resistanceText.text = session.SessaoAtiva ? $"Resistencia: {resistance}%" : "Sessao finalizada";
        }

        private RectTransform CreatePanel(string name, Transform parent, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            Image image = go.AddComponent<Image>();
            image.color = color;
            return go.GetComponent<RectTransform>();
        }

        private Text CreateText(string value, Transform parent, int fontSize, FontStyle fontStyle, Color color)
        {
            GameObject go = new GameObject("Text");
            go.transform.SetParent(parent, false);
            Text text = go.AddComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.color = color;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private Button CreateButton(string label, Transform parent, Color color)
        {
            RectTransform buttonRect = CreatePanel("Button_" + label, parent, color);
            Button button = buttonRect.gameObject.AddComponent<Button>();
            button.targetGraphic = buttonRect.GetComponent<Image>();
            Text text = CreateText(label, buttonRect, 20, FontStyle.Bold, _ink);
            Stretch(text.rectTransform);
            return button;
        }

        private InputField CreateInput(Transform parent, string placeholder)
        {
            RectTransform inputRect = CreatePanel("Input_Name", parent, Color.white);
            InputField input = inputRect.gameObject.AddComponent<InputField>();
            input.targetGraphic = inputRect.GetComponent<Image>();

            Text text = CreateText(string.Empty, inputRect, 18, FontStyle.Normal, _ink);
            text.alignment = TextAnchor.MiddleLeft;
            SetAnchors(text.rectTransform, new Vector2(0.04f, 0f), new Vector2(0.96f, 1f));

            Text placeholderText = CreateText(placeholder, inputRect, 18, FontStyle.Italic, new Color(0.35f, 0.3f, 0.24f));
            placeholderText.alignment = TextAnchor.MiddleLeft;
            SetAnchors(placeholderText.rectTransform, new Vector2(0.04f, 0f), new Vector2(0.96f, 1f));

            input.textComponent = text;
            input.placeholder = placeholderText;
            return input;
        }

        private void Stretch(RectTransform rect)
        {
            SetAnchors(rect, Vector2.zero, Vector2.one);
        }

        private void SetAnchors(RectTransform rect, Vector2 min, Vector2 max)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
