using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public enum EBulletType
    {
        Normal,
        Poison,
        Fire

    }

    [System.Flags]
    public enum EGunType
    {
        None = 0,               // 0
        AssaultRifle = 1 << 0,  // 1
        SubMachineGun = 1 << 1, // 2
        SniperRifle = 1 << 2,   // 4
        GrenadeLauncher = 1 << 3, // 8
        Shotgun = 1 << 4,       // 16
        HandGun = 1 << 5        // 32
    }


    public enum ESpecialWeapon
    {
        FlameThrower,
        GrenadeLauncher
    }





}
