namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Guns")]
    public class MagRifle : Gun
    {
        public MagRifle(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("mag"));

            center = new Vec2(17f, 7.5f);
            collisionSize = new Vec2(34f, 13f);
            collisionOffset = new Vec2(-17f, -5.5f);
            _barrelOffsetTL = new Vec2(34f, 4f);
            _holdOffset = new Vec2(0f, 0f);

            this.loseAccuracy = 1f;
            _fullAuto = true;
            this._fireWait = 1f;
            ammo = 30;
            _ammoType = (AmmoType)new ATMagRifle();
            _type = "gun";
            _kickForce = 9f;
            this._fireSound = "magShot";
            editorTooltip = "This rifle is too powerful to contain...";
        }
    }

    public class ATMagRifle : AmmoType
    {
        public bool angleShot = true;

        public ATMagRifle()
        {
            this.accuracy = 0.90f;
            this.range = 300f;
            this.penetration = 3f;
            this.bulletSpeed = 40f;
            this.bulletThickness = 0.8f;
            this.bulletType = typeof(MagBullet);
            this.combustable = true;
        }
    }
}
