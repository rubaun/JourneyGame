using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPrefsGame : MonoBehaviour
{
    private string nomeCena;
    private int nivelAtual;
    private string nomeCenaAnterior;
    private int nivelAnterior;

    void Start()
    {
        nomeCena = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("NomeCena", nomeCena);
        nivelAtual = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("NivelAtual", nivelAtual);
        PlayerPrefs.Save();
    }

    public void SetCenaAnterior()
    {   
        if (PlayerPrefs.HasKey("NomeCenaAnterior"))
        {
            PlayerPrefs.SetString("NomeCenaAnterior", nomeCena);
            PlayerPrefs.SetInt("NivelAnterior", nivelAtual);
            PlayerPrefs.Save();
        }
    }

    public string GetCenaAnterior()
    {
        if (PlayerPrefs.HasKey("NomeCenaAnterior"))
        {
            nomeCenaAnterior = PlayerPrefs.GetString("NomeCenaAnterior");
            return nomeCenaAnterior;
        }
        return null;
    }

    public int GetNivelAnterior()
    {
        if (PlayerPrefs.HasKey("NivelAnterior"))
        {
            nivelAnterior = PlayerPrefs.GetInt("NivelAnterior");
            return nivelAnterior;
        }
        return 0;
    }

}
