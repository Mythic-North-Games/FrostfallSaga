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
        private VisualElement _debugPanelElement;
        private GameObject _uiDocumentObject;
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
                _uiDocumentObject = new GameObject("UIDocument");
                UIDInstance = _uiDocumentObject.AddComponent<UIDocument>();

                UIDInstance.visualTreeAsset = myTreeAssetMember;
                UIDInstance.panelSettings = myPanelSetting;

                DontDestroyOnLoad(_uiDocumentObject);

                _uiDocumentObject.SetActive(false);
            }
        }

        public void ToggleDebugPanel()
        {
            if (_uiDocumentObject)
            {
                bool isActive = _uiDocumentObject.activeSelf;
                _uiDocumentObject.SetActive(!isActive);
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

            VisualElement actionButtonCont = uiRoot.Q("ActionCmdList");
            VisualElement baseButtonCont = uiRoot.Q("BaseCmdList");
            VisualElement miscButtonCont = uiRoot.Q("MiscCmdList");
            //supprimer tout les bouttons déja contenus
            actionButtonCont.Clear();
            baseButtonCont.Clear();
            miscButtonCont.Clear();
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
                        actionButtonCont.Add(button);
                        break;
                    case "base":
                        baseButtonCont.Add(button);
                        break;
                    default:
                        miscButtonCont.Add(button);
                        break;
                }
                // Ajouter le bouton au conteneur

                button.MarkDirtyRepaint();
                if (actionButtonCont.parent == null)
                    Debug.LogError("ActionButtonCont is not added to the UI hierarchy.");
            }
        }

        private bool IsUserDefinedType(Type type)
        {
            // Vérifiez si le type est défini dans un assembly qui n'est pas un assembly de système
            if (type.IsDefined(typeof(CompilerGeneratedAttribute))) return false;

            // Vérifiez les namespaces pour exclure les types système et Unity
            return type.Namespace == null || (!type.Namespace.StartsWith("UnityEngine")
                                              && !type.Namespace.StartsWith("System")
                                              && !type.Namespace
                                                  .StartsWith("TMPro")); // Correction pour TextMeshPro namespace
        }
    }
}