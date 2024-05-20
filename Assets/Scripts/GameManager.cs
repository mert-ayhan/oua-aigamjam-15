using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GeminiAPI.Types;
using TMPro;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _chatText;
    [SerializeField] private TextMeshProUGUI _responseText;
    [SerializeField] private GameObject _chatPanel;
    [SerializeField] private GameObject _npc;
    [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;

    private static readonly string Context =
        "Bir post-apokaliptik evrende geçen bir oyun tasarlıyorum. " +
        "Bu dünyada para yerine zaman kullanılıyor ve herkesin bir zamanı var. Zamanları dolunca ölüyorlar. " +
        "İnsanlar çalışarak zaman kazanıyor ve hayatta kalmak için bu zamanı kullanıyor. " +
        "Oyuncu, bir tüccar olarak dükkanında zaman karşılığında yemek, su ve ilaç alıp satıyor." +
        "Müşteriler oyuncu ile pazarlık yapabilir. Bu müşterilerden birisi gibi davran." +
        "Unutma, müşteriler cebindeki son zamanı veremezler yoksa ölürler. Zamanları yaşamlarından kalan süreyi temsil ediyor." +
        "Diyalog sonunu kısa tut. Anlaştık veya anlaşamadık gibi kısa bir cümle yeterli olacaktır." +
        "En son oyuncuyla müşteri fiyatta anlaştığında veya anlaşamadığında yani diyalog bitince bana ouagamejam yazarak bildir." +
        "Diyalog sonunda oyuncuyla anlaşırsan kaça anlaştığını ve anlaşma tipini ouagamejam:tip:saat olarak yaz. Saat cinsinden istiyorum gün ise saate çevir. Örneğin ouagamejam:al:24 veya ouagamejam:sat:24";

    private readonly GenerateContentRequest initialRequest = new()
    {
        contents = new Content[]
        {
        },
        systemInstruction =
            new Content(Context)
    };

    private GenerateContentRequest currentRequest;
    private GeminiAPIClient _geminiAPIClient;
    private CharacterController _characterController;
    private Coroutine _typingCoroutine;
    private NPCSplineMover npcSplineMover;
    private bool waitingForResponse = true;

    void Start()
    {
        Debug.Log("GameManager Start method begins");

        _geminiAPIClient = GetComponent<GeminiAPIClient>();
        _characterController = GetComponent<CharacterController>();
        npcSplineMover = _npc.GetComponent<NPCSplineMover>();

        if (_geminiAPIClient == null) Debug.LogError("GeminiAPIClient is null");
        if (_characterController == null) Debug.LogError("CharacterController is null");
        if (npcSplineMover == null) Debug.LogError("NPCSplineMover is null");
        if (_chatText == null) Debug.LogError("ChatText is null");
        if (_responseText == null) Debug.LogError("ResponseText is null");
        if (_chatPanel == null) Debug.LogError("ChatPanel is null");

        _chatText.text = "...";
        currentRequest = initialRequest;
        SendMessageToAI("Merhaba, hoş geldiniz nasıl yardımcı olabilirim?");

        npcSplineMover.Move(false, () =>
        {
            Debug.Log("NPC movement complete");
            _chatPanel.SetActive(true);
            _cinemachineVirtualCamera.LookAt = _npc.transform;
            _cinemachineVirtualCamera.Priority = 1;
        });
    }

    public async void SendMessageToAI(string message)
    {
        if (string.IsNullOrEmpty(message) || !waitingForResponse)
            return;

        currentRequest.contents = currentRequest.contents.Append(new Content(Role.User, message)).ToArray();
        _responseText.text = "";
        _chatText.text = "...";

        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
        }

        _typingCoroutine = StartCoroutine(ShowTypingAnimation());

        string response = await _geminiAPIClient.SendRequest(currentRequest);

        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
        }

        if (response.Contains("ouagamejam"))
        {
            string pattern = @"ouagamejam:(al|sat):(\d+)";
            Match match = Regex.Match(response, pattern);
            if (match.Success)
            {
                Debug.Log(response);

                string tip = match.Groups[1].Value;
                int saat = int.Parse(match.Groups[2].Value);
                if (tip == "sat")
                    _characterController.IncreaseTime(saat);
                else if (tip == "al")
                    _characterController.DecreaseTime(saat);

                response = Regex.Replace(response, pattern, "").Trim();
                StartCoroutine(CloseWindowAfterDelay(1f));
            }
            else
            {
                response = response.Replace("ouagamejam", "");
                StartCoroutine(CloseWindowAfterDelay(1f));
            }

            waitingForResponse = false;
        }
        else
        {
            waitingForResponse = true;
        }

        if (response.Trim().Length < 3)
        {
            response = "Kolay gelsin.";
        }

        _chatText.text = response;
        Debug.Log("chat text: " + response);
        currentRequest.contents = currentRequest.contents.Append(new Content(Role.Model, response)).ToArray();
    }

    public void SendMessageToAI()
    {
        SendMessageToAI(_responseText.text);
    }

    private IEnumerator ShowTypingAnimation()
    {
        while (true)
        {
            _chatText.text = ".";
            yield return new WaitForSeconds(0.5f);
            _chatText.text = "..";
            yield return new WaitForSeconds(0.5f);
            _chatText.text = "...";
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator CloseWindowAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("closing");
        _chatPanel.SetActive(false);
        _cinemachineVirtualCamera.Priority = 10;
        npcSplineMover.Move(true, () => { StartCoroutine(ExitAndEnterAgain(5f)); });
    }

    private IEnumerator ExitAndEnterAgain(float delay)
    {
        _npc.SetActive(false);
        yield return new WaitForSeconds(delay);
        currentRequest = initialRequest;
        waitingForResponse = true;
        SendMessageToAI("Merhaba, hoş geldiniz nasıl yardımcı olabilirim?");
        _responseText.text = "...";
        _responseText.ForceMeshUpdate();
        _npc.transform.rotation = Quaternion.Euler(3.84257054f, 75.7687149f, 0.948965549f);
        _npc.SetActive(true);
        npcSplineMover.Move(false, () =>
        {
            _chatPanel.SetActive(true);
            _cinemachineVirtualCamera.LookAt = _npc.transform;
            _cinemachineVirtualCamera.Priority = 1;
        });
    }
}