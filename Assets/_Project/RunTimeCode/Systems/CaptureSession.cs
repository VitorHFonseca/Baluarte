using System;
using System.Collections.Generic;
using UnityEngine;
using GameCore.Data;

namespace GameCore.Systems
{
    public class CaptureSession : MonoBehaviour
    {
        public static CaptureSession Instance { get; private set; }

        public event Action<SessionInventory> OnSessionEnd;

        [Header("Economia da Sessão")]
        public float energiaCaptura = 30f;
        public float tempoGlobalRestante = 90f;
        private bool _sessaoAtiva = false;

        private SessionInventory _inventarioSessao;
        private ResistanceBar _barraResistenciaAtual;
        private CreatureInstance _criaturaSendoCapturada;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void IniciarSessão()
        {
            _inventarioSessao = new SessionInventory();
            _sessaoAtiva = true;
            SpawnProximaCriatura();
        }

        private void Update()
        {
            if (!_sessaoAtiva) return;

            float dt = Time.deltaTime;
            tempoGlobalRestante -= dt;

            if (_barraResistenciaAtual != null)
            {
                _barraResistenciaAtual.Update(dt);
                if (_barraResistenciaAtual.IsCapturado())
                {
                    ProcessarSucessoCaptura();
                }
                else if (_barraResistenciaAtual.IsEscapou())
                {
                    ProcessarFugaCriatura();
                }
            }

            if (tempoGlobalRestante <= 0 || energiaCaptura <= 0)
            {
                FinalizarSessao();
            }
        }

        public void SpawnProximaCriatura()
        {
            // Sistema de Loot por Peso baseado em Bioma fictício
            RarityType raridadeSorteada = SorterRaridadePorPesos();
            ElementType elementoSorteado = ElementType.Fogo; // Exemplo fixo: Bioma Vulcânico

            // Criar Instância de Dados temporária
            _criaturaSendoCapturada = new CreatureInstance
            {
                Raridade = raridadeSorteada,
                Elemento = elementoSorteado,
                ID_Especie = "esp_dragao_fogo"
            };

            // Cria a barra de resistência de acordo com a mecânica do Gacha Clicker
            _barraResistenciaAtual = new ResistanceBar(raridadeSorteada);
        }

        public void TratarCliqueDeCaptura()
        {
            if (!_sessaoAtiva || _barraResistenciaAtual == null) return;
            _barraResistenciaAtual.OnClick();
        }

        private void ProcessarSucessoCaptura()
        {
            int custo = ObterCustoEnergia(_criaturaSendoCapturada.Raridade);
            energiaCaptura -= custo;

            // Gerar IVs genéticos e Habilidades definitivas pós-captura bem sucedida
            // Aqui buscamos um ScriptableObject mockado
            CreatureSpeciesSO dummySO = ScriptableObject.CreateInstance<CreatureSpeciesSO>();
            dummySO.elemento = _criaturaSendoCapturada.Elemento;
            dummySO.idEspecie = _criaturaSendoCapturada.ID_Especie;

            _criaturaSendoCapturada.IVs = IV_Generator.GenerateIVs(dummySO);
            _criaturaSendoCapturada.IndicePotencia = IV_Generator.CalcularIndicePotencia(_criaturaSendoCapturada.IVs, IV_Generator.GetPoolPorElemento(_criaturaSendoCapturada.Elemento));

            var parPoderes = AbilitySystem.Instance.SortearPoderes(_criaturaSendoCapturada.Elemento, _criaturaSendoCapturada.Raridade);
            _criaturaSendoCapturada.ID_PoderAtivo = parPoderes.Ativo != null ? parPoderes.Ativo.idPoder : "Nenhum";
            _criaturaSendoCapturada.ID_PoderPassivo = parPoderes.Passivo != null ? parPoderes.Passivo.idPoder : "Nenhum";

            _inventarioSessao.animaisCapturados.Add(_criaturaSendoCapturada);
            
            CleanCurrentCapture();
            SpawnProximaCriatura();
        }

        private void ProcessarFugaCriatura()
        {
            CleanCurrentCapture();
            SpawnProximaCriatura();
        }

        private void CleanCurrentCapture()
        {
            _barraResistenciaAtual = null;
            _criaturaSendoCapturada = null;
        }

        private void FinalizarSessao()
        {
            _sessaoAtiva = false;
            _barraResistenciaAtual = null;
            OnSessionEnd?.Invoke(_inventarioSessao);
        }

        private int ObterCustoEnergia(RarityType raridade)
        {
            return raridade switch
            {
                RarityType.Comum => 1,
                RarityType.Raro => 3,
                RarityType.Epico => 6,
                RarityType.Lendario => 12,
                _ => 1
            };
        }

        private RarityType SorterRaridadePorPesos()
        {
            // Roleta de Pesos Base: Comum=700, Raro=200, Épico=80, Lendário=20
            int[] pesos = { 700, 200, 80, 20 };
            int totalPesos = 700 + 200 + 80 + 20;
            int r = UnityEngine.Random.Range(0, totalPesos);

            if (r < pesos[0]) return RarityType.Comum;
            if (r < pesos[0] + pesos[1]) return RarityType.Raro;
            if (r < pesos[0] + pesos[1] + pesos[2]) return RarityType.Epico;
            return RarityType.Lendario;
        }
    }

    public class ResistanceBar
    {
        private float _hpMaximo;
        private float _hpAtual;
        private float _decaimentoPorSegundo;
        private float _tempoLimite;
        private float _timerCorrido;
        private float _escapeTimer;
        private RarityType _raridade;

        // Controle do multiplicador de combos consecutivos
        private float _ultimoCliqueTime;
        private int _contadorCombo;
        private const float JanelaCombo = 0.5f;

        public ResistanceBar(RarityType raridade)
        {
            _raridade = raridade;
            _timerCorrido = 0f;
            _escapeTimer = 0f;
            _contadorCombo = 0;
            _ultimoCliqueTime = -10f;

            switch (raridade)
            {
                case RarityType.Comum:
                    _hpMaximo = 1f; // Click único instantâneo
                    _decaimentoPorSegundo = 0f;
                    _tempoLimite = 999f;
                    break;
                case RarityType.Raro:
                    _hpMaximo = 30f;
                    _decaimentoPorSegundo = 0.08f * _hpMaximo; // 8% por seg
                    _tempoLimite = 15f;
                    break;
                case RarityType.Epico:
                    _hpMaximo = 60f;
                    _decaimentoPorSegundo = 0.12f * _hpMaximo; // 12% por seg
                    _tempoLimite = 12f;
                    break;
                case RarityType.Lendario:
                    _hpMaximo = 100f;
                    _decaimentoPorSegundo = 0.18f * _hpMaximo; // 18% por seg
                    _tempoLimite = 10f;
                    break;
            }
            _hpAtual = _hpMaximo;
        }

        public void Update(float deltaTime)
        {
            if (_raridade == RarityType.Comum) return;

            _timerCorrido += deltaTime;
            
            // Decaimento passivo natural da barra
            _hpAtual = Mathf.Min(_hpMaximo, _hpAtual + (_decaimentoPorSegundo * deltaTime));

            // Mecânica de Escape do Lendário: A cada 3 segundos, ganha +15 HP de surpresa
            if (_raridade == RarityType.Lendario)
            {
                _escapeTimer += deltaTime;
                if (_escapeTimer >= 3f)
                {
                    _escapeTimer = 0f;
                    _hpAtual = Mathf.Min(_hpMaximo, _hpAtual + 15f);
                }
            }
        }

        public void OnClick()
        {
            float agora = Time.time;
            if (agora - _ultimoCliqueTime <= JanelaCombo)
            {
                _contadorCombo = Mathf.Min(3, _contadorCombo + 1);
            }
            else
            {
                _contadorCombo = 0;
            }
            _ultimoCliqueTime = agora;

            // Dano Base por clique alterado pelo combo do clicker
            float danoBaseClique = 3f;
            float multiplicadorCombo = 1.0f + (_contadorCombo * 0.10f);
            float danoFinal = danoBaseClique * multiplicadorCombo;

            if (_raridade == RarityType.Comum) _hpAtual = 0f;
            else _hpAtual = Mathf.Max(0f, _hpAtual - danoFinal);
        }

        public bool IsCapturado() => _hpAtual <= 0f;
        public bool IsEscaped() => _timerCorrido >= _tempoLimite;
        public bool IsEscapou() => IsEscaped();

        public float GetProgressoNormalizado() => _hpAtual / _hpMaximo;
    }
}
