using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Lasers")]
    public class LMG : Gun
    {
        public LMG(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("lmg"));

            center = new Vec2(14.5f, 6f);
            collisionSize = new Vec2(29f, 9f);
            collisionOffset = new Vec2(-14.5f, -4f);
            _barrelOffsetTL = new Vec2(29f, 5f);
            _holdOffset = new Vec2(3f, -1f);

            wideBarrel = true;
            _fullAuto = true;
            this._fireWait = 0.8f;
            ammo = 30;
            _ammoType = (AmmoType)new ATLMG();
            _type = "gun";
            _kickForce = 1f;
            _fireSound = GetPath("destroyerShot");
            editorTooltip = "Fast, lethal, laser machine gun.";
        }
    }

    public class ATLMG : AmmoType
    {
        public ATLMG()
        {
            this.accuracy = 0.95f;
            this.range = 300f;
            this.penetration = 9f;
            this.bulletSpeed = 30f;
            this.bulletThickness = 0.5f;
            this.bulletType = typeof(LaserBulletOrange);
            this.flawlessPipeTravel = true;
        }
    }
}
