using System;
using System.Collections.Generic;
using GameCore.Data;

namespace GameCore.Systems
{
    public static class IV_Generator
    {
        private static readonly Dictionary<ElementType, List<StatType>> PoolPorElemento = new Dictionary<ElementType, List<StatType>>()
        {
            { ElementType.Fogo, new List<StatType>{ StatType.Dano, StatType.CadenciaDeTiro, StatType.SplashRadius, StatType.CritChance } },
            { ElementType.Agua, new List<StatType>{ StatType.Raio, StatType.CadenciaDeTiro, StatType.Lentidao_Duracao, StatType.Knockback } },
            { ElementType.Terra, new List<StatType>{ StatType.Vida, StatType.Armadura_Propria, StatType.CustoInvocacao, StatType.SlowPower } },
            { ElementType.Trovao, new List<StatType>{ StatType.CadenciaDeTiro, StatType.ChainRange, StatType.StunDuracao, StatType.Velocidade_Projetil } },
            { ElementType.Gelo, new List<StatType>{ StatType.Raio, StatType.FreezeChance, StatType.FreezeDuracao, StatType.CritMultiplier } },
            { ElementType.Sombra, new List<StatType>{ StatType.Dano, StatType.CritChance, StatType.InvisibilityWindow, StatType.LifeSteal } },
            { ElementType.Veneno, new List<StatType>{ StatType.DoT_Dano, StatType.DoT_Duracao, StatType.StacksDeVeneno, StatType.SlowPower } }
        };

        public static Dictionary<StatType, float> GenerateIVs(CreatureSpeciesSO especie, int? seed = null)
        {
            Random rng = seed.HasValue ? new Random(seed.Value) : new Random();
            Dictionary<StatType, float> ivsResultantes = new Dictionary<StatType, float>();
            
            List<StatType> poolDisponivel = new List<StatType>(PoolPorElemento[especie.elemento]);
            
            // a) Determina aleatoriamente a quantidade de rolagens de status (2 a 4)
            int quantidadeStats = rng.Next(2, 5); // Limite superior exclusivo (2, 3 ou 4)
            if (quantidadeStats > poolDisponivel.Count) quantidadeStats = poolDisponivel.Count;

            // b) Embaralha a lista para escolher sem repetição (Fisher-Yates)
            for (int i = poolDisponivel.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                var temp = poolDisponivel[i];
                poolDisponivel[i] = poolDisponivel[j];
                poolDisponivel[j] = temp;
            }

            float somaMultiplicadores = 0f;

            for (int i = 0; i < quantidadeStats; i++)
            {
                // b) Multiplicador contínuo entre 0.85 e 1.15
                double multiplicador = 0.85 + (rng.NextDouble() * (1.15 - 0.85));
                float valFloat = (float)Math.Round(multiplicador, 3);
                
                ivsResultantes.Add(poolDisponivel[i], valFloat);
                somaMultiplicadores += valFloat;
            }

            // Garante que todos os status não sorteados fiquem salvos como multiplicador neutro (1.0)
            foreach (StatType stat em Enum.GetValues(typeof(StatType)))
            {
                if (!ivsResultantes.ContainsKey(stat))
                {
                    ivsResultantes.Add(stat, 1.0f);
                    somaMultiplicadores += 1.0f;
                }
            }

            return ivsResultantes;
        }

        public static float CalcularIndicePotencia(Dictionary<StatType, float> ivs, List<StatType> statsPrincipais)
        {
            float somaPonderada = 0f;
            float somatorioPesos = 0f;

            foreach (var item in ivs)
            {
                // Status pertencentes ao pool nativo têm peso 3, status neutros têm peso 1
                float peso = statsPrincipais.Contains(item.Key) ? 3.0f : 1.0f;
                somaPonderada += item.Value * peso;
                somatorioPesos += peso;
            }

            return somaPonderada / somatorioPesos;
        }

        public static List<StatType> GetPoolPorElemento(ElementType elemento)
        {
            return PoolPorElemento[elemento];
        }
    }
}