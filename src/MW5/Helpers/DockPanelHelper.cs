﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using MW5.Plugins;
using MW5.Plugins.Concrete;
using MW5.Plugins.Enums;
using MW5.Plugins.Helpers;
using MW5.Plugins.Interfaces;
using MW5.Properties;
using MW5.Services.Helpers;
using MW5.Services.Serialization;
using MW5.UI.Docking;
using Syncfusion.Runtime.Serialization;
using Syncfusion.Windows.Forms.Tools;

namespace MW5.Helpers
{
    internal static class DockPanelHelper
    {
        private const int PanelSize = 300;

        public static void InitDocking(this ISerializableContext context)
        {
            var panels = context.DockPanels;
            panels.Lock();

            try
            {
                InitLegend(context);

                InitLocator(context);

                InitToolbox(context);
            }
            finally
            {
                panels.Unlock();
            }

            context.DockPanels.Legend.TabPosition = 0;
        }

        private static void InitLegend(ISerializableContext context)
        {
            var legendControl = context.GetDockPanelObject(DefaultDockPanel.Legend);

            var legend = context.DockPanels.Add(legendControl, DockPanelKeys.Legend, PluginIdentity.Default);
            legend.Caption = "Legend";
            legend.DockTo(null, DockPanelState.Left, PanelSize);
            legend.SetIcon(Resources.ico_legend);
        }

        private static void InitToolbox(ISerializableContext context)
        {
            var toolboxControl = context.GetDockPanelObject(DefaultDockPanel.Toolbox);

            var toolbox = context.DockPanels.Add(toolboxControl, DockPanelKeys.Toolbox, PluginIdentity.Default);
            toolbox.Caption = "GIS Toolbox";
            toolbox.DockTo(context.DockPanels.Legend, DockPanelState.Tabbed, PanelSize);
            toolbox.SetIcon(Resources.ico_tools);
        }

        private static void InitLocator(ISerializableContext context)
        {
            var toolboControl = context.GetDockPanelObject(DefaultDockPanel.Locator);

            var locator = context.DockPanels.Add(toolboControl, DockPanelKeys.Preview, PluginIdentity.Default);
            locator.Caption = "Locator";
            locator.SetIcon(Resources.ico_zoom_to_layer);
            locator.DockTo(context.DockPanels.Legend, DockPanelState.Bottom, PanelSize);

            var size = locator.Size;
            locator.Size = new Size(size.Width, 250);
        }

        public static void SaveLayout(this DockingManager dockingManager)
        {
            var sr = GetSerializer();
            dockingManager.SaveDockState(sr);
            sr.PersistNow();
        }

        private static AppStateSerializer GetSerializer()
        {
            return new AppStateSerializer(SerializeMode.XMLFile, ConfigPathHelper.GetDockingConfigPath());
        }

        public static void SerializeDockState(IAppContext context)
        {
            var panels = context.DockPanels;
            panels.Lock();

            foreach (var panel in panels)
            {
                Debug.Print(panel.Caption);
                Debug.Print("Hidden: " + panel.AutoHidden);
                Debug.Print("Visible: " + panel.Visible);
                Debug.Print("Style: " + panel.DockState);

                //bool hidden = panel.Hidden;
                //if (hidden)
                //{
                //    panel.Hidden = false;
                //}

                //bool visible = panel.Visible;
                //if (!visible)
                //{
                //    panel.Visible = true;
                //}

                var host = panel.Control.Parent as DockHost;
                if (host != null)
                {


                    var dhc = host.InternalController as DockHostController;
                    if (dhc != null)
                    {
                        DockInfo di = dhc.GetSerCurrentDI();
                        if (di != null)
                        {

                            Rectangle r;

                            if (dhc.bInAutoHide)
                            {
                                r = dhc.DINew.rcDockArea;
                            }
                            else
                            {
                                r = dhc.LayoutRect;
                            }

                            Debug.Print("Child host count: " + dhc.ChildHostCount);

                            Debug.Print("Controller name: " + di.ControlleName);
                            Debug.Print("Style: " + di.dStyle);
                            Debug.Print("x: {0}; y: {1}; w: {2}; h: {3}", r.X, r.Y, r.Width, r.Height);
                            //Debug.Print("x: {0}; y: {1}; w: {2}; h: {3}", r2.X, r2.Y, r2.Width, r2.Height);
                            Debug.Print("Priority: " + di.nPriority);
                            Debug.Print("DockIndex: " + di.nDockIndex);
                        }
                    }
                }

                //if (!visible)
                //{
                //    panel.Visible = false;
                //}

                //if (hidden)
                //{
                //    panel.Hidden = true;
                //}

                Debug.Print("--------------");
            }

            panels.Unlock();
        }
    }
}
