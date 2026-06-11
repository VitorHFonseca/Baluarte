using System.Collections.Generic;
using UnityEngine;
using GameCore.Data;

namespace GameCore.Simulation
{
    public class EnemyController : MonoBehaviour
    {
        public float vidaAtual = 100f;
        public bool estaMorto = false;

        public void ReceberDano(float dano, ElementType elementoDoAtaque)
        {
            if (estaMorto) return;
            vidaAtual -= dano;
            if (vidaAtual <= 0)
            {
                estaMorto = true;
                EnemyManager.Instance.RemoverInimigo(this);
                Destroy(gameObject);
            }
        }
    }

    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance { get; private set; }

        private List<EnemyController> _todosInimigos = new List<EnemyController>();

        // Estrutura Simples de Otimização por Spatial Hashing para busca espacial O(1)
        private Dictionary<Vector2Int, List<EnemyController>> _gridBucket = new Dictionary<Vector2Int, List<EnemyController>>();
        [SerializeField] private float tamanhoCelulaGrid = 3f;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Update()
        {
            AtualizarSpatialHash();
        }

        private void AtualizarSpatialHash()
        {
            _gridBucket.Clear();
            for (int i = 0; i < _todosInimigos.Count; i++)
            {
                var inimigo = _todosInimigos[i];
                if (inimigo == null) continue;

                Vector2Int posGrid = ConverterParaGrid(inimigo.transform.position);
                if (!_gridBucket.ContainsKey(posGrid))
                {
                    _gridBucket.Add(posGrid, new List<EnemyController>());
                }
                _gridBucket[posGrid].Add(inimigo);
            }
        }

        private Vector2Int ConverterParaGrid(Vector3 pos)
        {
            return new Vector2Int(Mathf.FloorToInt(pos.x / tamanhoCelulaGrid), Mathf.FloorToInt(pos.z / tamanhoCelulaGrid));
        }

        public List<EnemyController> QueryInRadius(Vector3 centro, float raio)
        {
            List<EnemyController> resultado = new List<EnemyController>();
            float raioQuadrado = raio * raio;

            int celulasRaio = Mathf.CeilToInt(raio / tamanhoCelulaGrid);
            Vector2Int centroGrid = ConverterParaGrid(centro);

            // Vasculha apenas os buckets vizinhos ao invés de usar OverlapSphere
            for (int x = -celulasRaio; x <= celulasRaio; x++)
            {
                for (int y = -celulasRaio; y <= celulasRaio; y++)
                {
                    Vector2Int alvoCel = centroGrid + new Vector2Int(x, y);
                    if (_gridBucket.TryGetValue(alvoCel, out var listaInimigos))
                    {
                        for (int i = 0; i < listaInimigos.Count; i++)
                        {
                            var e = listaInimigos[i];
                            if ((e.transform.position - centro).sqrMagnitude <= raioQuadrado)
                            {
                                resultado.Add(e);
                            }
                        }
                    }
                }
            }
            return resultado;
        }

        public void AdicionarInimigo(EnemyController inimigo) => _todosInimigos.Add(inimigo);
        
        public void RemoverInimigo(EnemyController inimigo)
        {
            if (_todosInimigos.Contains(inimigo)) _todosInimigos.Remove(inimigo);
        }
    }
}