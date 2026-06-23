using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

public class SessaoJogoManager : MonoBehaviour
{
    public static SessaoJogoManager Instance { get; private set; }

    private const string KeyCheckpointCenaLocal = "CheckpointCenaBatalha";
    private const string KeyCheckpointAtivoLocal = "CheckpointBatalhaAtivo";
    private const string KeyCheckpointCenaCloud = "checkpoint_cena";
    private const string KeyCheckpointAtivoCloud = "checkpoint_ativo";

    private string checkpointCenaCache;
    private bool checkpointAtivoCache;

    public bool ServicosProntos { get; private set; }
    public bool CheckpointSincronizado { get; private set; }
    public bool Logado => ServicosProntos && AuthenticationService.Instance.IsSignedIn;

    public event Action OnCheckpointAtualizado;

    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CheckpointSincronizado = false;
        CarregarCheckpointLocalParaCache();

        await InicializarLoginAsync();
        await CarregarCheckpointCloudAsync();

        CheckpointSincronizado = true;
        OnCheckpointAtualizado?.Invoke();
    }

    private async Task InicializarLoginAsync()
    {
        if (ServicosProntos) return;

        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        ServicosProntos = true;
        Debug.Log($"Sessão pronta. PlayerId: {AuthenticationService.Instance.PlayerId}");
    }

    public bool TemJogoEmAndamento()
    {
        return checkpointAtivoCache && !string.IsNullOrWhiteSpace(checkpointCenaCache);
    }

    public void DescartarJogoAtual()
    {
        checkpointAtivoCache = false;
        checkpointCenaCache = string.Empty;
        SalvarCheckpointLocal();
        _ = SalvarCheckpointCloudAsync();
        OnCheckpointAtualizado?.Invoke();
    }

    public void MarcarBatalhaEmAndamento(string nomeCenaBatalha)
    {
        if (string.IsNullOrWhiteSpace(nomeCenaBatalha)) return;

        checkpointCenaCache = nomeCenaBatalha;
        checkpointAtivoCache = true;

        SalvarCheckpointLocal();
        _ = SalvarCheckpointCloudAsync();
        OnCheckpointAtualizado?.Invoke();
    }

    public void ConcluirBatalhaAtual()
    {
        checkpointAtivoCache = false;
        SalvarCheckpointLocal();
        _ = SalvarCheckpointCloudAsync();
        OnCheckpointAtualizado?.Invoke();
    }

    public string ObterCenaParaContinuar(string cenaInicialPadrao)
    {
        if (!checkpointAtivoCache)
            return cenaInicialPadrao;

        return string.IsNullOrWhiteSpace(checkpointCenaCache) ? cenaInicialPadrao : checkpointCenaCache;
    }

    private void CarregarCheckpointLocalParaCache()
    {
        checkpointCenaCache = PlayerPrefs.GetString(KeyCheckpointCenaLocal, string.Empty);
        checkpointAtivoCache = PlayerPrefs.GetInt(KeyCheckpointAtivoLocal, 0) == 1;
    }

    private void SalvarCheckpointLocal()
    {
        PlayerPrefs.SetString(KeyCheckpointCenaLocal, checkpointCenaCache ?? string.Empty);
        PlayerPrefs.SetInt(KeyCheckpointAtivoLocal, checkpointAtivoCache ? 1 : 0);
        PlayerPrefs.Save();
    }

    private async Task SalvarCheckpointCloudAsync()
    {
        if (!Logado) return;

        try
        {
            var payload = new Dictionary<string, object>
            {
                { KeyCheckpointCenaCloud, checkpointCenaCache ?? string.Empty },
                { KeyCheckpointAtivoCloud, checkpointAtivoCache ? 1 : 0 }
            };

            await CloudSaveService.Instance.Data.ForceSaveAsync(payload);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Cloud Save (save) falhou: {ex.Message}");
        }
    }

    private async Task CarregarCheckpointCloudAsync()
    {
        if (!Logado) return;

        try
        {
            var keys = new HashSet<string> { KeyCheckpointCenaCloud, KeyCheckpointAtivoCloud };
            var data = await CloudSaveService.Instance.Data.LoadAsync(keys);

            if (data.TryGetValue(KeyCheckpointCenaCloud, out var cena))
                checkpointCenaCache = cena;

            if (data.TryGetValue(KeyCheckpointAtivoCloud, out var ativoStr) && int.TryParse(ativoStr, out int ativo))
                checkpointAtivoCache = ativo == 1;

            SalvarCheckpointLocal();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Cloud Save (load) falhou: {ex.Message}");
        }
    }
}