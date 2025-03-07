using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.DebugPanel
{
    public class DebugPanelManager : MonoBehaviour
    {
        // Start is called before the first frame update
        public UIDocument UIDInstance;
        [SerializeField] private VisualTreeAsset myTreeAssetMember;
        [SerializeField] private PanelSettings myPanelSetting;
        private VisualElement debugPanelElement;
        private GameObject uiDocumentObject;
        public static DebugPanelManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                GenrateUIElements();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F10)) ToggleDebugPanel();
        }

        private void GenrateUIElements()
        {
            if (UIDInstance == null)
            {
                uiDocumentObject = new GameObject("UIDocument");
                UIDInstance = uiDocumentObject.AddComponent<UIDocument>();

                UIDInstance.visualTreeAsset = myTreeAssetMember;
                UIDInstance.panelSettings = myPanelSetting;

                DontDestroyOnLoad(uiDocumentObject);

                uiDocumentObject.SetActive(false);
            }
        }

        public void ToggleDebugPanel()
        {
            if (uiDocumentObject != null)
            {
                var isActive = uiDocumentObject.activeSelf;
                uiDocumentObject.SetActive(!isActive);
                if (!isActive)
                    MapMethodsToButtons();
            }
        }

        [DebugPanelAttribute("Refresh lists", "Misc")]
        public void MapMethodsToButtons()
        {
            // Trouver tous les composants dans la scène
            Component[] allComponents = FindObjectsOfType<Component>();

            // Collecter toutes les méthodes publiques sans paramètres et annotés
            var methods = allComponents
                .SelectMany(component => component.GetType()
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(m => !m.IsSpecialName
                                && m.GetParameters().Length == 0
                                && IsUserDefinedType(component.GetType()) // Filtres sur les workspaces
                                && m.IsDefined(
                                    typeof(DebugPanelAttribute))) // Vérifier si la méthode contient l'atribute de debug
                    .Select(m => new
                    {
                        Method = m,
                        Component = component,
                        Attribute = (DebugPanelAttribute)m.GetCustomAttribute(typeof(DebugPanelAttribute))
                    })
                )
                .ToList();

            Debug.Log($"Found {methods.Count} methods");
            //Déclarer les conteneurs de bouttons
            VisualElement uiRoot = UIDInstance.rootVisualElement;
            if (uiRoot == null)
            {
                Debug.LogError("rootVisualElement is null.");
                return;
            }

            VisualElement ActionButtonCont = uiRoot.Q("ActionCmdList");
            VisualElement BaseButtonCont = uiRoot.Q("BaseCmdList");
            VisualElement MiscButtonCont = uiRoot.Q("MiscCmdList");
            //supprimer tout les bouttons déja contenus
            ActionButtonCont.Clear();
            BaseButtonCont.Clear();
            MiscButtonCont.Clear();
            // Créer des boutons pour chaque méthode et les ajouter au conteneur
            foreach (var methodInfo in methods)
            {
                Button button = new();
                button.AddToClassList("btnCmd");
                // Définir le text du boutton a la description de l'annotation
                button.text = methodInfo.Attribute.Description;
                var category = methodInfo.Attribute.Category;
                button.clicked += () =>
                {
                    Component targetComponent =
                        FindObjectsOfType(methodInfo.Method.DeclaringType).FirstOrDefault() as Component;
                    if (targetComponent != null)
                        methodInfo.Method.Invoke(targetComponent, null);
                    else
                        Debug.LogWarning($"No instance of component {methodInfo.Method.DeclaringType.Name} found.");
                };
                switch (category.ToLower())
                {
                    case "action":
                        ActionButtonCont.Add(button);
                        break;
                    case "base":
                        BaseButtonCont.Add(button);
                        break;
                    default:
                        MiscButtonCont.Add(button);
                        break;
                }
                // Ajouter le bouton au conteneur

                button.MarkDirtyRepaint();
                if (ActionButtonCont.parent == null)
                    Debug.LogError("ActionButtonCont is not added to the UI hierarchy.");
            }
        }

        private bool IsUserDefinedType(Type type)
        {
            // Vérifiez si le type est défini dans un assembly qui n'est pas un assembly de système
            if (type.IsDefined(typeof(CompilerGeneratedAttribute))) return false;

            // Vérifiez les namespaces pour exclure les types système et Unity
            if (type.Namespace != null && (type.Namespace.StartsWith("UnityEngine")
                                           || type.Namespace.StartsWith("System")
                                           || type.Namespace
                                               .StartsWith("TMPro"))) // Correction pour TextMeshPro namespace
                return false;

            return true;
        }
    }
}