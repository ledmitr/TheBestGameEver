using UnityEngine;

namespace Assets.TD.scripts
{
    public abstract class Selectable : Unit
    {
        public abstract bool IsSelected();
        public abstract void Select(bool isSelected);
    }
}