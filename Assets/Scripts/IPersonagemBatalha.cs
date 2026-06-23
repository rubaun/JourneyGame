public interface IPersonagemBatalha
{
    string NomePersonagem { get; }
    bool EstaVivo { get; }

    void CurarVida(int valor);
    void RecuperarMana(float valor);
    void AtivarEscudoItem(int valorEscudo, int turnos);
    void AplicarBuffAtaque(int valor, int turnos);
    void AplicarBuffDefesa(int valor, int turnos);
    void AtualizarEfeitosPorTurno();
    void MostrarTextoAcao(string texto);
}