using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using TMPro;

public class LoginPlayer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerAccountClientId;

    async void Start()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
            await UnityServices.InitializeAsync();

        PlayerAccountService.Instance.SignedIn += SignInWithUnityAuth;

        AtualizarStatusConexaoUI();
    }

    void OnDestroy()
    {
        if (PlayerAccountService.Instance != null)
            PlayerAccountService.Instance.SignedIn -= SignInWithUnityAuth;
    }

    private void AtualizarStatusConexaoUI()
    {
        if (playerAccountClientId == null) return;

        if (AuthenticationService.Instance.IsSignedIn)
        {
            playerAccountClientId.text = "Online";
            playerAccountClientId.color = Color.green;
            AtualizarTextoUsuario();
        }
        else
        {
            playerAccountClientId.text = "Offline";
            playerAccountClientId.color = Color.red;
        }
    }

    public async void StartPlayerAccountsSignInAsync()
    {
        if (PlayerAccountService.Instance.IsSignedIn)
        {
            SignInWithUnityAuth();
            return;
        }

        try
        {
            await PlayerAccountService.Instance.StartSignInAsync();
        }
        catch (PlayerAccountsException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    async void SignInWithUnityAuth()
    {
        try
        {
            string accessToken = PlayerAccountService.Instance.AccessToken;

            if (AuthenticationService.Instance.IsSignedIn)
                await LinkWithUnityAsync(accessToken);
            else
                await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);

            Debug.Log("Login/Vínculo Unity concluído com sucesso.");
            AtualizarStatusConexaoUI();
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    async Task LinkWithUnityAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.LinkWithUnityAsync(accessToken);
            Debug.Log("Link is successful.");
        }
        catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            Debug.Log("Conta Unity já vinculada.");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    public void SignOut(bool clearSessionToken = false)
    {
        AuthenticationService.Instance.SignOut(clearSessionToken);
        PlayerAccountService.Instance.SignOut();
        AtualizarStatusConexaoUI();
    }

    private string FormatarIdCurto(string playerId)
    {
        if (string.IsNullOrWhiteSpace(playerId) || playerId.Length < 10) return playerId;
        return $"{playerId.Substring(0, 6)}...{playerId.Substring(playerId.Length - 4)}";
    }

    private void AtualizarTextoUsuario()
    {
        if (playerAccountClientId == null) return;

        string nome = AuthenticationService.Instance.PlayerName;
        if (!string.IsNullOrWhiteSpace(nome))
            playerAccountClientId.text = $"Olá, {nome}";
        else
            playerAccountClientId.text = $"ID: {FormatarIdCurto(AuthenticationService.Instance.PlayerId)}";
    }

    public async void DefinirNomeExibicaoSeVazio(string nomeDesejado)
    {
        if (!AuthenticationService.Instance.IsSignedIn) return;
        if (string.IsNullOrWhiteSpace(nomeDesejado)) return;

        if (!string.IsNullOrWhiteSpace(AuthenticationService.Instance.PlayerName))
            return; // já tem nome

        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(nomeDesejado.Trim());
            AtualizarStatusConexaoUI();
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
}
