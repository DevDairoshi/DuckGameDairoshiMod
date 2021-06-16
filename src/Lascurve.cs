using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Lasers")]
    public class Lascurve : Gun
    {
        public Lascurve(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("lascurve"));

            center = new Vec2(12.5f, 5f);
            collisionSize = new Vec2(25f, 10f);
            collisionOffset = new Vec2(-12.5f, -5f);
            _barrelOffsetTL = new Vec2(25f, 3f);
            _holdOffset = new Vec2(5f, 1f);

            _kickForce = 1.5f;
            _fullAuto = true;
            ammo = 30;
            _fireWait = 1f;
            _type = "gun";
            _ammoType = (AmmoType)new ATCurve();
            this._ammoType.barrelAngleDegrees = -45f;
            _fireSound = "phaserLarge";
            editorTooltip = "Wait, wut? Is it broken?";
        }
    }

    public class ATCurve : AmmoType
    {
        public ATCurve()
        {
            this.accuracy = 0.6f;
            this.range = 1000f;
            this.penetration = 0.2f;
            this.bulletSpeed = 5f;
            this.speedVariation = 3f;
            this.bulletThickness = 0.3f;
            this.affectedByGravity = true;
            this.bulletType = typeof(LaserBulletPurple);
            this.flawlessPipeTravel = true;
        }
    }
}
