using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace FrostfallSaga.Utils.UI
{
    /// <summary>
    /// Singleton class that provides a way to get icons for the UI.
    /// </summary>
    public class UIIconsProvider : MonoBehaviourPersistingSingleton<UIIconsProvider>
    {
        private static readonly string ICONS_ROOT_RESOURCES_PATH = "UI/Icons/";
        private static readonly string ICONS_SUFFIX = "Icon";

        private List<Sprite> _preloadedIcons;

        /// <summary>
        /// Preload all icons in the resources folder in memory.
        /// </summary>
        public void PreloadIcons()
        {
            _preloadedIcons = Resources.LoadAll<Sprite>(ICONS_ROOT_RESOURCES_PATH).ToList();
        }

        /// <summary>
        /// Get an icon by its name. If the icon is not preloaded, it will be loaded from the resources folder.
        /// </summary>
        /// <param name="iconName">The name of the icon's file without the suffix.</param>
        /// <returns>The loaded sprite if present in the folder, null otherwise</returns>
        public Sprite GetIcon(string iconName)
        {
            if (_preloadedIcons != null)
            {
                return _preloadedIcons.Find(icon => icon.name == iconName);
            }
            return Resources.Load<Sprite>($"{ICONS_ROOT_RESOURCES_PATH}{iconName}{ICONS_SUFFIX}");
        }
    }
}