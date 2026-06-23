using Firebase;
using Firebase.Messaging;
using UnityEngine;

public class PushSmokeTest : MonoBehaviour
{
    private async void Start()
    {
        var status = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (status != DependencyStatus.Available)
        {
            Debug.LogError("Firebase indisponível: " + status);
            return;
        }

        FirebaseMessaging.TokenReceived += OnTokenReceived;
        FirebaseMessaging.MessageReceived += OnMessageReceived;

        Debug.Log("Firebase Messaging inicializado.");
    }

    private void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("FCM TOKEN: " + token.Token);
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("Push recebida: " + (e.Message?.MessageId ?? "sem id"));
    }

    private void OnDestroy()
    {
        FirebaseMessaging.TokenReceived -= OnTokenReceived;
        FirebaseMessaging.MessageReceived -= OnMessageReceived;
    }
}