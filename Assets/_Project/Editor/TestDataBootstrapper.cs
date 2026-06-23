using System.Collections.Generic;
using System.IO;
using GameCore.Data;
using GameCore.Simulation;
using UnityEditor;
using UnityEngine;

namespace GameCore.EditorTools
{
    public static class TestDataBootstrapper
    {
        private const string CreatureFolder = "Assets/_Project/ScriptableObjects/Creatures";
        private const string AbilityFolder = "Assets/_Project/ScriptableObjects/Habilidades";
        private const string ConfigFolder = "Assets/_Project/ScriptableObjects/TD";
        private const string ProjectileFolder = "Assets/_Project/Prefabs/Projectiles";
        private const string EnemyFolder = "Assets/_Project/Prefabs/Enemies";

        [MenuItem("Baluarte/Dados de Teste/Criar ou Atualizar")]
        public static void CreateOrUpdate()
        {
            EnsureFolders();

            GameObject projectilePrefab = CreateProjectilePrefab();
            GameObject enemyPrefab = CreateEnemyPrefab();

            CreateFireCreature(projectilePrefab);
            CreateFireballAbility();
            CreateBurningAuraAbility();
            CreateTowerDefenseConfig(enemyPrefab);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Dados de teste do Baluarte criados/atualizados.");
        }

        private static void EnsureFolders()
        {
            EnsureFolder(CreatureFolder);
            EnsureFolder(AbilityFolder);
            EnsureFolder(ConfigFolder);
            EnsureFolder(ProjectileFolder);
            EnsureFolder(EnemyFolder);
        }

        private static void EnsureFolder(string folderPath)
        {
            string[] parts = folderPath.Split('/');
            string current = parts[0];

            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }
                current = next;
            }
        }

        private static void CreateFireCreature(GameObject projectilePrefab)
        {
            string path = CreatureFolder + "/Criatura_DragaoFagulha.asset";
            CreatureSpeciesSO creature = LoadOrCreate<CreatureSpeciesSO>(path);

            creature.idEspecie = "esp_dragao_fagulha";
            creature.nomeEspecie = "Dragao Fagulha";
            creature.elemento = ElementType.Fogo;
            creature.raridadeBase = RarityType.Comum;
            creature.vidaBase = 100f;
            creature.danoBase = 14f;
            creature.cadenciaDeTiroBase = 1.15f;
            creature.raioBase = 5.5f;
            creature.custoInvocacaoBase = 35;
            creature.tipoDeProjetil = ProjectileType.ProjetilUnico;
            creature.estrategiaAlvoPadrao = TargetStrategy.Primeiro;
            creature.prefabProjetil = projectilePrefab;

            EditorUtility.SetDirty(creature);
        }

        private static void CreateFireballAbility()
        {
            string path = AbilityFolder + "/Habilidade_BolaDeFogo.asset";
            AbilityData ability = LoadOrCreate<AbilityData>(path);

            ability.idPoder = "BolaDeFogo";
            ability.nome = "Bola de Fogo";
            ability.descricao = "Causa dano direto de fogo ao alvo atual.";
            ability.tipo = AbilityType.Ativo;
            ability.elemento = ElementType.Fogo;
            ability.cooldown = 4f;
            ability.chavesParametros = new List<string> { "dano" };
            ability.valoresPadrao = new List<float> { 28f };

            EditorUtility.SetDirty(ability);
        }

        private static void CreateBurningAuraAbility()
        {
            string path = AbilityFolder + "/Habilidade_AuraArdente.asset";
            AbilityData ability = LoadOrCreate<AbilityData>(path);

            ability.idPoder = "AuraArdente";
            ability.nome = "Aura Ardente";
            ability.descricao = "Aumenta o dano final da torre em 15%.";
            ability.tipo = AbilityType.Passivo;
            ability.elemento = ElementType.Fogo;
            ability.cooldown = 0f;
            ability.chavesParametros = new List<string>();
            ability.valoresPadrao = new List<float>();

            EditorUtility.SetDirty(ability);
        }

        private static void CreateTowerDefenseConfig(GameObject enemyPrefab)
        {
            string path = ConfigFolder + "/ConfigTD_VerticalSlice.asset";
            TowerDefenseConfig config = LoadOrCreate<TowerDefenseConfig>(path);

            config.scanIntervalGlobal = 0.15f;
            config.maxTorresNoMapa = 6;
            config.waves = new[]
            {
                new WaveData
                {
                    quantidadeInimigos = 8,
                    spawnInterval = 0.85f,
                    prefabInimigo = enemyPrefab
                }
            };

            EditorUtility.SetDirty(config);
        }

        private static GameObject CreateProjectilePrefab()
        {
            string path = ProjectileFolder + "/Prefab_ProjetilFogoTeste.prefab";
            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null) return existing;

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "Prefab_ProjetilFogoTeste";
            go.transform.localScale = Vector3.one * 0.25f;
            UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
            go.AddComponent<Projectile>();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            UnityEngine.Object.DestroyImmediate(go);
            return prefab;
        }

        private static GameObject CreateEnemyPrefab()
        {
            string path = EnemyFolder + "/Prefab_InimigoTeste.prefab";
            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null) return existing;

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "Prefab_InimigoTeste";
            go.transform.localScale = new Vector3(0.75f, 1f, 0.75f);
            go.AddComponent<EnemyController>();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            UnityEngine.Object.DestroyImmediate(go);
            return prefab;
        }

        private static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;

            EnsureFolder(Path.GetDirectoryName(path)?.Replace("\\", "/") ?? "Assets");
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }
    }
}
