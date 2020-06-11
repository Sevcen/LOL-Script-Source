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

    using Newtonsoft.Json;

    using NLog;

    /// <summary>
    ///     Class MenuComponent.
    /// </summary>
    /// <seealso cref="Aimtec.SDK.Menu.IMenuComponent" />
    public abstract class MenuComponent : IMenuComponent
    {
        #region Fields

        private string _AssemblyConfigDirectoryName;

        #endregion

        #region Delegates

        /// <summary>
        /// The Value Changed Event Handler
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        public delegate void ValueChangedHandler(MenuComponent sender, ValueChangedArgs args);

        #endregion

        #region Public Indexers

        /// <inheritdoc />
        public virtual IMenuComponent this[string name] => null;

        #endregion

        #region Public Events

        /// <inheritdoc />
        public virtual event ValueChangedHandler OnValueChanged;

        #endregion

        #region Public Properties

        /// <inheritdoc />
        [JsonProperty(Order = 1)]
        public string DisplayName { get; set; }

        /// <inheritdoc />
        [JsonProperty(Order = 2)]
        public string InternalName { get; set; }

        /// <summary>
        ///     Gets a numeric value associated with MenuComponent <see cref="MenuComponent" />.
        /// </summary>
        /// <remarks>
        ///     This property will only succeed for MenuSlider, MenuSliderBool and MenuList.
        /// </remarks>
        public int Value => ((IReturns<int>)this).Value;

        /// <inheritdoc />
        public bool Enabled => ((IReturns<bool>) this).Value;

        /// <inheritdoc />
        public Menu Parent { get; set; }

        /// <summary>
        ///     The children of this menu
        /// </summary>
        /// <value>The children.</value>
        public Dictionary<string, MenuComponent> Children { get; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        internal Vector2 Position { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="MenuComponent" /> is the root menu.
        /// </summary>
        /// <value><c>true</c> if this menu is the root menu; otherwise, <c>false</c>.</value>
        internal bool Root { get; set; }

        /// <summary>
        ///     Gets whether this cref="MenuComponent" /> is shared.
        /// </summary>
        /// <value><c>true</c> if shared; otherwise, <c>false</c>.</value>
        internal bool Shared { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="IMenuComponent" /> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        internal virtual bool Visible { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="IMenuComponent" /> is toggled.
        /// </summary>
        /// <value><c>true</c> if toggled; otherwise, <c>false</c>.</value>
        internal virtual bool Toggled { get; set; }

        /// <summary>
        ///     Gets or sets whether this <see cref="MenuComponent" /> is a Menu.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        internal virtual bool IsMenu => false;

        internal string AssemblyConfigDirectoryName
        {
            get
            {
                return this.Root || this.Parent == null
                    ? this._AssemblyConfigDirectoryName
                    : this.Parent.AssemblyConfigDirectoryName;
            }
            set
            {
                this._AssemblyConfigDirectoryName = value;
            }
        }

        internal string ConfigBaseFolder
        {
            get
            {
                if (this.Shared || this.Parent != null && this.Parent.Shared)
                {
                    return MenuManager.Instance.SharedSettingsPath;
                }

                return Path.Combine(MenuManager.Instance.MenuSettingsPath, this.AssemblyConfigDirectoryName);
            }
        }

        internal virtual string ConfigName
        {
            get
            {
                if (this.IsMenu)
                {
                    return this.CleanFileName(this.InternalName);
                }

                return this.CleanFileName($"{this.InternalName}.{this.GetType().Name}.json");
            }
        }

        internal string ConfigPath
        {
            get
            {
                if (this.Root || this.Shared)
                {
                    return Path.Combine(this.ConfigBaseFolder, this.ConfigName);
                }

                return Path.Combine(this.Parent.ConfigPath, this.ConfigName);
            }
        }

        /// <summary>
        ///     Gets the Root Menu that this Menu belongs to
        /// </summary>
        internal Menu RootMenu
        {
            get
            {
                if (this.Root)
                {
                    return (Menu) this;
                }

                if (this.Parent != null)
                {
                    return this.Parent.RootMenu;
                }

                return null;
            }
        }


        internal virtual bool SavableMenuItem { get; set; } = true;

        internal abstract string Serialized { get; }

        internal Logger Logger => LogManager.GetCurrentClassLogger();

        private string ToolTip { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <inheritdoc />
        public T As<T>()
            where T : IMenuComponent
        {
            return (T) (IMenuComponent) this;
        }

        /// <summary>
        ///     Removes this component from its parent menu
        /// </summary>
        public void Dispose()
        {
            if (this.Parent != null)
            {
                this.Parent.Children.Remove(this.InternalName);
            }
        }

        /// <summary>
        ///     Sets the Tool Tip
        /// </summary>
        /// <param name="toolTip">The tooltip</param>
        public MenuComponent SetToolTip(string toolTip)
        {
            this.ToolTip = toolTip;
            return this;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the bounds.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <returns>System.Drawing.Rectangle.</returns>
        internal virtual Rectangle GetBounds(Vector2 pos)
        {
            return new Rectangle((int)pos.X, (int)pos.Y, this.Parent.Width, MenuManager.Instance.Theme.MenuHeight);
        }

        /// <summary>
        ///     The WndProc that all Menu Components share
        /// </summary>
        internal void BaseWndProc(uint message, uint wparam, int lparam)
        {
            if (this.Visible && message == (ulong)WindowsMessages.WM_MOUSEMOVE)
            {
                var x = lparam & 0xffff;
                var y = lparam >> 16;

                MenuManager.LastMouseMoveTime = Game.TickCount;
                MenuManager.LastMousePosition = new Point(x, y);
            }

            this.WndProc(message, wparam, lparam);
        }

        /// <summary>
        ///     An application-defined function that processes messages sent to a window.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="wparam">Additional message information.</param>
        /// <param name="lparam">Additional message information.</param>
        internal virtual void WndProc(uint message, uint wparam, int lparam)
        {
        }

        /// <summary>
        ///     Gets the render manager.
        /// </summary>
        /// <returns>Aimtec.SDK.Menu.Theme.IRenderManager.</returns>
        internal abstract IRenderManager GetRenderManager();

        /// <summary>
        ///     Renders the tooltip
        /// </summary>
        private void RenderToolTip()
        {
            var text = $"[i] {this.ToolTip}";
            var width = Math.Max(this.Parent.Width, (int)MiscUtils.MeasureTextWidth(text));

            DefaultMenuTheme.DrawRectangleOutline(
                this.Position.X,
                this.Position.Y,
                width,
                MenuManager.Instance.Theme.MenuHeight,
                MenuManager.Instance.Theme.LineWidth,
                MenuManager.Instance.Theme.LineColor);

            var position = this.Position + MenuManager.Instance.Theme.LineWidth;

            Aimtec.Render.Rectangle(
                position,
                width - MenuManager.Instance.Theme.LineWidth,
                MenuManager.Instance.Theme.MenuHeight - MenuManager.Instance.Theme.LineWidth,
                MenuManager.Instance.Theme.MenuBoxBackgroundColor);

            var centerPoint = this.Position + new Vector2(
                width - MenuManager.Instance.Theme.LineWidth * 2 / 2,
                MenuManager.Instance.Theme.MenuHeight - MenuManager.Instance.Theme.LineWidth * 2 / 2);

            var textPosition = position + new Vector2(
                MenuManager.Instance.Theme.TextSpacing,
                MenuManager.Instance.Theme.MenuHeight / 2);
            Aimtec.Render.Text(textPosition, Color.LightBlue, text, RenderTextFlags.VerticalCenter);
        }

        /// <summary>
        ///     Renders at the specified position.
        /// </summary>
        /// <param name="pos">The position.</param>
        internal virtual void Render(Vector2 pos)
        {
            if (this.Visible)
            {
                if (!string.IsNullOrEmpty(this.ToolTip))
                {
                    if (Game.TickCount - MenuManager.LastMouseMoveTime > 500)
                    {
                        if (this.GetBounds(this.Position).Contains(MenuManager.LastMousePosition))
                        {
                            this.RenderToolTip();
                            return;
                        }
                    }
                }

                this.GetRenderManager().Render(pos);
            }
        }

        internal virtual void Save()
        {
            if (!this.SavableMenuItem)
            {
                return;
            }

            if (this.Shared)
            {
                if (!Directory.Exists(MenuManager.Instance.SharedSettingsPath))
                {
                    Directory.CreateDirectory(MenuManager.Instance.SharedSettingsPath);
                }
            }

            else
            {
                if (!Directory.Exists(this.Parent.ConfigPath))
                {
                    Directory.CreateDirectory(this.Parent.ConfigPath);
                }
            }

            File.WriteAllText(this.ConfigPath, this.Serialized);
        }

        /// <summary>
        ///     Loads the saved value for this item
        /// </summary>
        internal abstract void LoadValue();

        /// <summary>
        ///     Fires the OnValueChanged event for this item
        /// </summary>
        protected virtual void FireOnValueChanged(MenuComponent sender, ValueChangedArgs args)
        {
            if (this.OnValueChanged != null)
            {
                //Fire the value changed of this menucomponent instance
                this.OnValueChanged(sender, args);
            }

            //Fire the value changed on the parent menu 
            if (this.Parent != null)
            {
                this.Parent.FireOnValueChanged(sender, args);
            }
        }

        private string CleanFileName(string fileName)
        {
            var clean = Path.GetInvalidFileNameChars().Aggregate(
                fileName,
                (current, c) => current.Replace(c.ToString(), string.Empty));
            return clean;
        }

        #endregion
    }
}