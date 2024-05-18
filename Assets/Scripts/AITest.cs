using System.Collections;
using System.Collections.Generic;
using GeminiAPI.Types;
using UnityEngine;

public class AITest : MonoBehaviour
{
    private readonly List<string> customerNames = new()
        { "Gabe Broflovski", "Alice Marlowe", "Derek Wainwright", "Sophie Kensington", "Ethan Hawthorne" };

    private GeminiAPIClient _geminiAPIClient;

    async void Start()
    {
        _geminiAPIClient = GetComponent<GeminiAPIClient>();

        /*while (customerNames.Count > 0)
        {
            int randomIndex = Random.Range(0, customerNames.Count);
            string currentCustomerName = customerNames[randomIndex];
            customerNames.RemoveAt(randomIndex);
            Debug.Log(currentCustomerName);
        }*/

        GenerateContentRequest requestBody = new()
        {
            contents = new Content[]
            {
                new Content(Role.User,
                    "Merhaba, hoş geldiniz nasıl yardımcı olabilirim?"
                )
            },
            systemInstruction =
                new Content(
                    "Bir oyun tasarlıyorum. Oyuncu tezgahtar ve 5 farklı müşteri içeri girip eşya satıyor. Bu müşterilerden birisi gibi davran.")
        };

        await _geminiAPIClient.SendRequest(requestBody);
    }

    void Update()
    {
    }
}