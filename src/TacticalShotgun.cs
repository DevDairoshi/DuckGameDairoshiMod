using System;
using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Guns")]
    public class TacticalShotgun : Gun
    {
        public sbyte _loadProgress = 100;
        public float _loadAnimation = 1f;
        protected SpriteMap _loaderSprite;

        public TacticalShotgun(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("tactical"));

            center = new Vec2(20.5f, 7f);
            collisionSize = new Vec2(41f, 14f);
            collisionOffset = new Vec2(-20.5f, -7f);
            _barrelOffsetTL = new Vec2(41f, 4f);
            _holdOffset = new Vec2(11f, 0f);
            
            wideBarrel = true;
            _manualLoad = true;
            _numBulletsPerFire = 8;
            ammo = 6;
            _type = "gun";
            _ammoType = (AmmoType)new ATTactical();
            _kickForce = 3f;
            _fireSound = GetPath("tacticalShot");
            _fireSoundPitch = -0.3f;
            editorTooltip = "Tactical - long range, low spread.";

            _loaderSprite = new SpriteMap(GetPath("loader"), 9, 4);
            _loaderSprite.center = new Vec2(16f, 3f);
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

    public class ATTactical : AmmoType
    {
        public ATTactical()
        {
            this.accuracy = 0.9f;
            this.range = 300f;
            this.penetration = 2f;
            this.rangeVariation = 0f;
            this.combustable = true;
        }

        public override void PopShell(float x, float y, int dir)
        {
            ShotgunShell shotgunShell = new ShotgunShell(x, y);
            shotgunShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)shotgunShell);
        }
    }
}
