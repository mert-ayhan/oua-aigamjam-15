using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeminiAPI.Types;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class GeminiAPIClient : MonoBehaviour
{
    private readonly string URL = "https://generativelanguage.googleapis.com/v1beta/models/";
    private readonly string MODEL = "gemini-1.5-flash-latest";
    private readonly string KEY = "AIzaSyAwwxcfQqAaL3Sm3JFbTms5wNup8z5IBbQ";

    public async Task<string> SendRequest(GenerateContentRequest requestBody)
    {
        string uri = URL + MODEL + ":generateContent?key=" + KEY;
        using (UnityWebRequest webRequest = UnityWebRequest.Post(
                   uri,
                   postData: JsonConvert.SerializeObject(requestBody),
                   contentType: "application/json"
               ))
        {
            Task<UnityWebRequest.Result> requestTask = SendWebRequestAsync(webRequest);
            UnityWebRequest.Result result = await requestTask;

            switch (result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("Error: " + webRequest.error);
                    return "Nasıl yani? Bunu anlayamadım.";
                case UnityWebRequest.Result.Success:
                    Debug.Log("Received: " + webRequest.downloadHandler.text);
                    GenerateContentResponse response =
                        JsonConvert.DeserializeObject<GenerateContentResponse>(webRequest.downloadHandler.text);
                    if (response.candidates[0].finishReason.Equals(Candidate.FinishReason.Safety))
                        return "Bu da ne demek oluyor? Bunu anlayamadım.";

                    return response.candidates[0].content.parts[0].text;
                default:
                    return "Nasıl yani? Bunu anlayamadım.";
            }
        }
    }

    private Task<UnityWebRequest.Result> SendWebRequestAsync(UnityWebRequest webRequest)
    {
        var tcs = new TaskCompletionSource<UnityWebRequest.Result>();
        webRequest.SendWebRequest().completed += operation => { tcs.SetResult(webRequest.result); };
        return tcs.Task;
    }
}