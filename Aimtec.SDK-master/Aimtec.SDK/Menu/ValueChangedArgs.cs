namespace Aimtec.SDK.Menu
{
    using System;

    /// <summary>
    ///     The Arguments for ValueChanged Event
    /// </summary>
    public class ValueChangedArgs : EventArgs
    {
        #region Fields

        private MenuComponent newValue;

        private MenuComponent previousValue;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Creates a new instance of the ValueChangedArgs class
        /// </summary>
        /// <param name="oldVal"></param>
        /// <param name="newVal"></param>
        public ValueChangedArgs(MenuComponent oldVal, MenuComponent newVal)
        {
            this.previousValue = oldVal;
            this.newValue = newVal;
            this.InternalName = newVal.InternalName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The internal name of the Menu Component that fired this event
        /// </summary>
        public string InternalName { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the new or current instance after the Value Changed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetNewValue<T>()
            where T : MenuComponent
        {
            return (T) this.newValue;
        }

        /// <summary>
        ///     Gets the previous instance before the Value Changed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetPreviousValue<T>()
            where T : MenuComponent
        {
            return (T) this.previousValue;
        }

        #endregion
    }
}