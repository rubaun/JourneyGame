using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using TMPro;

public class LoginPlayer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerAccountClientId; // Substitua pelo seu Client ID

    async void Start()
    {
        await UnityServices.InitializeAsync();

        // Registrar o handler após inicializar os serviços
        PlayerAccountService.Instance.SignedIn += SignInWithUnityAuth;

        if(playerAccountClientId != null)
        {
            playerAccountClientId.text = "Offline";
            playerAccountClientId.color = Color.red;
        }
    }

    void OnDestroy()
    {
        // Remover o handler para evitar referências remanescentes
        if (PlayerAccountService.Instance != null)
            PlayerAccountService.Instance.SignedIn -= SignInWithUnityAuth;
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

    // <-- note que é async void (event handler)
    async void SignInWithUnityAuth()
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
            playerAccountClientId.text = "Logado na Unity";
            playerAccountClientId.color = Color.green;
            Debug.Log("SignIn is successful.");
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
            Debug.LogError("This user is already linked with another account. Log in instead.");
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

    async Task UnlinkUnityAsync()
    {
        try
        {
            await AuthenticationService.Instance.UnlinkUnityAsync();
            Debug.Log("Unlink is successful.");
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
    }
}
