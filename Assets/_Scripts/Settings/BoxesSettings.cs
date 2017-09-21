using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoxesSettings", menuName = "BoxesSettings Definition", order = 2)]
public class BoxesSettings : ScriptableObject
{
    public string[] IdOfBoxTypes = new string[24] {
        "ReflectShellBox", "ProjectileShellBox","3SpreadShellBox","SuccessiveShellBox","SwordBox",
        "HealthBox-min", "HealthBox-low", "HealthBox-mid","HealthBox-high","Accelerate-low", "Accelerate-high", "ShieldBox","InvincibleBox",
        "TrapBox","FirstAidKitBox","TimeBombBox","FirePathBox","FlameThrowerBox","DecelerationZoneBox","SelfExplosionBox","HealOrHarmBox","GoodRandomBox","RandomBox", "TrackShellBox",};

    public string[] IdOfShellBoxTypes = new string[6] { "ReflectShellBox", "ProjectileShellBox", "3SpreadShellBox", "SuccessiveShellBox", "SwordBox", "TrackShellBox" };
    public string[] IdOfGoodBoxTypes = new string[8] { "HealthBox-min", "HealthBox-low", "HealthBox-mid", "HealthBox-high", "Accelerate-low", "Accelerate-high", "ShieldBox", "InvincibleBox" };
    public string[] IdOfWeaponItemBoxTypes = new string[7] { "TrapBox", "FirstAidKitBox", "TimeBombBox", "FirePathBox", "FlameThrowerBox", "DecelerationZoneBox", "SelfExplosionBox" };
    public string[] IdOfRandomBoxTypes = new string[2] { "GoodRandomBox", "RandomBox" };

    public string[] YourBoxTypes = new string[24] { 
        "ReflectShellBox", "ProjectileShellBox","3SpreadShellBox","SuccessiveShellBox","SwordBox",
        "HealthBox-min", "HealthBox-low", "HealthBox-mid","HealthBox-high","Accelerate-low", "Accelerate-high", "ShieldBox","InvincibleBox",
        "TrapBox","FirstAidKitBox","TimeBombBox","FirePathBox","FlameThrowerBox","DecelerationZoneBox","SelfExplosionBox","HealOrHarmBox","GoodRandomBox","RandomBox","TrackShellBox"};
}
