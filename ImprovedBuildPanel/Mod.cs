using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace ImprovedBuildPanel
{

    public class Mod : IUserMod
    {

        public string Name
        {
            get
            {
                return "ImprovedBuildPanel";
            }
        }

        public string Description
        {
            get { return "Improved in-game build panel"; }
        }

    }

    public class ModLoad : LoadingExtensionBase
    {

        private ImprovedBuildPanel improvedBuildPanel;

        public override void OnLevelLoaded(LoadMode mode)
        {
            var uiView = GameObject.FindObjectOfType<UIView>();
            improvedBuildPanel = uiView.gameObject.AddComponent<ImprovedBuildPanel>();
        }

        public override void OnLevelUnloading()
        {
            GameObject.Destroy(improvedBuildPanel);
        }
    }

}
