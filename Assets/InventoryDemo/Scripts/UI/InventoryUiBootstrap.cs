using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace InventoryDemo.UI
{
    public sealed class InventoryUiBootstrap : MonoBehaviour
    {
        private static readonly Color ScreenDim = new(0.035f, 0.045f, 0.05f, 0.72f);
        private static readonly Color PanelColor = new(0.105f, 0.125f, 0.13f, 0.98f);
        private static readonly Color HeaderColor = new(0.145f, 0.18f, 0.17f, 1f);
        private static readonly Color FieldColor = new(0.075f, 0.09f, 0.095f, 1f);
        private static readonly Color SlotColor = new(0.155f, 0.175f, 0.18f, 1f);
        private static readonly Color SlotBorderColor = new(0.29f, 0.33f, 0.32f, 1f);
        private static readonly Color AccentColor = new(0.42f, 0.68f, 0.55f, 1f);
        private static readonly Color DangerColor = new(0.72f, 0.31f, 0.29f, 1f);
        private static readonly Color PrimaryText = new(0.94f, 0.95f, 0.93f, 1f);
        private static readonly Color SecondaryText = new(0.65f, 0.69f, 0.67f, 1f);

        private Font uiFont;
        private bool isBuilt;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateAfterSceneLoad()
        {
            CreateForCurrentScene();
        }

        public static InventoryUiBootstrap CreateForCurrentScene()
        {
            InventoryUiBootstrap existing = FindFirstObjectByType<InventoryUiBootstrap>();
            if (existing != null)
            {
                existing.BuildIfNeeded();
                return existing;
            }

            GameObject root = new("Inventory UI Bootstrap");
            InventoryUiBootstrap bootstrap = root.AddComponent<InventoryUiBootstrap>();
            bootstrap.BuildIfNeeded();
            return bootstrap;
        }

        private void Awake()
        {
            BuildIfNeeded();
        }

        public void BuildIfNeeded()
        {
            if (isBuilt)
            {
                return;
            }

            isBuilt = true;
            uiFont = Font.CreateDynamicFontFromOSFont(
                new[] { "Microsoft YaHei", "SimHei", "Arial" },
                20);

            EnsureEventSystem();
            BuildInventoryUi();
        }

        private void EnsureEventSystem()
        {
            if (EventSystem.current != null)
            {
                return;
            }

            GameObject eventSystemObject = new("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            eventSystemObject.transform.SetParent(transform);
        }

        private void BuildInventoryUi()
        {
            GameObject canvasObject = CreateUiObject("Inventory Canvas", transform);
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            canvasObject.AddComponent<GraphicRaycaster>();

            RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
            Stretch(canvasRect);

            InventoryWindowView windowView = gameObject.AddComponent<InventoryWindowView>();
            GameObject window = CreatePanel("Inventory Window", canvasRect, ScreenDim);
            Stretch(window.GetComponent<RectTransform>());

            RectTransform panel = CreatePanel("Panel", window.transform, PanelColor).GetComponent<RectTransform>();
            panel.anchorMin = new Vector2(0.5f, 0.5f);
            panel.anchorMax = new Vector2(0.5f, 0.5f);
            panel.pivot = new Vector2(0.5f, 0.5f);
            panel.sizeDelta = new Vector2(760f, 620f);
            panel.anchoredPosition = Vector2.zero;

            BuildHeader(panel);
            BuildSearchArea(panel);
            BuildGrid(panel, windowView);
            RectTransform contextMenu = BuildContextMenu(window.transform);

            windowView.Initialize(window, contextMenu, canvasRect, canvas);
        }

        private void BuildHeader(RectTransform panel)
        {
            RectTransform header = CreatePanel("Header", panel, HeaderColor).GetComponent<RectTransform>();
            SetTopRect(header, 0f, 64f, 0f, 0f);

            Text title = CreateText("Title", header, "背包", 28, FontStyle.Bold, PrimaryText);
            SetRect(title.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(28f, 0f), new Vector2(-28f, 0f));
            title.alignment = TextAnchor.MiddleLeft;
        }

        private void BuildSearchArea(RectTransform panel)
        {
            RectTransform searchArea = CreateUiObject("Search Area", panel).GetComponent<RectTransform>();
            SetTopRect(searchArea, 82f, 48f, 28f, 28f);

            RectTransform fieldRect = CreatePanel("Search Field", searchArea, FieldColor).GetComponent<RectTransform>();
            fieldRect.anchorMin = new Vector2(0f, 0f);
            fieldRect.anchorMax = new Vector2(1f, 1f);
            fieldRect.offsetMin = Vector2.zero;
            fieldRect.offsetMax = new Vector2(-118f, 0f);

            InputField input = fieldRect.gameObject.AddComponent<InputField>();
            input.targetGraphic = fieldRect.GetComponent<Image>();

            Text inputText = CreateText("Text", fieldRect, string.Empty, 18, FontStyle.Normal, PrimaryText);
            SetRect(inputText.rectTransform, Vector2.zero, Vector2.one, new Vector2(16f, 5f), new Vector2(-16f, -5f));
            inputText.alignment = TextAnchor.MiddleLeft;
            inputText.supportRichText = false;

            Text placeholder = CreateText("Placeholder", fieldRect, "名称、类型或标签", 18, FontStyle.Normal, SecondaryText);
            SetRect(placeholder.rectTransform, Vector2.zero, Vector2.one, new Vector2(16f, 5f), new Vector2(-16f, -5f));
            placeholder.alignment = TextAnchor.MiddleLeft;
            placeholder.fontStyle = FontStyle.Italic;

            input.textComponent = inputText;
            input.placeholder = placeholder;

            Button searchButton = CreateButton("Search Button", searchArea, "搜索", AccentColor, PrimaryText);
            RectTransform buttonRect = searchButton.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(1f, 0f);
            buttonRect.anchorMax = new Vector2(1f, 1f);
            buttonRect.pivot = new Vector2(1f, 0.5f);
            buttonRect.sizeDelta = new Vector2(102f, 0f);
            buttonRect.anchoredPosition = Vector2.zero;
        }

        private void BuildGrid(RectTransform panel, InventoryWindowView windowView)
        {
            RectTransform gridFrame = CreatePanel("Grid Frame", panel, FieldColor).GetComponent<RectTransform>();
            gridFrame.anchorMin = new Vector2(0.5f, 0f);
            gridFrame.anchorMax = new Vector2(0.5f, 0f);
            gridFrame.pivot = new Vector2(0.5f, 0f);
            gridFrame.sizeDelta = new Vector2(620f, 440f);
            gridFrame.anchoredPosition = new Vector2(0f, 28f);

            RectTransform grid = CreateUiObject("Slot Grid", gridFrame).GetComponent<RectTransform>();
            Stretch(grid, 26f);

            GridLayoutGroup layout = grid.gameObject.AddComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(84f, 68f);
            layout.spacing = new Vector2(12f, 12f);
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = 6;
            layout.childAlignment = TextAnchor.MiddleCenter;

            for (int index = 0; index < 30; index++)
            {
                GameObject slot = CreatePanel($"Slot {index + 1:00}", grid, SlotColor);
                Outline outline = slot.AddComponent<Outline>();
                outline.effectColor = SlotBorderColor;
                outline.effectDistance = new Vector2(1f, -1f);

                InventorySlotView slotView = slot.AddComponent<InventorySlotView>();
                slotView.Initialize(windowView);
            }
        }

        private RectTransform BuildContextMenu(Transform window)
        {
            RectTransform menu = CreatePanel("Context Menu", window, HeaderColor).GetComponent<RectTransform>();
            menu.anchorMin = new Vector2(0.5f, 0.5f);
            menu.anchorMax = new Vector2(0.5f, 0.5f);
            menu.pivot = new Vector2(0.5f, 0.5f);
            menu.sizeDelta = new Vector2(148f, 144f);

            VerticalLayoutGroup layout = menu.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(6, 6, 6, 6);
            layout.spacing = 4f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateButton("Use", menu, "使用", SlotColor, PrimaryText);
            CreateButton("Split", menu, "拆分", SlotColor, PrimaryText);
            CreateButton("Discard", menu, "丢弃", DangerColor, PrimaryText);

            menu.gameObject.SetActive(false);
            return menu;
        }

        private Button CreateButton(string name, Transform parent, string label, Color background, Color textColor)
        {
            GameObject buttonObject = CreatePanel(name, parent, background);
            Button button = buttonObject.AddComponent<Button>();
            button.targetGraphic = buttonObject.GetComponent<Image>();

            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.12f, 1.12f, 1.12f, 1f);
            colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;

            Text text = CreateText("Label", buttonObject.transform, label, 18, FontStyle.Bold, textColor);
            Stretch(text.rectTransform);
            text.alignment = TextAnchor.MiddleCenter;
            return button;
        }

        private Text CreateText(
            string name,
            Transform parent,
            string content,
            int fontSize,
            FontStyle fontStyle,
            Color color)
        {
            GameObject textObject = CreateUiObject(name, parent);
            Text text = textObject.AddComponent<Text>();
            text.font = uiFont;
            text.text = content;
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.color = color;
            text.raycastTarget = false;
            return text;
        }

        private static GameObject CreatePanel(string name, Transform parent, Color color)
        {
            GameObject panel = CreateUiObject(name, parent);
            Image image = panel.AddComponent<Image>();
            image.color = color;
            return panel;
        }

        private static GameObject CreateUiObject(string name, Transform parent)
        {
            GameObject child = new(name, typeof(RectTransform));
            child.transform.SetParent(parent, false);
            return child;
        }

        private static void Stretch(RectTransform rect, float inset = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(inset, inset);
            rect.offsetMax = new Vector2(-inset, -inset);
        }

        private static void SetTopRect(
            RectTransform rect,
            float top,
            float height,
            float left,
            float right)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.offsetMin = new Vector2(left, -top - height);
            rect.offsetMax = new Vector2(-right, -top);
        }

        private static void SetRect(
            RectTransform rect,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 offsetMin,
            Vector2 offsetMax)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }
    }
}
