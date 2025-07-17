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
        soundPlayer = GameObject.Find("Audio Source").GetComponent<SoundPlayer>();
        caixaMensagem = GameObject.Find("InformativoAtiva").GetComponent<Toggle>();
        caixaMensagemObj = GameObject.Find("FundoInformativo");
        
        if (caixaMensagemObj != null)
        {
            caixaMensagemObj.SetActive(caixaMensagem.isOn);
        }

        caixaMensagem.onValueChanged.AddListener(delegate { CaixaMensagem(); });

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
    
    private void CaixaMensagem()
    {
        if (caixaMensagem != null)
        {
            if(caixaMensagem.isOn)
            {
                caixaMensagem.gameObject.SetActive(true);
            }
            else
            {
                caixaMensagem.gameObject.SetActive(false);
            }
        }
    }
}
