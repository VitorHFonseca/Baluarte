using UnityEngine;

namespace GameCore.Simulation
{
    [CreateAssetMenu(fileName = "ConfigTD", menuName = "Sistemas/Configuracao/TD")]
    public class TowerDefenseConfig : ScriptableObject
    {
        public float scanIntervalGlobal = 0.15f;
        public int maxTorresNoMapa = 10;
        public WaveData[] waves;
    }
}
