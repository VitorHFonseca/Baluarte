using System.Collections.Generic;
using UnityEngine;
using GameCore.Data;
using GameCore.Systems;

namespace GameCore.Simulation
{
    [System.Serializable]
    public struct WaveData
    {
        public int quantidadeInimigos;
        public float spawnInterval;
        public GameObject prefabInimigo;
    }

    [CreateAssetMenu(fileName = "ConfigTD", menuName = "Sistemas/Configuração/TD")]
    public class TowerDefenseConfig : ScriptableObject
    {
        public float scanIntervalGlobal = 0.15f;
        public int maxTorresNoMapa = 10;
        public WaveData[] waves;
    }

    public class TowerDefenseEngine : MonoBehaviour
    {
        public static TowerDefenseEngine Instance { get; private set; }
        
        public TowerDefenseConfig configTD;
        private List<CreatureInstance> _torresDisponiveisParaUso;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void CarregarTorresDisponiveis(SessionInventory inventario)
        {
            _torresDisponiveisParaUso = inventario.animaisCapturados.FindAll(x => x.SelecionadoParaDefesa);
            Debug.Log($"Engine de Combate carregada com {_torresDisponiveisParaUso.Count} criaturas prontas para ação.");
        }
    }
}