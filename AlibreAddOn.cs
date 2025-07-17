#nullable enable // Enable nullable context for better code analysis

using AlibreAddOn;
using AlibreX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Controls.Ribbon;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace AlibreAddOnAssembly
{
    public static class AlibreAddOn
    {
        private static IADRoot? AlibreRoot { get; set; }
        private static AddOnRibbon? _addon;

        public static void AddOnLoad(IntPtr hwnd, IAutomationHook pAutomationHook, IntPtr _)
        {
            if (Environment.GetEnvironmentVariable("ALIBRE_DISABLE_SKIN") == "1") return;

            AlibreRoot = (IADRoot)pAutomationHook.Root;
            _addon = new AddOnRibbon(AlibreRoot);

            NativeUiHider.SetNativeUiVisibility(hwnd, false);
            WpfOverlay.Attach(hwnd, _addon);
        }

        public static void AddOnUnload(IntPtr hwnd, bool _, ref bool __, int ___, int ____)
        {
            WpfOverlay.Detach();
            NativeUiHider.SetNativeUiVisibility(hwnd, true);

            _addon = null;
            AlibreRoot = null;
        }

        public static IAlibreAddOn? GetAddOnInterface() => _addon;
        public static IADRoot? GetRoot() => AlibreRoot;
        public static void AddOnInvoke(IntPtr _, IntPtr __, string ___, bool ____, int _____, int ______) { }
    }

    public class AddOnRibbon : IAlibreAddOn
    {
        private readonly MenuManager _mgr;
        public IADSession Session { get; }

        public AddOnRibbon(IADRoot root)
        {
            Session = root.TopmostSession;
            _mgr = new MenuManager();
        }

        public int RootMenuItem => _mgr.Root.Id;
        public IAlibreAddOnCommand? InvokeCommand(int id, string _) => _mgr[id]?.Cmd?.Invoke(Session);
        public bool HasSubMenus(int id) => _mgr[id]?.Subs.Count > 0;
        public Array SubMenuItems(int id) => _mgr[id]?.Subs.Select(s => s.Id).ToArray() ?? Array.Empty<int>();
        public string? MenuItemText(int id) => _mgr[id]?.Text;
        public ADDONMenuStates MenuItemState(int _, string __) => ADDONMenuStates.ADDON_MENU_ENABLED;
        public string? MenuItemToolTip(int id) => _mgr[id]?.Tip;
        public string? MenuIcon(int id) => _mgr[id]?.Icon;
        public bool PopupMenu(int _) => false;
        public bool HasPersistentDataToSave(string _) => false;
        public void SaveData(System.Runtime.InteropServices.ComTypes.IStream _, string __) { }
        public void LoadData(System.Runtime.InteropServices.ComTypes.IStream _, string __) { }

        public bool UseDedicatedRibbonTab() => false;
        public void setIsAddOnLicensed(bool _) { }

        public void LoadData(global::AlibreAddOn.IStream pCustomData, string sessionIdentifier)
        {
            throw new NotImplementedException();
        }
        public void SaveData(global::AlibreAddOn.IStream pCustomData, string sessionIdentifier)
        {
            throw new NotImplementedException();
        }
        public MenuManager Manager => _mgr;
    }

    #region Menu Data
    public class MenuItemData(int id, string text, string? tip = null, string? icon = null, Func<IADSession, IAlibreAddOnCommand>? cmd = null)
    {
        public int Id { get; } = id;
        public string Text { get; } = text;
        public string? Tip { get; } = tip;
        public string? Icon { get; } = icon;
        public Func<IADSession, IAlibreAddOnCommand>? Cmd { get; } = cmd;
        public List<MenuItemData> Subs { get; } = [];
    }

    public class MenuManager
    {
        private readonly Dictionary<int, MenuItemData> _dict = [];
        public MenuItemData Root { get; }

        public MenuManager()
        {
            // Initialize the root menu item
            Root = new MenuItemData(401, "HOOPS Commands");

            // Add commands 1 through 9 as sub-items to the root
            Root.Subs.AddRange(new[]
            {
        new MenuItemData(9021, "cmd1", "cmd1", null, s => new cmd1()),
        new MenuItemData(9022, "cmd2", "cmd2", null, s => new cmd2()),
        new MenuItemData(9023, "cmd3", "cmd3", null, s => new cmd3()),
        new MenuItemData(9024, "cmd4", "cmd4", null, s => new cmd4()),
        new MenuItemData(9025, "cmd5", "cmd5", null, s => new cmd5()),
        new MenuItemData(9026, "cmd6", "cmd6", null, s => new cmd6()),
        new MenuItemData(9027, "cmd7", "cmd7", null, s => new cmd7()),
        new MenuItemData(9028, "cmd8", "cmd8", null, s => new cmd8()),
        new MenuItemData(9029, "cmd9", "cmd9", null, s => new cmd9())
    });

            // Register the entire menu structure
            Register(Root);
        }

        private void Register(MenuItemData m)
        {
            _dict[m.Id] = m;
            foreach (var c in m.Subs) Register(c);
        }

        public MenuItemData? this[int id] => _dict.TryGetValue(id, out var m) ? m : null;
    }
    #endregion

    #region WPF Overlay
    internal static class WpfOverlay
    {
        private static ElementHost? _host;

        public static void Attach(IntPtr parentHwnd, AddOnRibbon addon)
        {
            if (_host != null) return;
            var rib = BuildRibbon(addon);
            _host = new ElementHost
            {
                Child = rib,
                Dock = DockStyle.Top,
                Height = 150
            };
            var parent = System.Windows.Forms.Control.FromHandle(parentHwnd);
            parent?.Controls.Add(_host);
        }

        public static void Detach()
        {
            if (_host == null) return;
            _host.Parent?.Controls.Remove(_host);
            _host.Dispose();
            _host = null;
        }

        private static Ribbon BuildRibbon(AddOnRibbon addon)
        {
            var ribbon = new Ribbon();
            var tab = new RibbonTab { Header = "Sample" };
            ribbon.Items.Add(tab);
            var panel = new RibbonGroup { Header = "Commands" };
            tab.Items.Add(panel);
            var btn = new RibbonButton { Label = "Draw", LargeImageSource = null };
            btn.Click += (_, __) =>
            {
                var cmd = addon.Manager.Root.Subs[0].Cmd?.Invoke(addon.Session);
                cmd?.OnShowUI(0);
            };
            panel.Items.Add(btn);
            return ribbon;
        }
    }
    #endregion

    #region Native UI hider
    internal static class NativeUiHider
    {
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        private delegate bool EnumProc(IntPtr hWnd, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumChildWindows(IntPtr hWndParent, EnumProc lpEnumFunc, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static void SetNativeUiVisibility(IntPtr main, bool show)
        {
            EnumChildWindows(main, (h, _) =>
            {
                var sb = new StringBuilder(256);
                GetClassName(h, sb, sb.Capacity);
                string cls = sb.ToString();
                if (cls.Contains("RibbonBar") || cls.Contains("StatusBar") || cls.Contains("MenuBar"))
                {
                    ShowWindow(h, show ? SW_SHOW : SW_HIDE);
                }
                return true;
            }, IntPtr.Zero);
        }
    }
    #endregion

    #region Dummy Command
    public class DummyCommand(IADSession session) : IAlibreAddOnCommand
    {
        private readonly IADSession _session = session;

        public void OnShowUI(long _) => System.Windows.MessageBox.Show($"Dummy command executed in session: {_session.Name}");
        public bool AddTab() => false;
        public string? TabName => null;
        public Array? Extents => null;
        public IADAddOnCommandSite? CommandSite { get; set; }
        public void On3DRender() { }
        public void OnRender(int _, int __, int ___, int ____, int _____) { }
        public void OnComplete() { }
        public void OnTerminate() { }
        public bool IsTwoWayToggle() => false;
        public bool OnClick(int _, int __, ADDONMouseButtons ___) => false;
        public bool OnDoubleClick(int _, int __) => false;
        public bool OnMouseDown(int _, int __, ADDONMouseButtons ___) => false;
        public bool OnMouseMove(int _, int __, ADDONMouseButtons ___) => false;
        public bool OnMouseUp(int _, int __, ADDONMouseButtons ___) => false;
        public void OnSelectionChange() { }
        public bool OnKeyDown(int _) => false;
        public bool OnKeyUp(int _) => false;
        public bool OnEscape() => false;
        public bool OnMouseWheel(double _) => false;
    }
    #endregion
}