using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using Redcode.Extensions;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityRandom = UnityEngine.Random;

public enum CharacterPosition
{
    Left,
    Right
}

public enum CharacterNames
{
    Sandman,
    Boogeyman,
    Trainer
}

public record Dialogue(CharacterNames Name, string Msg, CharacterPosition CharacterPosition)
{
    public CharacterNames Name { get; } = Name;
    public string Msg { get; } = Msg;
    public CharacterPosition CharacterPosition { get; } = CharacterPosition;
}

[Serializable]
public class CharacterPortrait
{
    [SerializeField] Image[] images;
    public Image[] Images => images;
    private int currentIndex;
    public Image CurrentImage => images[currentIndex];
    public RectTransform RectTransform => CurrentImage.rectTransform;
    public Vector2 HidePosition { get; private set; }
    public Vector2 ShowPosition { get; private set; }

    public void Initialize(CharacterPosition pos)
    {
        ShowPosition = RectTransform.anchoredPosition;
        var sign = pos == CharacterPosition.Left ? -1 : 1;
        var rectTransform = RectTransform;
        var width = rectTransform.sizeDelta.x;
        var hidePosition = new Vector2(rectTransform.anchoredPosition.x + sign * width, rectTransform.anchoredPosition.y);
        HidePosition = hidePosition;
    }
    
    public void Switch()
    {
        currentIndex++;
        if (currentIndex >= images.Length)
            currentIndex = 0;
    }
}

public class VNManager : PersistentMonoSingleton<VNManager>
{
    [SerializeField] private GraphicRaycaster VNraphicRaycaster;
    [SerializeField] private CanvasGroup vnPanel;
    [SerializedDictionary("Position", "Images")]
    public SerializedDictionary<CharacterPosition, CharacterPortrait> characterPortraits;
    [SerializeField] private Image background;
    [SerializeField] private ChatBubbleUI chatBubbleUIPrefab;
    [SerializeField] private ScrollRect chatScrollRect;
    [SerializeField] private LayoutGroup contentLayoutGroup;
    [SerializeField] private Button historyButton;
    [SerializeField] private Button skipButton;
    [FormerlySerializedAs("clickable")] [SerializeField] private ClickableArea clickableArea;
    [SerializedDictionary("Name", "Sprite")]
    public SerializedDictionary<CharacterNames, Sprite> characterSprites;

    public delegate void VNFinished(VNPathSO vnPathSO);

    public static event VNFinished OnVNFinished;

    private VNPathSO currentVNPath;
    private readonly List<Dialogue> dialogues = new();
    private readonly List<ChatBubbleUI> chatBubbles = new();

    private Dictionary<CharacterPosition, Dialogue> lastDialogue = new()
    {
        { CharacterPosition.Left, null },
        { CharacterPosition.Right, null }
    };
    private int currentDialogueIndex;
    private Color originalBackgroundColor;
    private Image scrollRectImage;
    private Tween panelFadeTween;
    private Tween fadeTween;
    private Tween chatBubbleTween;
    private Tween delayTween;
    private Sequence characterPortraitSequence;
    private float originalAlpha;
    private bool showingHistory;
    private bool sceneLoaded;
    private bool firstChatAdded;
    public bool Playing { get; private set; }

    private List<GraphicRaycaster> graphicRaycasters = new();

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += (scene, mode) => OnSceneLoaded();
        clickableArea.OnClickEvent += NextChatBubble;
        originalBackgroundColor = background.color;
    }

    private void Start()
    {
        historyButton.onClick.AddListener(() => ToggleHistory(!showingHistory));
        skipButton.onClick.AddListener(CloseVN);
        scrollRectImage = chatScrollRect.GetComponent<Image>();
        chatScrollRect.vertical = false;
        originalAlpha = scrollRectImage.color.a;
        scrollRectImage.color = new Color(1f, 1f, 1f, 0f);
        foreach (var pos in characterPortraits.Keys)
        {
            characterPortraits[pos].Initialize(pos);
        }
        ResetPortrait();
        vnPanel.gameObject.SetActive(false);
    }

    private void OnSceneLoaded()
    {
        sceneLoaded = true;
        if (Playing && !firstChatAdded)
        {
            delayTween =
                DOVirtual.DelayedCall(1f, () => StartCoroutine(AddChatBubble(dialogues[currentDialogueIndex])));
            firstChatAdded = true;
        }
    }

    public void NextChatBubble()
    {
        if (panelFadeTween.IsActive()) return;
        if (showingHistory) return;
        if (chatBubbleTween.IsActive()) return;
        if (!firstChatAdded) return;
        if (currentDialogueIndex >= dialogues.Count)
        {
            CloseVN();
            return;
        }

        StartCoroutine(AddChatBubble(dialogues[currentDialogueIndex]));
    }

    public IEnumerator AddChatBubble(Dialogue dialogue)
    {
        var chatBubble = Instantiate(chatBubbleUIPrefab, chatScrollRect.content);
        contentLayoutGroup.childAlignment = dialogue.CharacterPosition == CharacterPosition.Left
            ? TextAnchor.UpperRight
            : TextAnchor.UpperLeft;
        chatBubble.Setup(dialogue);
        chatBubbles.Add(chatBubble);
        chatBubble.transform.localScale = Vector3.zero;
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatBubble.transform as RectTransform);
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatBubble.transform as RectTransform);
        chatBubbleTween = chatBubble.transform.DOScale(Vector3.one, 0.2f);
        HandleCharacterPortrait(dialogue);
        currentDialogueIndex++;
        //GlobalSoundManager.Instance.PlayBubbleSFX();
        FadeBubble();
        yield return null;
    }

    private void HandleCharacterPortrait(Dialogue dialogue)
    {
        if (characterPortraitSequence.IsActive()) characterPortraitSequence.Kill(true);
        var gray = new Color(0.5f, 0.5f, 0.5f);
        var white = Color.white;
        var otherPos = dialogue.CharacterPosition == CharacterPosition.Left ? CharacterPosition.Right : CharacterPosition.Left;
        var sequence = DOTween.Sequence();
        var currentPortrait = characterPortraits[dialogue.CharacterPosition];
        //Slide in
        if (lastDialogue[dialogue.CharacterPosition] == null)
        {
            currentPortrait.CurrentImage.sprite = characterSprites[dialogue.Name];
            sequence.Append(currentPortrait.RectTransform.DOAnchorPos(currentPortrait.ShowPosition, 0.25f));
            sequence.Join(currentPortrait.CurrentImage.DOFade(1f, 0.2f));
        }
        //Crossfade
        else if (lastDialogue[dialogue.CharacterPosition].Name != dialogue.Name)
        {
            sequence.Append(currentPortrait.RectTransform.DOAnchorPos(currentPortrait.HidePosition, 0.25f));
            sequence.Join(currentPortrait.CurrentImage.DOColor(gray, 0.25f));
            sequence.Join(currentPortrait.CurrentImage.DOFade(0f, 0.25f));
            currentPortrait.Switch();
            currentPortrait.CurrentImage.sprite = characterSprites[dialogue.Name];
            sequence.Join(currentPortrait.RectTransform.DOAnchorPos(currentPortrait.ShowPosition, 0.25f));
            sequence.Join(currentPortrait.CurrentImage.DOFade(1f, 0.25f));
        }
        //Change Color
        sequence.Join(currentPortrait.CurrentImage.DOColor(white, 0.25f));
        sequence.Join(characterPortraits[otherPos].CurrentImage.DOColor(gray, 0.25f));
        characterPortraitSequence = sequence;
        lastDialogue[dialogue.CharacterPosition] = dialogue;
    }

    private void ResetPortrait()
    {
        var gray = new Color(0.5f, 0.5f, 0.5f);
        foreach (var pos in characterPortraits.Keys)
        {
            foreach (var img in characterPortraits[pos].Images)
            {
                var sequence = DOTween.Sequence();
                var rectTransform = img.rectTransform;
                sequence.Join(rectTransform.DOAnchorPos(characterPortraits[pos].HidePosition, 0.2f));
                sequence.Join(img.DOColor(gray, 0.2f));
                sequence.Join(img.DOFade(0f, 0.2f));
                sequence.OnComplete(() =>
                {
                    img.sprite = null;
                });
            }
        }
    }

    private void FadeBubble()
    {
        for (var i = chatBubbles.Count - 1; i >= 0; i--)
        {
            if (i == chatBubbles.Count - 2) chatBubbles[i].SetAlpha(0.5f);
            if (i < chatBubbles.Count - 2) chatBubbles[i].SetAlpha(0f);
        }
    }

    private void ToggleHistory(bool active)
    {
        if (panelFadeTween.IsActive()) return;
        showingHistory = active;
        if (!active)
        {
            chatScrollRect.verticalNormalizedPosition = 1;
            chatScrollRect.vertical = false;
            clickableArea.gameObject.SetActive(true);
            if (fadeTween.IsActive())
                fadeTween.Kill();
            fadeTween = scrollRectImage.DOFade(0f, 0.2f);
            FadeBubble();
        }
        else
        {
            chatScrollRect.vertical = true;
            clickableArea.gameObject.SetActive(false);
            if (fadeTween.IsActive())
                fadeTween.Kill();
            fadeTween = scrollRectImage.DOFade(originalAlpha, 0.2f);
            foreach (var bubble in chatBubbles) bubble.SetAlpha(1f);
        }
    }

    public void ShowVN(VNPathSO vnPathSO)
    {
        if (panelFadeTween.IsActive()) return;
        OnFailToLoadDialogueFile += () => Debug.LogError("Failed to load dialogue file");
        OnSuccessToLoadDialogueFile += OnSuccess;
        StartCoroutine(LoadDialogueFile(vnPathSO));
    }

    private void OnSuccess()
    {
        vnPanel.gameObject.SetActive(true);
        vnPanel.alpha = 0;
        ResetPortrait();
        panelFadeTween = vnPanel.DOFade(1f, 0.2f);
        if (currentVNPath.Background)
        {
            background.sprite = currentVNPath.Background;
            background.color = Color.white;
        }

        Playing = true;
        if (sceneLoaded && !firstChatAdded)
        {
            delayTween =
                DOVirtual.DelayedCall(1f, () => StartCoroutine(AddChatBubble(dialogues[currentDialogueIndex])));
            firstChatAdded = true;
        }

        //Find all graphics raycaster and disable them except the VN graphic raycaster
        graphicRaycasters = FindObjectsOfType<GraphicRaycaster>().ToList();
        foreach (var graphicRaycaster in graphicRaycasters)
            graphicRaycaster.enabled = graphicRaycaster == VNraphicRaycaster;
    }

    private delegate void FailToLoadDialogueFile();

    private static event FailToLoadDialogueFile OnFailToLoadDialogueFile;

    private delegate void SuccessToLoadDialogueFile();

    private static event SuccessToLoadDialogueFile OnSuccessToLoadDialogueFile;

    private IEnumerator LoadDialogueFile(VNPathSO vnPathSO)
    {
        if (vnPathSO.TextAsset == null)
        {
            OnFailToLoadDialogueFile?.Invoke();
            yield break;
        }

        var lines = vnPathSO.TextAsset.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        dialogues.Clear();
        if (lines.Length == 0)
        {
            OnFailToLoadDialogueFile?.Invoke();
            yield break;
        }

        foreach (var line in lines)
        {
            var pattern = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";
            var values = Regex.Split(line, pattern);
            values = values.Select(x => x.Trim('"')).ToArray();
            if (!Enum.TryParse(values[0], out CharacterNames characterName)) continue;
            if (!Enum.TryParse(values[2], out CharacterPosition characterPosition)) continue;
            var dialogue = new Dialogue(characterName, values[1], characterPosition);
            dialogues.Add(dialogue);
            yield return null;
        }

        currentVNPath = vnPathSO;
        OnSuccessToLoadDialogueFile?.Invoke();
        yield return null;
    }

    public void CloseVN()
    {
        if (panelFadeTween.IsActive()) return;
        if (delayTween.IsActive())
            delayTween.Kill();
        foreach (var bubble in chatBubbles) 
            Destroy(bubble.gameObject);
        foreach (var last in lastDialogue.Keys.ToList())
            lastDialogue[last] = null;
        ResetPortrait();
        chatBubbles.Clear();
        currentDialogueIndex = 0;
        firstChatAdded = false;
        Playing = false;
        panelFadeTween = vnPanel.DOFade(0f, 0.2f).OnComplete(() => vnPanel.gameObject.SetActive(false));
        foreach (var graphicRaycaster in graphicRaycasters)
            if (graphicRaycaster)
                graphicRaycaster.enabled = true;
        background.sprite = null;
        background.color = originalBackgroundColor;
        graphicRaycasters.Clear();
        OnVNFinished?.Invoke(currentVNPath);
    }
}