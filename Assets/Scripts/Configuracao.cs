using UnityEngine;
using UnityEngine.UI;

public class Configuracao : MonoBehaviour
{
    private Slider somSlider;
    private Slider musicaSlider;
    private Toggle caixaMensagem;
    private GameObject caixaMensagemObj;
    private SoundPlayer soundPlayer;

    private void Awake()
    {
        somSlider = GameObject.Find("SomVolume").GetComponent<Slider>();
        musicaSlider = GameObject.Find("MusicaVolume").GetComponent<Slider>();
        soundPlayer = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundPlayer>();
        caixaMensagem = GameObject.Find("InformativoAtiva").GetComponent<Toggle>();
        caixaMensagemObj = GameObject.Find("FundoInformativo");

        caixaMensagemObj.SetActive(caixaMensagem.isOn);

        if (caixaMensagem != null)
        {
            caixaMensagem.onValueChanged.AddListener(delegate { AlternarCaixaMensagem(); });
        }

        if (somSlider != null)
        {
            somSlider.onValueChanged.AddListener(soundPlayer.SetSomVolume);
            somSlider.value = soundPlayer.GetSomVolume();
        }

        if (musicaSlider != null)
        {
            musicaSlider.onValueChanged.AddListener(soundPlayer.SetMusicaVolume);
            musicaSlider.value = soundPlayer.GetMusicaVolume();
        }
    }

    public void SalvarConfig()
    {
        soundPlayer.SaveVolumes();
    }

    // Método para alternar a visibilidade da caixa de mensagem
    public void AlternarCaixaMensagem()
    {
        if (caixaMensagemObj != null)
        {
            caixaMensagemObj.SetActive(caixaMensagem.isOn);
        }
    }
}
