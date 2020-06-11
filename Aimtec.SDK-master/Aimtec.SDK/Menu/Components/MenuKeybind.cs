namespace Aimtec.SDK.Menu.Components
{
    using System.IO;

    using Aimtec.SDK.Menu.Theme;
    using Aimtec.SDK.Util;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class MenuKeybind. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="Aimtec.SDK.Menu.MenuComponent" />
    /// <seealso cref="bool" />
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class MenuKeyBind : MenuComponent, IReturns<bool>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuKeyBind" /> class.
        /// </summary>
        /// <param name="internalName">The internal name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="key">The key.</param>
        /// <param name="keybindType">Type of the keybind.</param>
        /// <param name="active">Whether this item should be active by default</param>
        /// <param name="shared">Whether this item is shared across instances</param>
        public MenuKeyBind(
            string internalName,
            string displayName,
            KeyCode key,
            KeybindType keybindType,
            bool active = false,
            bool shared = false)
        {
            this.InternalName = internalName;
            this.DisplayName = displayName;
            this.Key = key;
            this.KeybindType = keybindType;
            this.Value = active;

            this.Shared = shared;
        }

        [JsonConstructor]
        private MenuKeyBind()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets if this keybind is currently active
        /// </summary>
        /// <value>The value.</value>
        public new bool Enabled => this.Value;

        /// <summary>
        ///     Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        [JsonProperty(Order = 4)]
        public KeyCode Key { get; set; }

        /// <summary>
        ///     Gets or sets the type of the keybind.
        /// </summary>
        /// <value>The type of the keybind.</value>
        public KeybindType KeybindType { get; set; }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [JsonProperty(Order = 3)]
        public new bool Value { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets whether this keybind is currently listening for key presses
        /// </summary>
        internal bool Inactive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the key is being set.
        /// </summary>
        /// <value><c>true</c> if the key is being set; otherwise, <c>false</c>.</value>
        internal bool KeyIsBeingSet { get; set; }

        internal override string Serialized => JsonConvert.SerializeObject(this, Formatting.Indented);

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the render manager.
        /// </summary>
        /// <returns>Aimtec.SDK.Menu.Theme.IRenderManager.</returns>
        internal override IRenderManager GetRenderManager()
        {
            return MenuManager.Instance.Theme.BuildMenuKeyBindRenderer(this);
        }

        /// <summary>
        ///     An application-defined function that processes messages sent to a window.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="wparam">Additional message information.</param>
        /// <param name="lparam">Additional message information.</param>
        internal override void WndProc(uint message, uint wparam, int lparam)
        {
            //No need to process if the item does not belong to a menu yet
            if (this.Parent == null)
            {
                return;
            }

            if (this.Visible)
            {
                var x = lparam & 0xffff;
                var y = lparam >> 16;

                if (message == (ulong)WindowsMessages.WM_LBUTTONDOWN)
                {
                    if (!this.KeyIsBeingSet && this.GetBounds(this.Position).Contains(x, y))
                    {
                        if (!MenuManager.Instance.Theme.GetMenuBoolControlBounds(this.Position, this.Parent.Width)
                                        .Contains(x, y))
                        {
                            this.KeyIsBeingSet = true;
                        }

                        else
                        {
                            this.UpdateValue(!this.Value);
                        }
                    }
                }

                if (this.KeyIsBeingSet && message == (ulong)WindowsMessages.WM_KEYUP)
                {
                    this.UpdateKey((KeyCode)wparam);
                    this.KeyIsBeingSet = false;
                }
            }

            if (this.Inactive || wparam != (ulong)this.Key || this.KeyIsBeingSet || MenuGUI.IsShopOpen()
                || MenuGUI.IsChatOpen())
            {
                return;
            }

            if (this.KeybindType == KeybindType.Press)
            {
                if (message == (ulong)WindowsMessages.WM_KEYDOWN)
                {
                    this.UpdateValue(true);
                }
                else if (message == (ulong)WindowsMessages.WM_KEYUP)
                {
                    this.UpdateValue(false);
                }
            }

            else if (message == (ulong)WindowsMessages.WM_KEYUP)
            {
                this.UpdateValue(!this.Value);
            }
        }

        /// <summary>
        ///     Loads the value from the file for this component
        /// </summary>
        internal override void LoadValue()
        {
            if (File.Exists(this.ConfigPath))
            {
                var read = File.ReadAllText(this.ConfigPath);

                var sValue = JsonConvert.DeserializeObject<MenuKeyBind>(read);

                if (sValue?.InternalName != null)
                {
                    this.Value = sValue.Value;
                    this.Key = sValue.Key;
                }
            }
        }

        private void UpdateKey(KeyCode key)
        {
            //Unregister the keybind if escape is pressed
            if (key == KeyCode.Escape)
            {
                this.Inactive = true;
                this.Value = false;
                return;
            }

            var oldClone = new MenuKeyBind
            {
                Value = this.Value,
                InternalName = this.InternalName,
                DisplayName = this.DisplayName,
                Key = this.Key,
                KeybindType = this.KeybindType
            };

            this.Key = key;

            this.Save();

            this.FireOnValueChanged(this, new ValueChangedArgs(oldClone, this));
        }

        /// <summary>
        ///     Updates the value of the KeyBind, saves the new value and fires the value changed event
        /// </summary>
        private void UpdateValue(bool newVal)
        {
            var oldClone = new MenuKeyBind
            {
                Value = this.Value,
                InternalName = this.InternalName,
                DisplayName = this.DisplayName,
                Key = this.Key,
                KeybindType = this.KeybindType
            };

            this.Value = newVal;

            if (this.KeybindType == KeybindType.Toggle)
            {
                this.Save();
            }

            this.FireOnValueChanged(this, new ValueChangedArgs(oldClone, this));
        }

        #endregion
    }

    /// <summary>
    ///     Enum KeybindType
    /// </summary>
    public enum KeybindType
    {
        /// <summary>
        ///     Press key bind.
        /// </summary>
        Press,

        /// <summary>
        ///     Toggle key bind.
        /// </summary>
        Toggle
    }
}