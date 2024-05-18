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
    private readonly string KEY = "AIzaSyDTeSHjdhcJMzrI1pbN_uoZvG7_6kr5in0";

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
                    break;
                case UnityWebRequest.Result.Success:
                    GenerateContentResponse response =
                        JsonConvert.DeserializeObject<GenerateContentResponse>(webRequest.downloadHandler.text);
                    Debug.Log("Received: " + response.candidates[0].content.parts[0].text);
                    break;
            }
        }

        return "";
    }

    private Task<UnityWebRequest.Result> SendWebRequestAsync(UnityWebRequest webRequest)
    {
        var tcs = new TaskCompletionSource<UnityWebRequest.Result>();
        webRequest.SendWebRequest().completed += operation => { tcs.SetResult(webRequest.result); };
        return tcs.Task;
    }
}