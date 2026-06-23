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
        public int vigor;
        public int foco;
        public int empatiaAnimal;
        public int sorte;
        public int conhecimentoArcano;

        public int Total => vigor + foco + empatiaAnimal + sorte + conhecimentoArcano;

        public static PlayerStats CreateDefault(CharacterBodyType bodyType)
        {
            return CreateEmpty();
        }

        public static PlayerStats CreateEmpty()
        {
            return new PlayerStats();
        }

        public PlayerStats Clone()
        {
            return new PlayerStats
            {
                vigor = vigor,
                foco = foco,
                empatiaAnimal = empatiaAnimal,
                sorte = sorte,
                conhecimentoArcano = conhecimentoArcano
            };
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
            return Create(nome, corpo, PlayerStats.CreateDefault(corpo));
        }

        public static PlayerProfile Create(string nome, CharacterBodyType corpo, PlayerStats stats)
        {
            string now = DateTime.UtcNow.ToString("o");
            return new PlayerProfile
            {
                id = Guid.NewGuid().ToString(),
                nome = string.IsNullOrWhiteSpace(nome) ? "Aprendiz" : nome.Trim(),
                corpo = corpo,
                stats = stats?.Clone() ?? PlayerStats.CreateEmpty(),
                createdAtUtc = now,
                lastPlayedAtUtc = now
            };
        }
    }
}
