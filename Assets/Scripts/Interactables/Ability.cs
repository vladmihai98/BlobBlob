using UnityEngine;

public class Ability : MonoBehaviour
{
    [Header("Generic Stats")]
    public TargetType TypeOfTarget;
    public int ManaCost;
    [Tooltip("In realtime seconds.")]
    public int Cooldown;
    public float Speed;
    [Tooltip("In realtime seconds.")]
    public float Duration;
    public float Range;

    [Header("Offensive Stats")]
    public int AttackDamage;
    public int AbilityPower;

    [Header("Defensive Stats")]
    public int HealAmount;
    public int ShieldAmount;

    //DamageModifiers? eg: DoT burn, ignore shield, reduce heal, disarm, crit/execution dmg.

    /// <summary>
    /// What kind of target the ability should collide with.
    /// </summary>
    public enum TargetType 
    {
        /// <summary>
        /// Allow this ability to collide with the players only.
        /// </summary>
        Player, 

        /// <summary>
        /// Allow this ability to collide with the enemies only.
        /// </summary>
        Enemy 
    };
}
