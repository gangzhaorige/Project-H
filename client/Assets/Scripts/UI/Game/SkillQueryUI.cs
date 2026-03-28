using UnityEngine;
using ProjectH.Models;
using ProjectH.UI;

public class SkillQueryUI : MonoBehaviour
{
    public static SkillQueryUI Instance { get; private set; }

    [Header("UI References")]
    private SkillResponsePanelView _view;

    private int currentSkillId;

    public void Init(SkillResponsePanelView view)
    {
        _view = view;
        Hide();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

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
        GameSession.Instance.TriggerSkillQuery(res.SkillId);
        Show($"Do you want to activate '{res.SkillName}'?");
    }

    public void Show(string message)
    {
        if (_view != null)
        {
            _view.Show(message, () => OnResponse(true), () => OnResponse(false));
        }
    }

    public void Hide()
    {
        if (_view != null) _view.Hide();
    }

    private void OnResponse(bool accepted)
    {
        RequestSkillResponse req = new RequestSkillResponse();
        req.Send(currentSkillId, accepted);
        NetworkManager.Instance.SendRequest(req);
        
        GameSession.Instance.TriggerSkillQueryAnswered();
        Hide();
    }
}
