using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GeminiAPI.Types;
using TMPro;
using UnityEngine;

public class AITest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _chatText;
    [SerializeField] private TextMeshProUGUI _responseText;
    [SerializeField] private GameObject _chatPanel;

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
    private bool waitingForResponse = true;

    void Start()
    {
        _geminiAPIClient = GetComponent<GeminiAPIClient>();
        _characterController = GetComponent<CharacterController>();
        _chatText.text = "...";

        currentRequest = initialRequest;
        SendMessageToAI("Merhaba, hoş geldiniz nasıl yardımcı olabilirim");
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
            StartCoroutine(CloseWindowAfterDelay(5f));
        }
        else
        {
            waitingForResponse = true;
        }

        if (response.Length < 1)
        {
            response = "Kolay gelsin.";
        }

        _chatText.text = response;
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
        waitingForResponse = false;
        Debug.Log("closing");
        //_chatPanel.SetActive(false);
    }
}