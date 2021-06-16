using System;
using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Lasers")]
    public class Laserose : Gun
    {
        public sbyte _loadProgress = 100;
        public float _loadAnimation = 1f;
        protected SpriteMap _loaderSprite;
        public Laserose(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("laserose"));

            center = new Vec2(16f, 4.5f);
            collisionSize = new Vec2(32f, 9f);
            collisionOffset = new Vec2(-16f, -4.5f);
            _barrelOffsetTL = new Vec2(32f, 2f);
            _holdOffset = new Vec2(8f, 0f);

            wideBarrel = true;
            _manualLoad = true;
            _numBulletsPerFire = 15;
            ammo = 10;
            _type = "gun";
            _ammoType = (AmmoType)new ATLaserose();
            _kickForce = 3f;
            _fireSound = GetPath("roseShot");
            _bulletColor = Color.White;
            editorTooltip = "Tactical - long range, low spread.";

            _loaderSprite = new SpriteMap(GetPath("roseLoader"), 7, 4);
            _loaderSprite.center = new Vec2(18f, 1f);
        }

        public override void Update()
        {
            base.Update();
            if ((double)this._loadAnimation == -1.0)
            {
                SFX.Play("shotgunLoad");
                this._loadAnimation = 0.0f;
            }
            if ((double)this._loadAnimation >= 0.0)
            {
                if ((double)this._loadAnimation == 0.5 && this.ammo != 0)
                    this.PopShell();
                if ((double)this._loadAnimation < 1.0)
                    this._loadAnimation += 0.1f;
                else
                    this._loadAnimation = 1f;
            }
            if (this._loadProgress < (sbyte)0)
                return;
            if (this._loadProgress == (sbyte)50)
                this.Reload(false);
            if (this._loadProgress < (sbyte)100)
                this._loadProgress += (sbyte)10;
            else
                this._loadProgress = (sbyte)100;
        }

        public override void OnPressAction()
        {
            if (this.loaded)
            {
                base.OnPressAction();
                this._loadProgress = (sbyte)-1;
                this._loadAnimation = -0.01f;
            }
            else
            {
                if (this._loadProgress != (sbyte)-1)
                    return;
                this._loadProgress = (sbyte)0;
                this._loadAnimation = -1f;
            }
        }

        public override void Draw()
        {
            base.Draw();
            Vec2 vec2 = new Vec2(13f, -2f);
            float num = (float)Math.Sin((double)this._loadAnimation * 3.14000010490417) * 3f;
            this.Draw((Sprite)this._loaderSprite, new Vec2(vec2.x + 8f - num, vec2.y + 3f));
        }
    }

    public class ATLaserose : AmmoType
    {
        public bool angleShot = true;

        public ATLaserose()
        {
            this.accuracy = 0.001f;
            this.range = 200f;
            this.penetration = 1f;
            this.bulletSpeed = 20f;
            this.bulletThickness = 0.3f;
            this.bulletType = typeof(LaserBullet);
            this.flawlessPipeTravel = true;
        }
    }
}
