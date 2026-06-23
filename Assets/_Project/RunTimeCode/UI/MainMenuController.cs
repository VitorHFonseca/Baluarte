using GameCore.Data;
using GameCore.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameCore.UI
{
    public class MainMenuController : MonoBehaviour
    {
        private const string GameVersion = "0.0.0.3";

        private readonly Color _parchment = new Color(0.78f, 0.68f, 0.48f);
        private readonly Color _ink = new Color(0.12f, 0.08f, 0.04f);
        private readonly Color _darkWood = new Color(0.16f, 0.09f, 0.04f);
        private readonly Color _gold = new Color(0.92f, 0.66f, 0.22f);

        private Canvas _canvas;
        private RectTransform _root;
        private InputField _nameInput;
        private CharacterBodyType _selectedBody = CharacterBodyType.Homem;
        private string _draftName = string.Empty;

        private void Start()
        {
            EnsureRuntimeObjects();
            ShowHome();
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

            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }

            if (_canvas != null) return;

            GameObject canvasObject = new GameObject("Canvas_MainMenu");
            _canvas = canvasObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObject.AddComponent<GraphicRaycaster>();

            _root = CreatePanel("Root", canvasObject.transform, _darkWood);
            _root.anchorMin = Vector2.zero;
            _root.anchorMax = Vector2.one;
            _root.offsetMin = Vector2.zero;
            _root.offsetMax = Vector2.zero;
        }

        private void ClearRoot()
        {
            for (int i = _root.childCount - 1; i >= 0; i--)
            {
                Destroy(_root.GetChild(i).gameObject);
            }
        }

        private void ShowHome()
        {
            ClearRoot();

            RectTransform frame = CreatePanel("HomeFrame", _root, _parchment);
            SetAnchors(frame, new Vector2(0.12f, 0.1f), new Vector2(0.88f, 0.9f));

            Text title = CreateText("Baluarte", frame, 52, FontStyle.Bold, _ink);
            SetAnchors(title.rectTransform, new Vector2(0.08f, 0.78f), new Vector2(0.92f, 0.94f));

            Text subtitle = CreateText("Bestiario, captura e defesa em um reino medieval de fantasia", frame, 18, FontStyle.Italic, _ink);
            SetAnchors(subtitle.rectTransform, new Vector2(0.1f, 0.7f), new Vector2(0.9f, 0.78f));

            PlayerProfile profile = PlayerProfileService.Instance.CurrentProfile;
            string buttonText = profile == null ? "Criar Personagem" : "Iniciar Jornada";
            Button startButton = CreateButton(buttonText, frame, _gold);
            SetAnchors(startButton.GetComponent<RectTransform>(), new Vector2(0.34f, 0.53f), new Vector2(0.66f, 0.64f));
            startButton.onClick.AddListener(() =>
            {
                if (PlayerProfileService.Instance.HasProfile) StartGame();
                else ShowCharacterCreation();
            });

            if (profile != null)
            {
                Text profileText = CreateText(
                    $"{profile.nome} | {profile.corpo} | Vigor {profile.stats.vigor}  Foco {profile.stats.foco}  Empatia {profile.stats.empatiaAnimal}",
                    frame,
                    16,
                    FontStyle.Normal,
                    _ink);
                SetAnchors(profileText.rectTransform, new Vector2(0.12f, 0.44f), new Vector2(0.88f, 0.5f));
            }

            RectTransform updates = CreatePanel("Updates", frame, new Color(0.2f, 0.12f, 0.06f, 0.92f));
            SetAnchors(updates, new Vector2(0.12f, 0.08f), new Vector2(0.88f, 0.34f));

            Text updateText = CreateText(
                "Atualizacoes - v" + GameVersion + "\n" +
                "- Tela inicial medieval adicionada.\n" +
                "- Criacao de personagem com homem/mulher padrao.\n" +
                "- Status base preparados para influenciar capturas futuras.",
                updates,
                16,
                FontStyle.Normal,
                _parchment);
            updateText.alignment = TextAnchor.MiddleLeft;
            SetAnchors(updateText.rectTransform, new Vector2(0.06f, 0.12f), new Vector2(0.94f, 0.88f));
        }

        private void ShowCharacterCreation()
        {
            ClearRoot();

            RectTransform frame = CreatePanel("CharacterCreationFrame", _root, _parchment);
            SetAnchors(frame, new Vector2(0.16f, 0.08f), new Vector2(0.84f, 0.92f));

            Text title = CreateText("Criacao de Personagem", frame, 36, FontStyle.Bold, _ink);
            SetAnchors(title.rectTransform, new Vector2(0.08f, 0.84f), new Vector2(0.92f, 0.94f));

            Text intro = CreateText("Escolha a base visual do seu aventureiro. Os status serao usados futuramente nas capturas.", frame, 16, FontStyle.Normal, _ink);
            SetAnchors(intro.rectTransform, new Vector2(0.12f, 0.75f), new Vector2(0.88f, 0.83f));

            _nameInput = CreateInput(frame, "Nome do personagem");
            _nameInput.text = _draftName;
            _nameInput.onValueChanged.AddListener(value => _draftName = value);
            SetAnchors(_nameInput.GetComponent<RectTransform>(), new Vector2(0.26f, 0.64f), new Vector2(0.74f, 0.72f));

            Button male = CreateButton("Homem", frame, _gold);
            SetAnchors(male.GetComponent<RectTransform>(), new Vector2(0.22f, 0.5f), new Vector2(0.46f, 0.6f));
            male.onClick.AddListener(() =>
            {
                _draftName = _nameInput != null ? _nameInput.text : _draftName;
                _selectedBody = CharacterBodyType.Homem;
                ShowCharacterCreation();
            });

            Button female = CreateButton("Mulher", frame, _gold);
            SetAnchors(female.GetComponent<RectTransform>(), new Vector2(0.54f, 0.5f), new Vector2(0.78f, 0.6f));
            female.onClick.AddListener(() =>
            {
                _draftName = _nameInput != null ? _nameInput.text : _draftName;
                _selectedBody = CharacterBodyType.Mulher;
                ShowCharacterCreation();
            });

            PlayerStats preview = PlayerStats.CreateDefault(_selectedBody);
            Text stats = CreateText(
                $"Selecionado: {_selectedBody}\n\n" +
                $"Vigor: {preview.vigor}\n" +
                $"Foco: {preview.foco}\n" +
                $"Empatia Animal: {preview.empatiaAnimal}\n" +
                $"Sorte: {preview.sorte}\n" +
                $"Conhecimento Arcano: {preview.conhecimentoArcano}",
                frame,
                18,
                FontStyle.Normal,
                _ink);
            stats.alignment = TextAnchor.MiddleLeft;
            SetAnchors(stats.rectTransform, new Vector2(0.28f, 0.24f), new Vector2(0.72f, 0.46f));

            Button create = CreateButton("Comecar Jornada", frame, _gold);
            SetAnchors(create.GetComponent<RectTransform>(), new Vector2(0.34f, 0.1f), new Vector2(0.66f, 0.2f));
            create.onClick.AddListener(CreateProfileAndStart);
        }

        private void CreateProfileAndStart()
        {
            string playerName = _nameInput != null ? _nameInput.text : string.Empty;
            PlayerProfileService.Instance.CreateProfile(playerName, _selectedBody);
            StartGame();
        }

        private void StartGame()
        {
            PlayerProfileService.Instance.TouchLastPlayed();
            ClearRoot();

            Text loading = CreateText("A jornada comeca...", _root, 34, FontStyle.Bold, _gold);
            SetAnchors(loading.rectTransform, new Vector2(0.2f, 0.4f), new Vector2(0.8f, 0.6f));

            if (CaptureSession.Instance != null)
            {
                GameManager.Instance.ChangeState(new CapturingState());
            }
            else
            {
                Debug.Log("Perfil pronto. Adicione CaptureSession na cena para iniciar a captura.");
            }
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
            SetAnchors(text.rectTransform, Vector2.zero, Vector2.one);
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

        private void SetAnchors(RectTransform rect, Vector2 min, Vector2 max)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
