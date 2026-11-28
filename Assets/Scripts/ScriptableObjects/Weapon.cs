using UnityEngine;


[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapons")]
public class Weapon : Item
{
    public float DamageMult;
    public int AnimationType; // 1 = swing, 2 = stab, 3 = shoot
}
