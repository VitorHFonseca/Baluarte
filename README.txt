========================================================================================================================
[ FASE 1: CLICKER / CAPTURA ]
========================================================================================================================
 Clique do Jogador 
        │
        ▼
 1. BiomeConfig.Sortear() ──────> [WeightedRandom] ──> Retorna: ID_Especie (Raridade, Elemento)
        │
        ▼
 2. IV_Generator.GenerateIVs() ──> [Seed RNG] ───────> Calcula Multiplicadores de Stats Baseados no Elemento (0.85 a 1.15)
        │
        ▼
 3. AbilitySystem.SortearPoderes() ──────────────────> Filtra e sorteia 1 Ativo + 1 Passivo do Pool do Elemento
        │
        ▼
 4. Instanciação de Dados ───────────────────────────> Cria CreatureInstance (Gera GUID único)
        │
        ▼
 5. CaptureSession ──────────────────────────────────> Adiciona ao SessionInventory se a Resistência for zerada a tempo
        │
        ▼
 Termina Tempo / Energia ────────────────────────────> Congela o SessionInventory e dispara OnSessionEnd

========================================================================================================================
[ FASE 2: TELA DE TRIAGEM & PERSISTÊNCIA ]
========================================================================================================================
 6. GameManager.ChangeState(TriageState)
        │
        ├─> Exibe UI Cards (Compara Índice de Potência [IP] e Destaque Âmbar se IV > 1.12)
        ├─> Jogador descarta criaturas indesejadas ──> Transforma em Fragmentos / Moedas
        └─> Jogador marca SelecionadoParaDefesa = true (Máx N criaturas)
        │
        ▼
 7. GameManager.ChangeState(DefenseState) ───────────> Injeta o inventário filtrado no contexto de combate

========================================================================================================================
[ FASE 3: TOWER DEFENSE BASEADO EM SESSÃO ]
========================================================================================================================
 Jogador clica no Tile Válido
        │
        ▼
 8. TowerSpawner ────────────────────────────────────> Consome CustoInvocacao, instancia Prefab_Torre
        │
        ▼
 9. TowerBrain.Init(CreatureInstance) ───────────────> Cacheia modificadores, registra Poder Passivo imediatamente
        │
        ▼
10. Loop de Simulação de Combate (A cada Frame / Ciclo de Scan O(1))
        │
        ├──> ScanInterval (0.15s) ──> EnemyManager.QueryInRadius() (Spatial Hash) ──> Define Alvo (Estratégia de Alvo)
        │
        ├──> Se Cooldown Tiro <= 0 ─> Executa Fire() ──> Instancia Projétil do Pool (Aplica Dano + Efeito Base do Elemento)
        │
        └──> Se Cooldown Ativo <= 0 ─> Executa AtivarPoder() ──> Dispara IAbilityEffect (Ex: Chain Lightning / Poison Cloud)
        │
        ▼
 Inimigo Limpo / Fim da Wave ────────────────────────> Armazena Moedas/Dados ──> Salva JSON ──> Retorna ao Menu
========================================================================================================================