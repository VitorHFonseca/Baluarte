using System.Collections.Generic;
using UnityEngine;
using GameCore.Data;
using GameCore.Systems;

namespace GameCore.Simulation
{
    public class TowerBrain : MonoBehaviour
    {
        private CreatureInstance _dadosInstancia;
        private CreatureSpeciesSO _dadosEspecieConfig;

        // Atributos finais em cache recalculados por frame/sessão
        private float _danoFinal;
        private float _cadenciaFinal;
        private float _raioFinal;
        private float _scanTimer;
        private float _tiroCooldownTimer;
        private float _habilidadeCooldownTimer;

        private EnemyController _alvoAtual;
        private Dictionary<string, float> _parametrosHabilidadeEscalados;

        public void Init(CreatureInstance criatura, CreatureSpeciesSO configBase)
        {
            _dadosInstancia = criatura;
            _dadosEspecieConfig = configBase;

            // 6.1 Processamento inicial de atributos aplicando os IVs genéticos à base
            _danoFinal = _dadosEspecieConfig.danoBase * (criatura.IVs.TryGetValue(StatType.Dano, out var ivDano) ? ivDano : 1.0f);
            _cadenciaFinal = _dadosEspecieConfig.cadenciaDeTiroBase * (criatura.IVs.TryGetValue(StatType.CadenciaDeTiro, out var ivCad) ? ivCad : 1.0f);
            _raioFinal = _dadosEspecieConfig.raioBase * (criatura.IVs.TryGetValue(StatType.Raio, out var ivRaio) ? ivRaio : 1.0f);

            // Carrega e armazena os parâmetros escalados das habilidades para evitar alocação em tempo de execução
            if (_dadosInstancia.ID_PoderAtivo != "Nenhum")
            {
                AbilityData dataAtivo = AbilitySystem.Instance.GetAbilityData(_dadosInstancia.ID_PoderAtivo);
                if (dataAtivo != null)
                {
                    _parametrosHabilidadeEscalados = AbilitySystem.Instance.CalcularParametrosEscalados(dataAtivo, _dadosInstancia);
                    _habilidadeCooldownTimer = dataAtivo.cooldown;
                }
            }

            RegistrarPoderesPassivosImediatos();
        }

        private void RegistrarPoderesPassivosImediatos()
        {
            // Aplica os bônus passivos permanentes modificando os status finais da torre
            if (_dadosInstancia.ID_PoderPassivo == "AuraArdente") _danoFinal *= 1.15f;
            if (_dadosInstancia.ID_PoderPassivo == "VelocidadeExtrema") _cadenciaFinal *= 1.30f;
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            _tiroCooldownTimer -= dt;
            _habilidadeCooldownTimer -= dt;
            _scanTimer += dt;

            // 6.2 Otimização profunda de Varredura de Alvos por intervalo regulado
            if (_scanTimer >= 0.15s) // Config por ScriptableObject
            {
                _scanTimer = 0f;
                FindTarget();
            }

            // Fluxo de Combate Ativo
            if (_alvoAtual != null)
            {
                if (_tiroCooldownTimer <= 0)
                {
                    Fire();
                    _tiroCooldownTimer = 1f / _cadenciaFinal;
                }

                if (_habilidadeCooldownTimer <= 0 && _dadosInstancia.ID_PoderAtivo != "Nenhum")
                {
                    AbilitySystem.Instance.AtivarPoder(_dadosInstancia.ID_PoderAtivo, _parametrosHabilidadeEscalados, _alvoAtual, this);
                    AbilityData dataAtivo = AbilitySystem.Instance.GetAbilityData(_dadosInstancia.ID_PoderAtivo);
                    _habilidadeCooldownTimer = dataAtivo != null ? dataAtivo.cooldown : 10f;
                }
            }
        }

        private void FindTarget()
        {
            // Valida o cache do alvo para evitar verificações desnecessárias
            if (_alvoAtual != null && _alvoAtual.estaMorto) _alvoAtual = null;

            List<EnemyController> candidatos = EnemyManager.Instance.QueryInRadius(transform.position, _raioFinal);
            if (candidatos.Count == 0)
            {
                _alvoAtual = null;
                return;
            }

            // Implementação de Estratégias de Escolha baseadas no Tipo de IA da criatura
            _alvoAtual = _dadosEspecieConfig.estrategiaAlvoPadrao switch
            {
                TargetStrategy.MaisProximo => ObterMaisProximo(candidatos),
                TargetStrategy.MaiorVida => ObterMaiorVida(candidatos),
                _ => candidatos[0]
            };
        }

        private EnemyController ObterMaisProximo(List<EnemyController> list)
        {
            EnemyController maisProximo = null;
            float menorDistSq = float.MaxValue;
            foreach (var e in list)
            {
                float distSq = (e.transform.position - transform.position).sqrMagnitude;
                if (distSq < menorDistSq)
                {
                    menorDistSq = distSq;
                    maisProximo = e;
                }
            }
            return maisProximo;
        }

        private EnemyController ObterMaiorVida(List<EnemyController> list)
        {
            EnemyController maiorHp = list[0];
            foreach (var e in list)
            {
                if (e.vidaAtual > maiorHp.vidaAtual) maiorHp = e;
            }
            return maiorHp;
        }

        private void Fire()
        {
            if (_dadosEspecieConfig.prefabProjetil == null) return;
            
            // Otimização recomendada: Utilizar Object Pooling para os projéteis reais
            GameObject projGO = Instantiate(_dadosEspecieConfig.prefabProjetil, transform.position, Quaternion.identity);
            Projectile scriptProj = projGO.GetComponent<Projectile>();
            scriptProj.Init(_alvoAtual, _danoFinal, _dadosInstancia.Elemento);
        }
    }
}