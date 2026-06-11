using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Data
{
    [CreateAssetMenu(fileName = "NovaHabilidade", menuName = "Sistemas/Criatura/Habilidade")]
    public class AbilityData : ScriptableObject
    {
        public string idPoder;
        public string nome;
        [TextArea(2, 5)] public string descricao;
        
        public AbilityType tipo;
        public ElementType elemento;
        public float cooldown;

        // Parâmetros de customização do efeito de jogo da habilidade
        public List<string> chavesParametros = new List<string>();
        public List<float> valoresPadrao = new List<float>();

        public GameObject prefabEfeitoVisual;
        public AudioClip somAtivacao;

        public Dictionary<string, float> GetParametrosPadrao()
        {
            Dictionary<string, float> map = new Dictionary<string, float>();
            for (int i = 0; i < chavesParametros.Count; i++)
            {
                if (i < valoresPadrao.Count)
                    map.Add(chavesParametros[i], valoresPadrao[i]);
            }
            return map;
        }
    }

    public struct AbilityPair
    {
        public AbilityData Ativo;
        public AbilityData Passivo;
    }
}