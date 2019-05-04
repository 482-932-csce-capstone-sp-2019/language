using System.Collections.Generic;

public class CharacterStat
{
    public float BaseValue;

    private readonly List<StatModifier> statModifiers;

    public CharacterStat(float baseValue)
    {
        BaseValue = baseValue;
        statModifiers = new List<StatModifier>();
    }
}
