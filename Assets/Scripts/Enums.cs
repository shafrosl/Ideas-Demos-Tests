public enum GunPart
{
    Body,
    Stock,
    Barrel
}

/// <summary>
/// fire rate = bullets per second
/// recoil control = 0 no control, 10 absolute control
/// magazine size = in groups of 5 (eg. 1 is 1 x 5 = 5)
///
/// burn dmg = burn dmg per second
/// burn duration = how long burn lasts
/// damage = damage per bullet
/// heal on damage = % of dmg is healed per bullet
/// stun duration = how long target is stunned for
/// knockback = how far target is knocked back
/// </summary>

public enum Modifier
{
    Null = 0,
    FireRate = 1,
    RecoilControl = 3,
    MagazineSize = 4,
    
    BurnDamage = 5,
    BurnDuration = 6,
    Damage = 7,
    HealOnDamage = 8,
    StunDuration = 9,
    PushBack = 10,
}

public enum Tier
{
    Null = 0,
    One = 1,
    Two = 2,
    Three = 3
}

public enum Cam
{
    Null = 0,
    UI = 1,
    Game = 2
}

public enum Axis
{
    Z = 0,
    X = 1,
    XZ = 2,
    ZX = 3
}

public enum GameMode
{
    NotSelected,
    GunRange,
    TimeCrisis
}

public enum Direction
{
    None,
    Up,
    Down,
    Left,
    Right
}

public enum RoomType
{
    Elbow,
    Straight
}
