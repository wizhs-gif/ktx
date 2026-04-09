using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Serializable]
    public class DialogueLine
    {
        public string speaker;
        public string content;

        public DialogueLine(string speaker, string content)
        {
            this.speaker = speaker;
            this.content = content;
        }
    }

    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Text speakerText;
    [SerializeField] private Text contentText;
    [SerializeField] private Text continueHintText;

    [Header("Input")]
    [SerializeField] private KeyCode nextKey = KeyCode.Space;

    [Header("Typewriter")]
    [SerializeField] private float charInterval = 0.03f;

    private readonly List<DialogueLine> lines = new List<DialogueLine>();
    private int currentLineIndex = -1;
    private Coroutine typingCoroutine;
    private string currentFullContent = "";
    private bool isTyping = false;
    private Action onDialogueFinished;

    public bool IsPlaying { get; private set; }

    private void Awake()
    {
        CloseImmediately();
    }

    private void Update()
    {
        if (!IsPlaying) return;

        if (Input.GetKeyDown(nextKey))
        {
            Advance();
        }
    }

    public void StartDialogue(TextAsset csvFile, string defaultSpeaker, Action finishCallback = null)
    {
        if (IsPlaying) return;
        if (csvFile == null) return;

        BuildLinesFromCsv(csvFile.text, defaultSpeaker);

        if (lines.Count == 0) return;

        onDialogueFinished = finishCallback;
        currentLineIndex = -1;
        IsPlaying = true;

        dialoguePanel.SetActive(true);

        if (continueHintText != null)
        {
            continueHintText.text = $"按 {nextKey} 继续";
        }

        ShowNextLine();
    }

    public void Advance()
    {
        if (!IsPlaying) return;

        if (isTyping)
        {
            CompleteTyping();
            return;
        }

        ShowNextLine();
    }

    private void ShowNextLine()
    {
        currentLineIndex++;

        if (currentLineIndex >= lines.Count)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = lines[currentLineIndex];

        if (speakerText != null)
            speakerText.text = line.speaker;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(line.content));
    }

    private IEnumerator TypeLine(string fullContent)
    {
        isTyping = true;
        currentFullContent = fullContent;
        contentText.text = "";

        foreach (char c in fullContent)
        {
            contentText.text += c;
            yield return new WaitForSeconds(charInterval);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    private void CompleteTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        contentText.text = currentFullContent;
        isTyping = false;
        typingCoroutine = null;
    }

    private void EndDialogue()
    {
        CloseImmediately();

        Action callback = onDialogueFinished;
        onDialogueFinished = null;
        callback?.Invoke();
    }

    public void CloseImmediately()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        lines.Clear();
        currentLineIndex = -1;
        currentFullContent = "";
        isTyping = false;
        typingCoroutine = null;
        IsPlaying = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (speakerText != null) speakerText.text = "";
        if (contentText != null) contentText.text = "";
        if (continueHintText != null) continueHintText.text = "";
    }

    private void BuildLinesFromCsv(string csvText, string defaultSpeaker)
    {
        lines.Clear();

        string normalized = csvText.Replace("\r\n", "\n").Replace("\r", "\n");
        string[] rows = normalized.Split('\n');

        for (int i = 0; i < rows.Length; i++)
        {
            string row = rows[i].Trim();
            if (string.IsNullOrEmpty(row)) continue;

            List<string> cols = ParseCsvRow(row);
            if (cols.Count < 2) continue;

            // 跳过表头
            if (i == 0 &&
                cols[0].Trim().Equals("speaker", StringComparison.OrdinalIgnoreCase) &&
                cols[1].Trim().Equals("content", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string speaker = cols[0].Trim();
            string content = cols[1].Trim();

            if (string.IsNullOrEmpty(speaker))
                speaker = defaultSpeaker;

            if (string.IsNullOrEmpty(content))
                continue;

            lines.Add(new DialogueLine(speaker, content));
        }
    }

    private List<string> ParseCsvRow(string row)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int i = 0; i < row.Length; i++)
        {
            char c = row[i];

            if (c == '"')
            {
                // 处理转义双引号 ""
                if (inQuotes && i + 1 < row.Length && row[i + 1] == '"')
                {
                    sb.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(sb.ToString());
                sb.Length = 0;
            }
            else
            {
                sb.Append(c);
            }
        }

        result.Add(sb.ToString());
        return result;
    }
}