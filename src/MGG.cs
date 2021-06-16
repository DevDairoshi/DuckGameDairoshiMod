using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Guns")]
    public class MGG : Gun
    {
        public MGG(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("mgg"));

            center = new Vec2(13f, 9.5f);
            collisionOffset = new Vec2(-13f, -3.5f);
            collisionSize = new Vec2(26f, 13f);
            _barrelOffsetTL = new Vec2(24f, 7f);
            _holdOffset = new Vec2(2f, -2f);

            wideBarrel = true;
            _fullAuto = true;
            _fireWait = 1.2f;
            ammo = 20;
            _ammoType = (AmmoType)new ATGrenade();
            _type = "gun";
            _kickForce = 3f;
            _fireSound = "deepMachineGun";
            editorTooltip = "Welp... You\'re fucked.";
        }
    }
}
