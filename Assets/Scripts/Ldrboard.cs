// .------..------..------..------..------..------..------..------..------..------..------..------.
// |L.--. ||D.--. ||R.--. ||B.--. ||O.--. ||A.--. ||R.--. ||D.--. ||..--. ||C.--. ||O.--. ||M.--. |
// | :/\: || :/\: || :(): || :(): || :/\: || (\/).|| :(): || :/\: || :(): || :/\: || :/\: || (\/).|
// | (__) || (__) || ()() || ()() || :\/:.|| :\/:.|| ()().|| (__) || ()().|| :\/:.|| :\/:.|| :\/:.|
// | '--'L|| '--'D|| '--'R|| '--'B|| '--'O|| '--'A|| '--'R|| '--'D|| '--'.|| '--'C|| '--'O|| '--'M|
// `------'`------'`------'`------'`------'`------'`------'`------'`------'`------'`------'`------'

using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System;
using UnityEditor;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


// Ldrboard.cs


public class Ldrboard : MonoBehaviour
{
    
    public static IEnumerator ldrboardWrite(float kills = 0.0f, float wave = 0.0f, string playerName = "", bool errorOnNullPlayerName = false, bool test = false, bool debug = true)
    {
        var url = "https://api.ldrboard.com/api/v1/ldrboard/anthilldefense/log-secure-json" + (test ? "?test=true" : "");

        string playerUniqueId = SystemInfo.deviceUniqueIdentifier;

        StringBuilder jsonBodyBuilder = new StringBuilder();
        jsonBodyBuilder.Append("{");
        jsonBodyBuilder.Append("\"player_unique_id\":\"").Append(playerUniqueId).Append("\",");
        jsonBodyBuilder.Append("\"player_name\":\"").Append(playerName).Append("\",");


        jsonBodyBuilder.Append("\"kills\":").Append(kills).Append(",");
        jsonBodyBuilder.Append("\"wave\":").Append(wave).Append(",");
        jsonBodyBuilder.Append("\"error_on_null_player_name\":").Append(errorOnNullPlayerName.ToString().ToLower());
        jsonBodyBuilder.Append("}");

        var jsonBody = jsonBodyBuilder.ToString();

        string publicKey = @"
        <RSAKeyValue>
        <Modulus>nYorElAmo5nSSj0tC2tqG2+oUqFVivnULz/Ghn5ZQSxMlOCFbh7cmfzHcMgkXFHWavSbqzE0oKrvMXxY5cUSZd8TGubdjyBjukeVN4AFz0+wrURIgE4tdkFf/WLaSAZhOYtA67OK+rbrwOZttSAOQcHRSCAugLRkM5Cz4a38P8YrrHCaQE2f7qGAwRb//Kz1tydbA/zoKg9A8xcZX9TLIVgWy4yMDt+4v4ehNnTh83xk83znH8qJ6EoYGs8iGbCK/Jglk7RNIw0b/Fh3iGRkJ/Wosekvc58KVvUq/79a/pidCBGBe/VzgIfNwoHnCxnwABroHu/QVuPq4Jc1zJsGkw==</Modulus>
        <Exponent>AQAB</Exponent>
        </RSAKeyValue>
        ";
        string Encrypt(string message)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.FromXmlString(publicKey);
                byte[] dataToEncrypt = Encoding.UTF8.GetBytes(message);
                byte[] encryptedData = rsa.Encrypt(dataToEncrypt, false);
                return Convert.ToBase64String(encryptedData).Replace('+', '-').Replace('/', '_');
            }
        }
        jsonBody = "{ \"data\": \"" + Encrypt(jsonBody) + "\"}";

        var request = new UnityWebRequest(url, "PUT")
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonBody)),
            downloadHandler = new DownloadHandlerBuffer()
        };

        request.SetRequestHeader("Content-Type", "application/json");


        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (debug)
            {
                Debug.Log("Request completed with response code: " + request.responseCode);
                Debug.Log("Response body: " + request.downloadHandler.text);

            }
            GameManager.instance.submitButton.SetActive(false);
            GameManager.instance.nameInput.gameObject.SetActive(false);
            
        }
        else
        {
            Debug.LogError("Request failed: " + request.error);
            if (request.downloadHandler != null)
            {
                Debug.LogError("Response body: " + request.downloadHandler.text);
            }
            GameManager.instance.submitError.gameObject.SetActive(true);
        }
    }


    public void SubmitScore()
    {
        GameManager.instance.submitError.gameObject.SetActive(false);
        if (string.IsNullOrEmpty(GameManager.instance.nameInput.text)) return;
        StartCoroutine(ldrboardWrite( GameManager.instance.kills, GameManager.instance.wave.currentWave, GameManager.instance.nameInput.text));
        
    }
}