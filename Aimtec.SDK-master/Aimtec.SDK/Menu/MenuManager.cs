namespace Aimtec.SDK.Menu
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;

    using Aimtec.SDK.Menu.Theme;
    using Aimtec.SDK.Menu.Theme.Default;
    using Aimtec.SDK.Util;

    internal class MenuManager : Menu
    {
        #region Fields

        private bool visible;

        #endregion

        #region Constructors and Destructors

        private MenuManager()
            : base("AimtecSDK-RootMenu", string.Empty, true)
        {
            //Sets the base menu position
            this.Position = new Vector2(10, 10);

            if (!Directory.Exists(this.AppDataConfigPath))
            {
                Directory.CreateDirectory(this.AppDataConfigPath);
            }

            if (!Directory.Exists(this.MenuSettingsPath))
            {
                Directory.CreateDirectory(this.MenuSettingsPath);
            }

            Aimtec.Render.OnPresent += () => this.Render(this.Position);
            Game.OnWndProc += args => this.WndProc(args.Message, args.WParam, args.LParam);

            Game.OnEnd += delegate { this.Save(); };
            AppDomain.CurrentDomain.DomainUnload += delegate { this.Save(); };
            AppDomain.CurrentDomain.ProcessExit += delegate { this.Save(); };

            // TODO: Make this load from settings
            this.Theme = new DefaultMenuTheme();
        }

        #endregion

        #region Public Properties

        public static MenuManager Instance { get; } = new MenuManager();

        public MenuTheme Theme { get; set; }

        #endregion

        #region Properties

        internal override bool IsMenu { get; } = false;

        internal override bool Visible
        {
            get
            {
                return this.visible;
            }
            set
            {
                foreach (var child in this.Menus)
                {
                    child.Visible = value;
                }

                this.visible = value;
            }
        }

        internal static float LastMouseMoveTime { get; set; }

        internal static Point LastMousePosition { get; set; }

        internal string AppDataConfigPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Aimtec.SDK");

        internal IReadOnlyList<MenuComponent> Menus => this.Children.Values.Where(x => x.IsMenu).ToList();

        internal string MenuSettingsPath => Path.Combine(this.AppDataConfigPath, "MenuSettings");

        internal string SharedSettingsPath => Path.Combine(this.MenuSettingsPath, "SharedConfig");

        #endregion

        #region Public Methods and Operators

        public new Menu Add(MenuComponent mc)
        {
            var menu = mc as Menu;
            if (menu != null)
            {
                if (menu.Root && menu.RootMenuKey != null)
                {
                    if (this.Children.ContainsKey(menu.RootMenuKey))
                    {
                        throw new Exception($"The menu {menu.InternalName} already exists.");
                    }

                    menu.Parent = this;

                    this.Children.Add(menu.RootMenuKey, menu);

                    this.UpdateWidth();
                }
            }

            return menu;
        }

        public new Menu Attach()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Methods

        internal override Rectangle GetBounds(Vector2 pos)
        {
            return new Rectangle((int)pos.X, (int)pos.Y, this.Width, this.Theme.MenuHeight * this.Menus.Count);
        }

        internal override void Render(Vector2 pos)
        {
            if (!this.Visible)
            {
                return;
            }

            for (var i = 0; i < this.Menus.Count; i++)
            {
                var position = this.Position + new Vector2(0, i * this.Theme.MenuHeight);
                this.Menus[i].Position = position;
                this.Menus[i].Render(this.Menus[i].Position);
            }
        }

        internal override void WndProc(uint message, uint wparam, int lparam)
        {
            // Drag menu
            if (message == (int)WindowsMessages.WM_KEYDOWN && wparam == (ulong)KeyCode.ShiftKey)
            {
                //Console.WriteLine("visible?? key = {0}", (Keys) wparam);
                this.Visible = true;
            }

            if (message == (int)WindowsMessages.WM_KEYUP && wparam == (ulong)KeyCode.ShiftKey)
            {
                //Console.WriteLine("not visible?? key = {0}", (Keys) wparam);
                this.Visible = false;
            }

            //Save Menu if F5 is pressed (upon reloading)
            if (message == (int)WindowsMessages.WM_KEYUP && wparam == (ulong)KeyCode.F5)
            {
                this.Save();
            }

            foreach (var menu in this.Menus)
            {
                menu.BaseWndProc(message, wparam, lparam);
            }
        }


        internal override void Save()
        {
            foreach (var m in this.Menus)
            {
                m.Save();
            }
        }

        internal override void UpdateWidth()
        {
            var maxWidth = this.Children.Values.Max(x => MiscUtils.MeasureTextWidth(x.DisplayName));
            this.Width = (int) (maxWidth + Instance.Theme.BaseMenuWidth);
        }

        #endregion
    }
}