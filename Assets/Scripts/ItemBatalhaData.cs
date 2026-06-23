using UnityEngine;

public enum CategoriaItem
{
    Pocao,
    Arma,
    Vestuario,
    Missao
}

public enum EfeitoItemBatalha
{
    Nenhum,
    CuraVida,
    RecuperaMana,
    BuffAtaque,
    BuffDefesa,
    Escudo
}

// Compatibilidade com código legado
public enum TipoItemBatalha
{
    Nenhum = EfeitoItemBatalha.Nenhum,
    CuraVida = EfeitoItemBatalha.CuraVida,
    RecuperaMana = EfeitoItemBatalha.RecuperaMana,
    BuffAtaque = EfeitoItemBatalha.BuffAtaque,
    BuffDefesa = EfeitoItemBatalha.BuffDefesa,
    Escudo = EfeitoItemBatalha.Escudo
}

[CreateAssetMenu(fileName = "NovoItem", menuName = "Jorney/Item Batalha")]
public class ItemBatalhaData : ScriptableObject
{
    public string id;
    public string nomeExibicao;
    [TextArea] public string descricao;
    public Sprite icone; // imagem exibida no slot da UI

    public CategoriaItem categoria;
    public EfeitoItemBatalha efeitoBatalha = EfeitoItemBatalha.Nenhum;

    // Alias legado: item.tipo
    public EfeitoItemBatalha tipo
    {
        get { return efeitoBatalha; }
        set { efeitoBatalha = value; }
    }

    public bool utilizavelEmBatalha = true;
    public bool consomeTurno = true;

    public int valor = 10;
    public int duracaoTurnos = 1;
}