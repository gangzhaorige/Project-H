using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillQueryUI : MonoBehaviour
{
    public static SkillQueryUI Instance { get; private set; }

    [Header("UI References")]
    public GameObject panel;
    public TextMeshProUGUI queryText;
    public Button yesButton;
    public Button noButton;

    private int currentSkillId;
    private int currentTargetId; // If needed, although not used for request anymore

    private void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (yesButton != null) yesButton.onClick.AddListener(() => OnResponse(true));
        if (noButton != null) noButton.onClick.AddListener(() => OnResponse(false));

        Hide();
    }

    private void Start()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.AddCallback(Constants.SMSG_SKILL_QUERY, OnSkillQueryReceived);
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_SKILL_QUERY);
        }
    }

    private void OnSkillQueryReceived(ExtendedEventArgs args)
    {
        ResponseSkillQueryEventArgs res = args as ResponseSkillQueryEventArgs;
        if (res == null) return;

        // If this query isn't for us, ignore it
        if (res.PlayerId != Constants.USER_ID) return;

        currentSkillId = res.SkillId;
        Show($"Do you want to activate '{res.SkillName}'?");
    }

    public void Show(string message)
    {
        if (queryText != null) queryText.text = message;
        if (panel != null) panel.SetActive(true);
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
    }

    private void OnResponse(bool accepted)
    {
        if (accepted && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySkillSFX();
        }

        RequestSkillResponse req = new RequestSkillResponse();
        req.Send(accepted);
        NetworkManager.Instance.SendRequest(req);
        
        Hide();
    }
}
