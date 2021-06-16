namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Lasers")]
    public class LaserTrapper : Gun
    {
        public LaserTrapper(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("laserTrapper"));

            center = new Vec2(9.5f, 5.5f);
            collisionSize = new Vec2(19f, 11f);
            collisionOffset = new Vec2(-9.5f, -5.5f);
            _barrelOffsetTL = new Vec2(19f, 4f);
            _holdOffset = new Vec2(2f, 1f);

            wideBarrel = true;
            this._fireWait = 5f;
            ammo = 3;
            _ammoType = (AmmoType)new ATLaserTrap();
            _type = "gun";
            _kickForce = 0f;
            _fireSound = "laserRifle";
            editorTooltip = "Great tool to trap some nasty ducks.";
        }
    }

    public class ATLaserTrap : AmmoType
    {
        public ATLaserTrap()
        {
            this.accuracy = 1f;
            this.range = float.MaxValue;
            this.rebound = true;
            this.penetration = 9f;
            this.bulletSpeed = 7f;
            this.bulletThickness = 0.5f;
            this.bulletLength = 100f;
            this.bulletType = typeof(LaserBullet);
        }
    }
}
