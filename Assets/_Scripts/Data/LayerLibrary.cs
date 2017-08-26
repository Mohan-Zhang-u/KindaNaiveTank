using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerLibrary : PersistentSingleton<LayerLibrary>
{
    #region layermasks
    [HideInInspector]
    public LayerMask DefaultMask;
    [HideInInspector]
    public LayerMask TransparentFXMask;
    [HideInInspector]
    public LayerMask IgnoreRaycastMask;
    [HideInInspector]
    public LayerMask WaterMask;
    [HideInInspector]
    public LayerMask UIMask;
    [HideInInspector]
    public LayerMask PlayersMask;
    [HideInInspector]
    public LayerMask TankMask;
    [HideInInspector]
    public LayerMask WallMask;
    [HideInInspector]
    public LayerMask ShellMask;
    [HideInInspector]
    public LayerMask GeneralItemsMask;
    [HideInInspector]
    public LayerMask ExplosiveItemMask;
    [HideInInspector]
    public LayerMask GroundMask;
    [HideInInspector]
    public LayerMask ObstacleItemMask;
    #endregion

    //[HideInInspector]
    //protected LayerMask DefaultMask;
    //[HideInInspector]
    //protected LayerMask TransparentFXMask;
    //[HideInInspector]
    //protected LayerMask IgnoreRaycastMask;
    //[HideInInspector]
    //protected LayerMask WaterMask;
    //[HideInInspector]
    //protected LayerMask UIMask;
    //[HideInInspector]
    //protected LayerMask PlayersMask;
    //[HideInInspector]
    //protected LayerMask TankMask;
    //[HideInInspector]
    //protected LayerMask WallMask;
    //[HideInInspector]
    //protected LayerMask ShellMask;
    //[HideInInspector]
    //protected LayerMask GeneralItemsMask;
    //[HideInInspector]
    //protected LayerMask ExplosiveItemMask;
    //[HideInInspector]
    //protected LayerMask GroundMask;
    //[HideInInspector]
    //protected LayerMask ObstacleItemMask;


    protected override void Awake()
    {
        base.Awake();
#region LayerInits
        DefaultMask = 0;
        TransparentFXMask = 1;
        IgnoreRaycastMask = 2;
        WaterMask = 4;
        UIMask = 5;
        GroundMask = LayerMask.NameToLayer("Ground");
        PlayersMask = LayerMask.NameToLayer("Players");
        TankMask = LayerMask.NameToLayer("TankToSpawn");
        WallMask = LayerMask.NameToLayer("Wall");
        ShellMask = LayerMask.NameToLayer("Shell");
        GeneralItemsMask = LayerMask.NameToLayer("GeneralItem");
        ExplosiveItemMask = LayerMask.NameToLayer("ExplosiveItem");
        ObstacleItemMask = LayerMask.NameToLayer("ObstacleItem");
#endregion
    }

}
