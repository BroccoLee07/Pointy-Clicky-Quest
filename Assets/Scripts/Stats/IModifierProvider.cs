using System.Collections.Generic;

namespace RPG.Stats {
    public interface IModifierProvider {
        // IEnumerable so it can go through multiple equipments, etc for stat modifiers
        IEnumerable<float> GetAdditiveModifier(Stat stat);
    }
}

