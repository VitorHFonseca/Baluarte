using System.Collections.Generic;
using UnityEngine;
using GameCore.Data;
using GameCore.Simulation;

namespace GameCore.Systems
{
    public interface IAbilityEffect
    {
        void Aplicar(EnemyController alvo, Dictionary<string, float> parametros, MonoBehaviour contexto);
    }

    public class AbilitySystem : MonoBehaviour
    {
        public static AbilitySystem Instance { get; private set; }

        [SerializeField] private List<AbilityData> bancoDeDadosHabilidades;
        private Dictionary<string, AbilityData> _mapaHabilidades = new Dictionary<string, AbilityData>();
        private Dictionary<string, IAbilityEffect> _implementacoesEfeitos = new Dictionary<string, IAbilityEffect>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            foreach (var hab in bancoDeDadosHabilidades)
            {
                _mapaHabilidades.Add(hab.idPoder, hab);
            }

            RegistrarImplementacoesEfeitos();
        }

        private void RegistrarImplementacoesEfeitos()
        {
            _implementacoesEfeitos.Add("ExplosaoEmCadeia", new ChainLightningEffect());
            _implementacoesEfeitos.Add("NuvemToxica", new PoisonCloudEffect());
            // Demais mapeamentos de efeitos do Game Design seriam injetados aqui...
        }

        public AbilityData GetAbilityData(string id)
        {
            return _mapaHabilidades.TryGetValue(id, out var data) ? data : null;
        }

        public AbilityPair SortearPoderes(ElementType elemento, RarityType raridade)
        {
            List<AbilityData> ativosValidos = new List<AbilityData>();
            List<AbilityData> passivosValidos = new List<AbilityData>();

            foreach (var hab in bancoDeDadosHabilidades)
            {
                if (hab.elemento == elemento)
                {
                    if (hab.tipo == AbilityType.Ativo) ativosValidos.Add(hab);
                    else passivosValidos.Add(hab);
                }
            }

            AbilityPair par = new AbilityPair();
            if (ativosValidos.Count > 0) par.Ativo = ativosValidos[Random.Range(0, ativosValidos.Count)];
            if (passivosValidos.Count > 0) par.Passivo = passivosValidos[Random.Range(0, passivosValidos.Count)];

            return par;
        }

        public Dictionary<string, float> CalcularParametrosEscalados(AbilityData poder, CreatureInstance criatura)
        {
            Dictionary<string, float> parametrosEscalados = poder.GetParametrosPadrao();

            // Modificadores baseados nas rolagens genéticas de IV
            if (parametrosEscalados.ContainsKey("dano"))
            {
                float ivDano = criatura.IVs.TryGetValue(StatType.Dano, out var val) ? val : 1.0f;
                parametrosEscalados["dano"] *= ivDano;
            }
            if (parametrosEscalados.ContainsKey("cooldown"))
            {
                float ivCadencia = criatura.IVs.TryGetValue(StatType.CadenciaDeTiro, out var val) ? val : 1.0f;
                parametrosEscalados["cooldown"] /= ivCadencia; // Reduz cooldown se a cadência for alta
            }

            return parametrosEscalados;
        }

        public void AtivarPoder(string idPoder, Dictionary<string, float> parametros, EnemyController alvoAlvo, MonoBehaviour contexto)
        {
            if (_implementacoesEfeitos.TryGetValue(idPoder, out IAbilityEffect efeito))
            {
                efeito.Aplicar(alvoAlvo, parametros, contexto);
            }
            else
            {
                Debug.LogWarning($"Efeito lógico do ID {idPoder} não registrado no motor do AbilitySystem.");
            }
        }
    }

    // Lógicas de Efeito do Jogo Isoladas e Fortemente Otimizadas
    public class ChainLightningEffect : IAbilityEffect
    {
        public void Aplicar(EnemyController alvo, Dictionary<string, float> parametros, MonoBehaviour contexto)
        {
            float danoBase = parametros.ContainsKey("dano") ? parametros["dano"] : 10f;
            float maxAlvos = parametros.ContainsKey("maxAlvos") ? parametros["maxAlvos"] : 4f;
            float raioSalto = parametros.ContainsKey("raioSalto") ? parametros["raioSalto"] : 3.5f;

            List<EnemyController> atingidos = EnemyManager.Instance.QueryInRadius(alvo.transform.position, raioSalto);
            int contagem = 0;
            
            foreach (var inimigo in atingidos)
            {
                if (contagem >= (int)maxAlvos) break;
                inimigo.ReceberDano(danoBase, ElementType.Trovao);
                contagem++;
            }
        }
    }

    public class PoisonCloudEffect : IAbilityEffect
    {
        public void Aplicar(EnemyController alvo, Dictionary<string, float> parametros, MonoBehaviour contexto)
        {
            float danoPorTick = parametros.ContainsKey("danoTick") ? parametros["danoTick"] : 2f;
            float duracao = parametros.ContainsKey("duracao") ? parametros["duracao"] : 6f;
            
            // Instanciar gatilho temporário de área de dano contínuo (DoT) no mapa
            GameObject go = new GameObject("PoisonCloudZone");
            go.transform.position = alvo.transform.position;
            var cloudZone = go.AddComponent<TemporaryCloudTrigger>();
            cloudZone.Setup(danoPorTick, duracao, 2.5f);
        }
    }

    public class TemporaryCloudTrigger : MonoBehaviour
    {
        private float _danoTick;
        private float _duracaoTotal;
        private float _raio;
        private float _timer;
        private float _tickTimer;

        public void Setup(float dano, float tempo, float areaRaio)
        {
            _danoTick = dano;
            _duracaoTotal = tempo;
            _raio = areaRaio;
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            _timer += dt;
            _tickTimer += dt;

            if (_tickTimer >= 0.5f) // Ticks a cada meio segundo
            {
                _tickTimer = 0f;
                var inimigos = EnemyManager.Instance.QueryInRadius(transform.position, _raio);
                foreach (var inv in inimigos)
                {
                    inv.ReceberDano(_danoTick, ElementType.Veneno);
                }
            }

            if (_timer >= _duracaoTotal) Destroy(gameObject);
        }
    }
}
