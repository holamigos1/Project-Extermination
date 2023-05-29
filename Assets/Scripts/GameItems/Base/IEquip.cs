namespace GameItems.Base
{
    public interface IEquip
    {
        public bool IsEquipped { get; }

        public void Equip();
    }
}