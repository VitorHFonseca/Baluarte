using System.Collections.Generic;
using GameCore.Data;
using GameCore.Systems;
using UnityEngine;

namespace GameCore.Simulation
{
    [System.Serializable]
    public struct WaveData
    {
        public int quantidadeInimigos;
        public float spawnInterval;
        public GameObject prefabInimigo;
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
            Debug.Log($"Engine de Combate carregada com {_torresDisponiveisParaUso.Count} criaturas prontas para acao.");
        }
    }
}
