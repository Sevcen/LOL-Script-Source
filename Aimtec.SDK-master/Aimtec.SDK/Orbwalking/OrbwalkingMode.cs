namespace Aimtec.SDK.Orbwalking
{
    public enum OrbwalkingMode
    {
        /// <summary>
        ///     Orbwalker does nothing.
        /// </summary>
        None,

        /// <summary>
        ///     Orbwalker will attack the target.
        /// </summary>
        Combo,

        /// <summary>
        ///     Orbwalker will auto attack the target and last hit minions.
        /// </summary>
        Mixed,

        /// <summary>
        ///     Orbwalker will clear lane minions as fast as possible.
        /// </summary>
        Laneclear,

        /// <summary>
        ///     Orbwalker will only last hit minions.
        /// </summary>
        Lasthit,

        /// <summary>
        ///     Orbwalker will last hit minions as late as possible.
        /// </summary>
        Freeze,

        /// <summary>
        ///     Orbwalker will be in a custom mode.
        /// </summary>
        Custom
    }
}