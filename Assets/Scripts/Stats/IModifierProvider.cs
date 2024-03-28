using System.Collections.Generic;

namespace RPG.Stats {
    public interface IModifierProvider {
        // IEnumerable so it can go through multiple equipments, etc for stat modifiers
        IEnumerable<float> GetAdditiveModifiers(Stat stat);
        IEnumerable<float> GetPercentageModifiers(Stat stat);
    }
}

