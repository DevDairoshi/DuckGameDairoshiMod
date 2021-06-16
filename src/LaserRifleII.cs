namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Lasers")]
    public class LaserRifleII : LaserRifle
    {
        public LaserRifleII(float xval, float yval) : base(xval, yval)
        {
            this.ammo = 30;
            this._ammoType = (AmmoType)new ATReboundLaserII();
            this._ammoType.barrelAngleDegrees = -45f;
            this.graphic = new Sprite(GetPath("laserii"));
            this.center = new Vec2(12f, 6f);
            this.collisionOffset = new Vec2(-12f, -5f);
            this.collisionSize = new Vec2(24f, 11f);
            this._barrelOffsetTL = new Vec2(24f, 3.5f);
            this._holdOffset = new Vec2(0f, 0f);

            this.editorTooltip = "Fires a reflecting beam of deadly energy at an angle... but upwards.";
        }
    }

    public class ATReboundLaserII : ATLaser
    {
        public ATReboundLaserII()
        {
            this.accuracy = 0.8f;
            this.range = 220f;
            this.penetration = 1f;
            this.bulletSpeed = 20f;
            this.bulletThickness = 0.3f;
            this.rebound = true;
            this.bulletType = typeof(LaserBullet);
            this.angleShot = true;
            this.bulletColor = Color.Red;
        }
    }
}
