namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Misc")]
    public class DemonicBook : Gun
    {
        public SpriteMap _sprite;
        public DemonicBook(float xval, float yval) : base(xval, yval)
        {
            this._sprite = new SpriteMap(GetPath("demonic"), 19, 16);
            this._sprite.frame = 0;
            this.graphic = this._sprite;

            center = new Vec2(9.5f, 8f);
            collisionSize = new Vec2(19f, 16f);
            
            collisionOffset = new Vec2(-9.5f, -8f);
            this._holdOffset = new Vec2(3f, 4f);
            this.handOffset = new Vec2(1f, 1f);

            _hasTrigger = false;
            ammo = 1;
            _ammoType = (AmmoType)new ATLaser();
            _type = "gun";
            _kickForce = 0f;
            _fireSound = "deepMachineGun";
            editorTooltip = "Summons some powerful beings.";
        }

        public override void Update()
        {
            base.Update();
            if (this.owner != null)
            {
                _sprite._frame = 1;
            }
            else
            {
                _sprite._frame = 0;
            }
        }

        public override void OnPressAction()
        {
            if (this.ammo > 0 && duck != null)
            {
                --this.ammo;
                SFX.Play(GetPath("summon"));
                if (this.receivingPress)
                    return;

                if (isServerForObject)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        MusketSmoke musketSmoke = new MusketSmoke(this.position.x + Rando.Float(-30f,30f), this.position.y - 150f + Rando.Float(-30f, 30f));
                        musketSmoke.depth = (Depth)(float)(0.899999976158142 + (double)i * (1.0 / 1000.0));
                        Level.Add(musketSmoke);
                    }
                    
                    EvilEye eye = new EvilEye(this.position.x, this.position.y - 150f);
                    Level.Add(eye);
                    this.Fondle(eye);

                    if (this.duck != null)
                    {
                        this.duck.Scream();
                    }

                    duck.Disarm(eye);
                }
            }
        }
    }
}
