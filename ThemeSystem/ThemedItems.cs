namespace Rpg;

public sealed class OldBook : Item{
    public override char Symbol => 'o';
    public override string Name => "Old Book";
    public override string Description => "A dusty book. Wisdom +2.";
    public override int GetWisdomModifier() => 2;
}

public sealed class MetalFragment : Item{
    public override char Symbol => 'r';
    public override string Name => "Metal Fragment";
    public override string Description => "A heavy metal fragment. Strength +1.";
    public override int GetStrengthModifier() => 1;
}

public sealed class BlackWand : TwoHandWeapon{
    private static readonly IWeaponKind _kind = new MagicalWeaponKind();
    public override int BaseDamage => 12;
    public override char Symbol => 'B';
    public override string Name => "Black Wand";
    public override bool IsTwoHanded => true;
    public override IWeaponKind WeaponKind => _kind;
}

public sealed class Blaster : TwoHandWeapon{
    private static readonly IWeaponKind _kind = new HeavyWeaponKind();
    public override int BaseDamage => 15;
    public override char Symbol => 'L';
    public override string Name => "Blaster";
    public override bool IsTwoHanded => true;
    public override IWeaponKind WeaponKind => _kind;
}

public sealed class LuckyCoinPouch : TwoHandWeapon{
    private static readonly IWeaponKind _kind = new LightWeaponKind();
    public override int BaseDamage => 8;
    public override char Symbol => 'P';
    public override string Name => "Lucky Coin Pouch";
    public override bool IsTwoHanded => true;
    public override IWeaponKind WeaponKind => _kind;
    public override int GetLuckModifier() => 10;
}