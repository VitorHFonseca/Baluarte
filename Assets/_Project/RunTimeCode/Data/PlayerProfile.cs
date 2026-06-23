using System;

namespace GameCore.Data
{
    public enum CharacterBodyType
    {
        Homem,
        Mulher
    }

    [Serializable]
    public class PlayerStats
    {
        public int vigor = 5;
        public int foco = 5;
        public int empatiaAnimal = 5;
        public int sorte = 5;
        public int conhecimentoArcano = 5;

        public int Total => vigor + foco + empatiaAnimal + sorte + conhecimentoArcano;

        public static PlayerStats CreateDefault(CharacterBodyType bodyType)
        {
            PlayerStats stats = new PlayerStats();

            if (bodyType == CharacterBodyType.Homem)
            {
                stats.vigor = 6;
                stats.foco = 5;
                stats.empatiaAnimal = 5;
                stats.sorte = 4;
                stats.conhecimentoArcano = 5;
            }
            else
            {
                stats.vigor = 5;
                stats.foco = 6;
                stats.empatiaAnimal = 5;
                stats.sorte = 4;
                stats.conhecimentoArcano = 5;
            }

            return stats;
        }
    }

    [Serializable]
    public class PlayerProfile
    {
        public string id;
        public string nome;
        public CharacterBodyType corpo;
        public PlayerStats stats;
        public string createdAtUtc;
        public string lastPlayedAtUtc;

        public static PlayerProfile Create(string nome, CharacterBodyType corpo)
        {
            string now = DateTime.UtcNow.ToString("o");
            return new PlayerProfile
            {
                id = Guid.NewGuid().ToString(),
                nome = string.IsNullOrWhiteSpace(nome) ? "Aprendiz" : nome.Trim(),
                corpo = corpo,
                stats = PlayerStats.CreateDefault(corpo),
                createdAtUtc = now,
                lastPlayedAtUtc = now
            };
        }
    }
}
