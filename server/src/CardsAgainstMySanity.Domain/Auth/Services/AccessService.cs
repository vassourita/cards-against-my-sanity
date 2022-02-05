namespace CardsAgainstMySanity.Domain.Auth.Services
{
    public class AccessService
    {
        void CanUserAccess(User user)
        {
            // ** - Um pong acontece a cada 10s

            // -> aspnet valida o access token
            // -> se o access token estiver expirado, gera um novo access token usando o refresh token
            // -> se o jogador não responder durante 6 pongs (1 minuto), ele é temporariamente desativado do jogo (até voltar a responder os pongs).
            // -> se o jogador não responder durante 90 pongs (15 minuto), ele é desconectado do jogo.
            // -> se o player não responder durante 360 pongs (1 hora), seus tokens são invalidados.
            // -> refresh token tem como tempo de acesso 24h.
        }
    }
}