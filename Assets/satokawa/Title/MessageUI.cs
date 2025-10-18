using UnityEngine;
using TMPro;
public class MessageUI : MonoBehaviour
{
    [SerializeField] private GameObject _messagePanel;
    [SerializeField] private TextMeshProUGUI _messageText;
    public void ShowMessage(string message)
    {
        _messageText.text = message;
        _messagePanel.SetActive(true);
    }
    public void Clause()
    {
        _messagePanel.SetActive(false);
    }
}