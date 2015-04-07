using System;
using ColossalFramework.UI;
using UnityEngine;

namespace ImprovedBuildPanel
{

    public class ImprovedBuildPanel : MonoBehaviour
    {

        private static readonly string configPath = "ImprovedBuildPanelConfig.xml";
        private Configuration config;

        private void LoadConfig()
        {
            config = Configuration.Deserialize(configPath);
            if (config == null)
            {
                config = new Configuration();
                SaveConfig();
            }
        }

        private void SaveConfig()
        {
            Configuration.Serialize(configPath, config);    
        }

        private bool resizing = false;
        private Vector2 resizeHandle = Vector2.zero;
        private bool moving = false;
        private Vector2 moveHandle = Vector2.zero;

        private UIPanel[] panels;

        private static readonly string[] panelNames = new string[]
        {
            "RoadsSmallPanel",
            "RoadsMediumPanel",
            "RoadsLargePanel",
            "RoadsHighwayPanel",
            "RoadsIntersectionPanel",
            "ZoningDefaultPanel",
            "DistrictDefaultPanel",
            "ElectricityDefaultPanel",
            "WaterAndSewageDefaultPanel",
            "GarbageDefaultPanel",
            "HealthcareDefaultPanel",
            "FireDepartmentDefaultPanel",
            "PoliceDefaultPanel",
            "EducationDefaultPanel",
            "PublicTransportBusPanel",
            "PublicTransportMetroPanel",
            "PublicTransportTrainPanel",
            "PublicTransportShipPanel",
            "PublicTransportPlanePanel",
            "BeautificationParksnPlazasPanel",
            "BeautificationPathsPanel",
            "BeautificationPropsPanel",
            /*"MonumentCategory1Panel",
            "MonumentCategory2Panel",
            "MonumentCategory3Panel",
            "MonumentCategory4Panel",
            "MonumentCategory5Panel",
            "MonumentCategory6Panel",*/
            //"WondersDefaultPanel"
        };

        void UpdatePanel(UIPanel panel)
        {
            var tabContainer = panel.gameObject.transform.parent.GetComponent<UITabContainer>();

            if (!config.panelPositionSet)
            {
                config.panelPosition = tabContainer.relativePosition;
                config.panelSize = tabContainer.size;
                config.panelPositionSet = true;
            }

            var scrollablePanel = panel.Find<UIScrollablePanel>("ScrollablePanel");
            var itemCount = scrollablePanel.transform.childCount;

            tabContainer.relativePosition = config.panelPosition;
            tabContainer.size = config.panelSize;

            var groupToolStrip = tabContainer.transform.parent.GetComponent<UIPanel>()
                .Find<UITabstrip>("GroupToolstrip");
            if (groupToolStrip != null)
            {
                groupToolStrip.AlignTo(panel, UIAlignAnchor.TopLeft);
                groupToolStrip.relativePosition = new Vector3(8.0f, -20.0f, 0.0f);
                groupToolStrip.zOrder = -9999;
            }

            var scrollBar = panel.Find<UIScrollbar>("Scrollbar");
            scrollBar.autoHide = false;
            scrollBar.size = new Vector2(20.0f, tabContainer.size.y - 26.0f);
            scrollBar.orientation = UIOrientation.Vertical;
            scrollBar.isInteractive = true;
            scrollBar.isVisible = true;
            scrollBar.enabled = true;
            scrollBar.relativePosition = new Vector3(tabContainer.size.x - 20.0f - 2.0f, 0.0f, 0);
            scrollBar.incrementAmount = 10;

            try
            {
                scrollBar.Find<UIButton>("ArrowLeft").isVisible = false;
                scrollBar.Find<UIButton>("ArrowRight").isVisible = false;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            var trackSprite = scrollBar.Find<UISlicedSprite>("Track");
            UISlicedSprite thumbSprite = null;

            if (trackSprite == null)
            {
                trackSprite = scrollBar.AddUIComponent<UISlicedSprite>();
                trackSprite.name = "Track";
                trackSprite.relativePosition = Vector2.zero;
                trackSprite.autoSize = true;
                trackSprite.fillDirection = UIFillDirection.Horizontal;
                trackSprite.spriteName = "ScrollbarTrack";
                scrollBar.trackObject = trackSprite;

                thumbSprite = trackSprite.AddUIComponent<UISlicedSprite>();
                thumbSprite.name = "Thumb";
                thumbSprite.relativePosition = Vector2.zero;
                thumbSprite.fillDirection = UIFillDirection.Horizontal;
                thumbSprite.autoSize = true;
                thumbSprite.spriteName = "ScrollbarThumb";
                scrollBar.thumbObject = thumbSprite;
            }
            else
            {
                thumbSprite = trackSprite.Find<UISlicedSprite>("Thumb");
            }

            trackSprite.size = scrollBar.size;
            thumbSprite.width = trackSprite.width;

            var resizeButton = scrollBar.Find<UIButton>("ResizeButton");
            if (resizeButton == null)
            {
                resizeButton = scrollBar.AddUIComponent<UIButton>();
                resizeButton.name = "ResizeButton";
                resizeButton.size = new Vector2(24.0f, 24.0f);
                resizeButton.AlignTo(scrollBar, UIAlignAnchor.TopLeft);
                resizeButton.normalFgSprite = "buttonresize";
                resizeButton.focusedFgSprite = "buttonresize";
                resizeButton.hoveredFgSprite = "buttonresize";
                resizeButton.pressedFgSprite = "buttonresize";
                resizeButton.disabledFgSprite = "buttonresize";

                resizeButton.eventMouseHover += (component, param) =>
                {
                    resizeButton.color = Color.grey;
                };

                resizeButton.eventMouseDown += (component, param) =>
                {
                    resizeButton.color = Color.black;
                    resizing = true;
                    resizeHandle = Input.mousePosition;
                };

                resizeButton.eventMouseUp += (component, param) =>
                {
                    resizeButton.color = Color.white;
                    resizing = false;
                    resizeHandle = Vector2.zero;
                    SaveConfig();
                };
            }

            resizeButton.relativePosition = new Vector3(0.0f, scrollBar.size.y, 0.0f);

            if (scrollablePanel.name != "ImprovedScrollablePanel")
            {
                scrollablePanel.name = "ImprovedScrollablePanel";
                scrollablePanel.scrollWheelDirection = UIOrientation.Vertical;
                scrollablePanel.horizontalScrollbar = null;
                scrollablePanel.verticalScrollbar = scrollBar;
                scrollablePanel.scrollWheelAmount = 16;
                scrollablePanel.autoLayout = false;
                scrollablePanel.autoSize = false;
                scrollablePanel.relativePosition = new Vector3(2.0f, 2.0f, 0.0f);

                scrollablePanel.eventMouseWheel += (component, param) =>
                {
                    scrollablePanel.scrollPosition = new Vector2(0.0f, scrollablePanel.scrollPosition.y + -param.wheelDelta * 16.0f);
                };

                scrollBar.eventValueChanged += delegate(UIComponent component, float value)
                {
                    scrollablePanel.scrollPosition = new Vector2(0.0f, value);
                };

                scrollablePanel.eventMouseDown += (component, param) =>
                {
                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        moving = true;
                        moveHandle = Input.mousePosition;
                    }
                };

                scrollablePanel.eventMouseUp += (component, param) =>
                {
                    moving = false;
                    moveHandle = Vector2.zero;
                    SaveConfig();
                };
            }

            scrollablePanel.size = new Vector2(tabContainer.size.x - 32.0f, config.panelSize.y - 2.0f);

            if (itemCount == 0)
            {
                return;
            }
            
            float x = 0.0f;
            float y = 0.0f;
            float width = scrollablePanel.transform.GetChild(0).GetComponent<UIButton>().size.x;
            float height = scrollablePanel.transform.GetChild(0).GetComponent<UIButton>().size.y;

            for (int i = 0; i < scrollablePanel.transform.childCount; i++)
            {
                var child = scrollablePanel.transform.GetChild(i).GetComponent<UIButton>();
                child.relativePosition = new Vector3(x, y, 0.0f);
                x += width;

                if (x >= scrollablePanel.width - width)
                {
                    x = 0.0f;
                    y += height;
                }
            }
        }

        private UIPanel openPanel = null;

        void Start()
        {
            LoadConfig();

            panels = new UIPanel[panelNames.Length];
            for (int i = 0; i < panelNames.Length; i++)
            {
                try
                {
                    panels[i] = GameObject.Find(panelNames[i]).GetComponent<UIPanel>();
                }
                catch (Exception)
                {
                    Debug.LogErrorFormat("Couldn't find panel with name {0}", panelNames[i]);
                }
            }
        }

        void Update()
        {
            try
            {
                if (openPanel != null)
                {
                    if (!openPanel.isVisible)
                    {
                        openPanel = null;
                    }
                }

                if (resizing)
                {
                    Vector2 pos = Input.mousePosition;
                    var delta = pos - resizeHandle;
                    resizeHandle = pos;
                    config.panelSize += new Vector2(delta.x, -delta.y);

                    if (config.panelSize.x <= 64.0f)
                    {
                        config.panelSize = new Vector2(64.0f, config.panelSize.y);
                    }

                    if (config.panelSize.y <= 64.0f)
                    {
                        config.panelSize = new Vector2(config.panelSize.x, 64.0f);
                    }

                    openPanel = null;
                }
                else if (moving)
                {
                    Vector2 pos = Input.mousePosition;
                    var delta = pos - moveHandle;
                    moveHandle = pos;
                    config.panelPosition += new Vector2(delta.x, -delta.y);

                    var tabContainer = openPanel.gameObject.transform.parent.GetComponent<UITabContainer>();
                    tabContainer.relativePosition = config.panelPosition;

                    if (tabContainer.absolutePosition.x + tabContainer.size.x >= Screen.width)
                    {
                        tabContainer.absolutePosition = new Vector3(Screen.width - tabContainer.size.x, tabContainer.absolutePosition.y);
                    }

                    if (tabContainer.absolutePosition.y + tabContainer.size.y >= Screen.height)
                    {
                        tabContainer.absolutePosition = new Vector3(tabContainer.absolutePosition.x, Screen.height - tabContainer.size.y);
                    }

                    if (tabContainer.absolutePosition.x <= 0.0f)
                    {
                        tabContainer.absolutePosition = new Vector3(0.0f, tabContainer.absolutePosition.y);
                    }

                    if (tabContainer.absolutePosition.y <= 0.0f)
                    {
                        tabContainer.absolutePosition = new Vector3(tabContainer.absolutePosition.x, 0.0f);
                    }

                    config.panelPosition = tabContainer.relativePosition;

                    openPanel = null;
                }

                if (openPanel == null)
                {
                    foreach (var panel in panels)
                    {
                        if (panel.isVisible)
                        {
                            openPanel = panel;
                            break;
                        }
                    }

                    if (openPanel != null)
                    {
                        UpdatePanel(openPanel);
                    }
                }
                else
                {
                    openPanel.BringToFront();
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

    }

}
