using UnityEngine;

namespace GameCore.Data
{
    [CreateAssetMenu(fileName = "NovaEspecie", menuName = "Sistemas/Criatura/Especie")]
    public class CreatureSpeciesSO : ScriptableObject
    {
        [Header("Metadados Base")]
        public string idEspecie;
        public string nomeEspecie;
        public ElementType elemento;
        public RarityType raridadeBase;

        [Header("Atributos de Combate Base")]
        public float vidaBase = 100f;
        public float danoBase = 15f;
        public float cadenciaDeTiroBase = 1f; // Tiros por segundo
        public float raioBase = 5f; // Alcance da torre
        public int custoInvocacaoBase = 50;

        [Header("Configurações do Componente de Torre")]
        public ProjectileType tipoDeProjetil;
        public TargetStrategy estrategiaAlvoPadrao;

        [Header("Assets Visuais")]
        public Sprite spriteIdle;
        public Sprite spriteAttack;
        public GameObject prefabProjetil;
        public Sprite iconeUI;
    }
}