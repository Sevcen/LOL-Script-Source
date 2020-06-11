namespace Aimtec.SDK.Menu.Components
{
    using System;
    using System.Drawing;
    using System.IO;

    using Aimtec.SDK.Menu.Theme;
    using Aimtec.SDK.Util;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class MenuSliderBool. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="Aimtec.SDK.Menu.MenuComponent" />
    /// <seealso cref="int" />
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class MenuSliderBool : MenuComponent, IReturns<int>, IReturns<bool>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuSliderBool" /> class.
        /// </summary>
        /// <param name="internalName">The internal name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="enabled">Whether this is enabled by default</param>
        /// <param name="value">The value.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="shared">Whether this item is shared across instances</param>
        public MenuSliderBool(
            string internalName,
            string displayName,
            bool enabled,
            int value,
            int minValue,
            int maxValue,
            bool shared = false)
        {
            this.InternalName = internalName;
            this.DisplayName = displayName;
            this.Enabled = enabled;
            this.Shared = shared;

            this.Value = value;
            this.MinValue = minValue;
            this.MaxValue = maxValue;

            if (this.Value > this.MaxValue)
            {
                Logger.Warn(
                    $"The value for slider {this.InternalName} is greater than the maximum value of the slider. Setting to maximum.");
                this.Value = maxValue;
            }

            else if (this.Value < this.MinValue)
            {
                Logger.Warn(
                    $"The value for slider {this.InternalName} is lower than the minimum value of the slider. Setting to minimum.");
                this.Value = minValue;
            }

            if (this.MinValue > this.MaxValue)
            {
                Logger.Error(
                    $"The minimum value is greater than the maximum value for slider with name \"{internalName}\"");
                throw new ArgumentException(
                    "The minimum value cannot be greater than the maximum value. Item name: {internalName}");
            }
        }

        [JsonConstructor]
        private MenuSliderBool()
        {
        }

        #endregion

        #region Public Properties

        [JsonProperty(Order = 4)]
        public new bool Enabled { get; set; }

        /// <summary>
        ///     Gets or sets the maximum value.
        /// </summary>
        /// <value>The maximum value.</value>
        public int MaxValue { get; set; }

        /// <summary>
        ///     Gets or sets the minimum value.
        /// </summary>
        /// <value>The minimum value.</value>
        public int MinValue { get; set; }

        [JsonProperty(Order = 3)]
        public new int Value { get; set; }

        #endregion

        #region Explicit Interface Properties

        bool IReturns<bool>.Value
        {
            get => this.Enabled;
            set
            {
                this.Enabled = value;
            }
        }

        #endregion

        #region Properties

        internal override string Serialized => JsonConvert.SerializeObject(this, Formatting.Indented);

        /// <summary>
        ///     Gets or sets a value indicating whether [mouse down].
        /// </summary>
        /// <value><c>true</c> if [mouse down]; otherwise, <c>false</c>.</value>
        private bool MouseDown { get; set; }

        #endregion

        #region Methods

        internal override Rectangle GetBounds(Vector2 pos)
        {
            var bounds = MenuManager.Instance.Theme.GetMenuSliderBoolControlBounds(pos, this.Parent.Width);
            return Rectangle.Union(bounds[0], bounds[1]);
        }

        internal override IRenderManager GetRenderManager()
        {
            return MenuManager.Instance.Theme.BuildMenuSliderBoolRenderer(this);
        }

        /// <summary>
        ///     An application-defined function that processes messages sent to a window.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="wparam">Additional message information.</param>
        /// <param name="lparam">Additional message information.</param>
        internal override void WndProc(uint message, uint wparam, int lparam)
        {
            if ((message == (ulong)WindowsMessages.WM_LBUTTONDOWN
                || message == (ulong)WindowsMessages.WM_MOUSEMOVE && this.MouseDown) && this.Visible)
            {
                var x = lparam & 0xffff;
                var y = lparam >> 16;

                var bounds =
                    MenuManager.Instance.Theme.GetMenuSliderBoolControlBounds(this.Position, this.Parent.Width);
                var sliderBounds = bounds[0];

                if (sliderBounds.Contains(x, y))
                {
                    this.SetSliderValue(x);
                    this.MouseDown = true;
                }
            }

            else if (message == (ulong)WindowsMessages.WM_LBUTTONUP)
            {
                if (this.Visible && !this.MouseDown)
                {
                    var x = lparam & 0xffff;
                    var y = lparam >> 16;
                    var boolBounds =
                        MenuManager.Instance.Theme.GetMenuSliderBoolControlBounds(this.Position, this.Parent.Width)[1];
                    if (boolBounds.Contains(x, y))
                    {
                        this.UpdateEnabled(!this.Enabled);
                    }
                }

                this.MouseDown = false;
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

                var sValue = JsonConvert.DeserializeObject<MenuSliderBool>(read);

                if (sValue?.InternalName != null)
                {
                    this.Value = sValue.Value;
                    this.Enabled = sValue.Enabled;
                }
            }
        }

        /// <summary>
        ///     Sets the slider value.
        /// </summary>
        /// <param name="x">The x.</param>
        private void SetSliderValue(int x)
        {
            var sliderbounds =
                MenuManager.Instance.Theme.GetMenuSliderBoolControlBounds(this.Position, this.Parent.Width)[0];
            var val = Math.Max(
                this.MinValue,
                Math.Min(
                    this.MaxValue,
                    (int) ((x - this.Position.X) / (sliderbounds.Width - MenuManager.Instance.Theme.LineWidth * 2)
                        * this.MaxValue)));
            this.UpdateValue(val);
        }

        /// <summary>
        ///     Updates the value of the Enabled variable, saves the new value and fires the value changed event
        /// </summary>
        /// <param name="newVal">The new value to set it to.</param>
        private void UpdateEnabled(bool newVal)
        {
            var oldClone = new MenuSliderBool
            {
                InternalName = this.InternalName,
                DisplayName = this.DisplayName,
                Enabled = this.Enabled,
                Value = this.Value,
                MinValue = this.MinValue,
                MaxValue = this.MaxValue
            };

            this.Enabled = newVal;

            this.Save();

            this.FireOnValueChanged(this, new ValueChangedArgs(oldClone, this));
        }

        /// <summary>
        ///     Updates the value of the slider, saves the new value and fires the value changed event
        /// </summary>
        /// <param name="newVal">The new value to set it to.</param>
        private void UpdateValue(int newVal)
        {
            var oldClone = new MenuSliderBool
            {
                InternalName = this.InternalName,
                DisplayName = this.DisplayName,
                Enabled = this.Enabled,
                Value = this.Value,
                MinValue = this.MinValue,
                MaxValue = this.MaxValue
            };

            this.Value = newVal;

            this.Save();

            this.FireOnValueChanged(this, new ValueChangedArgs(oldClone, this));
        }

        #endregion
    }
}