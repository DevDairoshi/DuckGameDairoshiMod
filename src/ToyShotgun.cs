using System;
using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Misc")]
    public class ToyShotgun : Gun
    {
        public StateBinding _LoadBinding = new StateBinding("_loadProgress");
        public StateBinding _LoadedStateBinding = new StateBinding("loaded");

        public NetSoundBinding _ShotSoundBinding = new NetSoundBinding("_shot");

        public NetSoundEffect _shot = new NetSoundEffect(new string[1] { "dartGunFire" });

        public sbyte _loadProgress = 100;
        public float _loadAnimation = 1f;
        protected SpriteMap _loaderSprite;
        public ToyShotgun(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("toyShotgun"));

            center = new Vec2(17f, 5f);
            collisionSize = new Vec2(34f, 9f);
            collisionOffset = new Vec2(-17f, -4f);
            _barrelOffsetTL = new Vec2(34f, 2f);
            _holdOffset = new Vec2(6f, 0f);
           
            wideBarrel = true;
            _manualLoad = true;
            _numBulletsPerFire = 8;
            ammo = 10;
            _type = "gun";
            _ammoType = (AmmoType)new ATLaser();
            _kickForce = 2f;
            _fireSound = "shotgunFire2";
            _bulletColor = Color.White;
            editorTooltip = "Just a toy shotgun... more like a toy sniper... more like duck fucker...";

            _loaderSprite = new SpriteMap(GetPath("toyLoader2"), 4, 7);
            _loaderSprite.center = new Vec2(11f, 1f);
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
                if (this.ammo > 0)
                {
                    _shot.Play(1f, Rando.Float(0.2f) - 0.1f);
                    
                    this.loaded = false;
                    if (this.receivingPress || !this.isServerForObject)
                        return;
                    Vec2 vec2 = this.Offset(this.barrelOffset);

                    Dart dart = new Dart(vec2.x, vec2.y, this.owner as Duck, 0);
                    Level.Add((Thing)dart);
                    this.Fondle((Thing)dart);

                    dart.gravMultiplier *= 0.2f;
                    dart.weight = 15f;
                    dart.hMax = 80f;
                    dart.vMax = 80f;


                    if (this.onFire)
                    {
                        Level.Add((Thing)SmallFire.New(0.0f, 0.0f, 0.0f, 0.0f, stick: ((MaterialThing)dart), firedFrom: ((Thing)this)));
                        dart.burning = true;
                        dart.onFire = true;
                    }
                    dart.hSpeed = barrelVector.x * 30f;
                    dart.vSpeed = barrelVector.y * 30f;
                }
                else
                {
                    this.DoAmmoClick();
                }

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
}
