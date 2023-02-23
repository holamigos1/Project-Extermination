namespace Scripts.TagHolders
{
    public struct UnityLayers
    {
        public const string GROUND_LAYER = "Ground";
        public const string OPS_CHARGES_LAYER = "OPS Chrages";
        public const string FIRST_PERSON_LAYER = "First Person View";
    }

    public struct UnityTags
    {
        public const string WEAPON_TAG = "Weapon";
        public const string BULLET_TAG = "Bullet";
        public const string OPS_CHARGE_TAG = "OPS Charge";
        public const string WET_TAG = "Wet";
        public const string DUSTY_TAG = "Dusty";
        public const string BROKEN_TAG = "Broken";
        public const string CLEAR_TAG = "Clear";
        public const string BOUND_TAG = "Bounds";
        public const string MAGNET_POINT_TAG = "Magnet Point";
        public const string FPS_CANVAS_TAG = "FPS Canvas";
        public const string MENU_CANVAS_TAG = "Menu Canvas";
        public const string MAIN_CAMERA_TAG = "MainCamera";
    }

    public struct AnimationTags
    {
        public const string ZOOM_IN_AIM = "Zoom In";
        public const string ZOOM_Out_AIM = "Zoom Out";

        public const string SHOOT_TRIGGER = "Shoot";
        public const string SHOOT_TRIGGER_ONE = "Shoot One";
        public const string SHOOT_TRIGGER_TWO = "Shoot Two";
        public const string SHOOT_TRIGGER_THREE = "Shoot Three";

        public const string RELOAD_TRIGGER = "Reload";
        public const string SWITCH_MODE_TRIGGER = "Switch Mode";

        public const string HIDE_TRIGGER = "Hide";

        public const string WALK_PARRAMITER = "Walk";
        public const string RUN_PARRAMITER = "Run";
        public const string JUMP_PARRAMITER = "Jump";
    }
}

namespace Scripts.GameEnums
{
    public enum Vectors
    {
        Forward,
        Back,
        Left,
        Right,
        Top,
        Bottom
    }

    public enum WeaponAimMode
    {
        Defualt,
        aimFirst,
        aimSecond
    }

    public enum WeaponFireMode
    {
        single,
        twoShoots,
        threeShoots,
        auto
    }
}