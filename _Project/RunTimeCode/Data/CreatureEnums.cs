namespace GameCore.Data
{
    public enum ElementType
    {
        Fogo,
        Agua,
        Terra,
        Trovao,
        Gelo,
        Sombra,
        Veneno
    }

    public enum RarityType
    {
        Comum,
        Raro,
        Epico,
        Lendario
    }

    public enum StatType
    {
        Dano,
        CadenciaDeTiro,
        SplashRadius,
        CritChance,
        Raio,
        Lentidao_Duracao,
        Knockback,
        Vida,
        Armadura_Propria,
        CustoInvocacao,
        SlowPower,
        ChainRange,
        StunDuracao,
        Velocidade_Projetil,
        FreezeChance,
        FreezeDuracao,
        CritMultiplier,
        InvisibilityWindow,
        LifeSteal,
        DoT_Dano,
        DoT_Duracao,
        StacksDeVeneno
    }

    public enum ProjectileType
    {
        ProjetilUnico,
        Splash,
        Laser,
        OndaDeChoque
    }

    public enum TargetStrategy
    {
        MaisProximo,
        Primeiro,
        Ultimo,
        MaiorVida,
        MenorVida
    }

    public enum AbilityType
    {
        Ativo,
        Passivo
    }
}