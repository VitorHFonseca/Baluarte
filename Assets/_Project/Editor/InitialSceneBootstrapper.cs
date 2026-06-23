using GameCore.Simulation;
using GameCore.Systems;
using GameCore.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameCore.EditorTools
{
    public static class InitialSceneBootstrapper
    {
        private const string SceneFolder = "Assets/_Project/Scenes";
        private const string ScenePath = SceneFolder + "/CenaInicial.unity";

        [MenuItem("Baluarte/Cenas/Criar Cena Inicial")]
        public static void CreateInitialScene()
        {
            EnsureFolder(SceneFolder);
            TestDataBootstrapper.CreateOrUpdate();

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "CenaInicial";

            CreateManager<GameManager>("GameManager");
            CreateManager<PlayerProfileService>("PlayerProfileService");
            CreateManager<CaptureSession>("CaptureSession");
            AbilitySystem abilitySystem = CreateManager<AbilitySystem>("AbilitySystem");
            CreateManager<EnemyManager>("EnemyManager");
            TowerDefenseEngine towerDefenseEngine = CreateManager<TowerDefenseEngine>("TowerDefenseEngine");
            CreateManager<MainMenuController>("MainMenuController");
            AssignGeneratedData(abilitySystem, towerDefenseEngine);

            GameObject camera = new GameObject("Main Camera");
            camera.tag = "MainCamera";
            Camera cameraComponent = camera.AddComponent<Camera>();
            cameraComponent.clearFlags = CameraClearFlags.SolidColor;
            cameraComponent.backgroundColor = new Color(0.09f, 0.06f, 0.04f);
            camera.transform.position = new Vector3(0f, 3f, -8f);
            camera.transform.rotation = Quaternion.Euler(18f, 0f, 0f);

            EditorSceneManager.SaveScene(scene, ScenePath);
            Debug.Log($"Cena inicial criada em {ScenePath}.");
        }

        private static T CreateManager<T>(string name) where T : Component
        {
            GameObject go = new GameObject(name);
            return go.AddComponent<T>();
        }

        private static void AssignGeneratedData(AbilitySystem abilitySystem, TowerDefenseEngine towerDefenseEngine)
        {
            SerializedObject abilityObject = new SerializedObject(abilitySystem);
            SerializedProperty abilityList = abilityObject.FindProperty("bancoDeDadosHabilidades");
            abilityList.arraySize = 2;
            abilityList.GetArrayElementAtIndex(0).objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Object>("Assets/_Project/ScriptableObjects/Habilidades/Habilidade_BolaDeFogo.asset");
            abilityList.GetArrayElementAtIndex(1).objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Object>("Assets/_Project/ScriptableObjects/Habilidades/Habilidade_AuraArdente.asset");
            abilityObject.ApplyModifiedPropertiesWithoutUndo();

            towerDefenseEngine.configTD =
                AssetDatabase.LoadAssetAtPath<TowerDefenseConfig>("Assets/_Project/ScriptableObjects/TD/ConfigTD_VerticalSlice.asset");
            EditorUtility.SetDirty(towerDefenseEngine);
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
    }
}
