namespace Aimtec.SDK.Menu
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;

    using Aimtec.SDK.Menu.Components;
    using Aimtec.SDK.Menu.Theme;
    using Aimtec.SDK.Util;

    /// <summary>
    ///     Class Menu.
    /// </summary>
    /// <seealso cref="Aimtec.SDK.Menu.IMenu" />
    /// <seealso cref="System.Collections.IEnumerable" />
    public class Menu : MenuComponent, IMenu, IEnumerable
    {
        #region Fields

        /// <summary>
        ///     The toggled
        /// </summary>
        private bool toggled;

        /// <summary>
        ///     The visible
        /// </summary>
        private bool visible;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Menu" /> class.
        /// </summary>
        /// <param name="internalName">Name of the internal.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="isRoot">Whether this Menu is a root menu</param>
        public Menu(string internalName, string displayName, bool isRoot = false)
        {
            var callingAssembly = Assembly.GetCallingAssembly();
            var GUID = (GuidAttribute) Attribute.GetCustomAttribute(callingAssembly, typeof(GuidAttribute));
            var assemblyGuidShort = GUID.Value.Substring(0, 5);
            var assemblyName = Assembly.GetCallingAssembly().GetName().Name;

            this.Root = isRoot;

            this.AssemblyConfigDirectoryName = $"{assemblyName}.{assemblyGuidShort}";

            this.InternalName = internalName;

            if (this.Root)
            {
                this.RootMenuKey = $"{internalName}.{assemblyGuidShort}";
            }

            this.DisplayName = displayName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether this instance is a menu.
        /// </summary>
        /// <value><c>true</c> if this instance is a menu; otherwise, <c>false</c>.</value>
        internal override bool IsMenu => true;

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="IMenuComponent" /> is toggled.
        /// </summary>
        /// <value><c>true</c> if toggled; otherwise, <c>false</c>.</value>
        internal override bool Toggled
        {
            get => this.toggled;
            set
            {
                this.toggled = value;

                foreach (var child in this.Children.Values)
                {
                    child.Visible = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="IMenuComponent" /> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        internal override bool Visible
        {
            get => this.visible;
            set
            {
                this.visible = value;

                if (this.Toggled)
                {
                    foreach (var child in this.Children.Values)
                    {
                        child.Visible = value;
                    }
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     The internal name of this Menu - adjusted with GUID so it is unique per assembly.
        ///     This is to allow using the same internal name for root menus across different assemblies.
        /// </summary>
        internal string RootMenuKey { get; set; }

        internal override string Serialized { get; }

        internal int Width { get; set; }

        #endregion

        #region Public Indexers

        /// <inheritdoc />
        public override IMenuComponent this[string name] => this.GetItem(name);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the specified identifier.
        /// </summary>
        /// <param name="menuComponent">The menu.</param>
        /// <returns>IMenu.</returns>
        public Menu Add(MenuComponent menuComponent)
        {
            if (menuComponent != null)
            {
                if (menuComponent.Root)
                {
                    throw new Exception("You cannot add a root menu to another menu.");
                }

                //Don't try to add this item if we already have an existing item with the same name
                if (this.Children.ContainsKey(menuComponent.InternalName))
                {
                    Console.WriteLine(
                        $"The Menu {this.InternalName} already contains a child with the name {menuComponent.InternalName}");
                    return this;
                }

                //Set this menu instance as its parent
                menuComponent.Parent = this;

                this.Children.Add(menuComponent.InternalName, menuComponent);

                //If this instance's root menu is null then it has not been attached and its settings can not be loaded
                if (this.RootMenu != null)
                {
                    menuComponent.LoadValue();
                }

                this.UpdateWidth();
            }

            return this;
        }

        /// <summary>
        ///     Attaches this instance.
        /// </summary>
        /// <returns>IMenu.</returns>
        public Menu Attach()
        {
            if (!this.Root)
            {
                throw new Exception(
                    $"You can only attach a Root Menu. If this is supposed to be your root menu, set isRoot to true in the constructor.");
            }

            this.LoadValue();

            MenuManager.Instance.Add(this);

            return this;
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        public IEnumerator GetEnumerator()
        {
            return this.Children.GetEnumerator();
        }

        /// <summary>
        ///     Gets the render manager.
        /// </summary>
        /// <returns>IRenderManager.</returns>
        internal override IRenderManager GetRenderManager()
        {
            return MenuManager.Instance.Theme.BuildMenuRenderer(this);
        }

        /// <summary>
        ///     Renders the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        internal override void Render(Vector2 position)
        {
            if (this.Visible)
            {
                this.GetRenderManager().Render(position);
            }

            for (var i = 0; i < this.Children.Values.Count; i++)
            {
                var child = this.Children.Values.ToList()[i];
                child.Position = position + new Vector2(this.Parent.Width, i * MenuManager.Instance.Theme.MenuHeight);
                child.Render(child.Position);
            }
        }

        /// <summary>
        ///     Sets this menu instance to true will make it shared resulting in all its children becoming shared.
        /// </summary>
        public Menu SetShared(bool value)
        {
            this.Shared = value;

            foreach (var item in this.Children.Values)
            {
                item.Shared = true;
            }

            return this;
        }

        /// <summary>
        ///     An application-defined function that processes messages sent to a window.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="wparam">Additional message information.</param>
        /// <param name="lparam">Additional message information.</param>
        internal override void WndProc(uint message, uint wparam, int lparam)
        {
            if (this.Visible && message == (ulong) WindowsMessages.WM_LBUTTONUP)
            {
                var x = lparam & 0xffff;
                var y = lparam >> 16;

                if (this.GetBounds(this.Position).Contains(x, y))
                {
                    this.Toggled = !this.Toggled;

                    foreach (var m in this.Parent.Children.Values.Where(
                        z => z.IsMenu && z.InternalName != this.InternalName))
                    {
                        m.Toggled = false;
                    }
                }
            }

            // Pass message to children
            foreach (var child in this.Children.Values)
            {
                child.BaseWndProc(message, wparam, lparam);
            }
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        internal IMenuComponent GetItem(string name, bool showLog = true)
        {
            this.Children.TryGetValue(name, out var ritem);

            if (ritem != null)
            {
                return ritem;
            }

            foreach (var item in this.Children.Values)
            {
                var asmenu = item as Menu;

                if (asmenu == null)
                {
                    continue;
                }

                ritem = (MenuComponent) asmenu.GetItem(name, false);

                if (ritem != null)
                {
                    return ritem;
                }
            }

            if (showLog)
            {
                this.Logger.Warn("[Menu] Item: {0} was not found in the menu: {1}", name, this.InternalName);
            }

            return null;
        }

        internal override void LoadValue()
        {
            //Load the saved value if applicable
            foreach (var item in this.Children.Values)
            {
                item.LoadValue();
            }
        }

        internal override void Save()
        {
            if (!Directory.Exists(this.ConfigBaseFolder))
            {
                Directory.CreateDirectory(this.ConfigBaseFolder);
            }

            foreach (var item in this.Children.Values)
            {
                item.Save();
            }
        }

        internal virtual void UpdateWidth()
        {
            var children = this.Children.Values;

            var maxWidth = 0;

            foreach (var child in children)
            {
                var width = 0;

                if (child is MenuList)
                {
                    var mList = child as MenuList;
                    var longestItem = mList.Items.OrderByDescending(x => x.Length).FirstOrDefault();
                    if (longestItem != null)
                    {
                        width = (int)MiscUtils.MeasureTextWidth(mList.DisplayName + longestItem)
                            + MenuManager.Instance.Theme.IndicatorWidth + 15;
                    }
                }

                else if (child is MenuKeyBind)
                {
                    var kb = child as MenuKeyBind;
                    width = (int) MiscUtils.MeasureTextWidth(kb.DisplayName + "PRESS KEY");
                }

                else if (child is MenuSlider)
                {
                    var slider = child as MenuSlider;
                    width = (int)MiscUtils.MeasureTextWidth(child.DisplayName + slider.MaxValue.ToString());
                }

                else if (child is MenuSliderBool)
                {
                    var slider = child as MenuSliderBool;
                    width = (int)MiscUtils.MeasureTextWidth(child.DisplayName + slider.MaxValue.ToString());
                }

                else
                {
                    width = (int)MiscUtils.MeasureTextWidth(child.DisplayName);
                }

                if (width > maxWidth)
                {
                    maxWidth = width;
                }
            }

            this.Width = (int) (maxWidth + MenuManager.Instance.Theme.BaseMenuWidth);
        }

        #endregion
    }
}