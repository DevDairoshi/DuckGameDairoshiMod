namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Explosives")]
    public class AirStrike : Gun
    {
        public StateBinding _TimerStateBinding = new StateBinding("_timer");
        public StateBinding _AmmoStateBinding = new StateBinding("ammo");
        public StateBinding _DropStateBinding = new CompressedVec2Binding("_strikePos");

        public NetSoundBinding _DropNetSoundBinding = new NetSoundBinding("_dropSFX");
        public NetSoundBinding _PresNetSoundBinding = new NetSoundBinding("_pressSFX");
        public NetSoundBinding _QuackSoundBinding = new NetSoundBinding("_quackSFX");

        public NetSoundEffect _dropSFX = new NetSoundEffect(new string[1] { Mod.GetPath<DairoshiMod>("drop") });
        public NetSoundEffect _pressSFX = new NetSoundEffect(new string[1] { Mod.GetPath<DairoshiMod>("press") });
        public NetSoundEffect _quackSFX = new NetSoundEffect(new string[1] { Mod.GetPath<DairoshiMod>("quack") });

        private SpriteMap _sprite;
        private Sprite _target;
        private Duck _lastDuck;

        public AirStrike(float xval, float yval) : base(xval, yval)
        {
            _target = new Sprite(GetPath("target"));
            _target.CenterOrigin();

            _sprite = new SpriteMap(GetPath("pilot"), 8, 15, false);
            graphic = _sprite;

            ammo = 2;
            
            center = new Vec2(4f, 9f);
            collisionOffset = new Vec2(-4f, -4f);
            collisionSize = new Vec2(8f, 10f);
            _barrelOffsetTL = new Vec2(4f, 9f);

            editorTooltip = "Justice rains from above!";
            base.bouncy = 0.3f;
            friction = 0.1f;
            _timer = 2f;
        }

        public override void Update()
        {
            base.Update();
            _target._angle += 0.05f;

            if (this.duck != null)
            {
                _lastDuck = duck;
            }

            if (ammo < 2)
            {
                _timer -= 0.01f;
            }
            if (_timer < 0 && ammo > 0)
            {
                _dropSFX.Play(1.5f);

                for (int i = -4; i < 4; i++)
                {
                    var bullet = new Bullet(_strikePos.x - 10f + i * 30f, _strikePos.y - 500f + i * 30f, new ATMissile(), -85f);
                    Level.Add(bullet);

                    this.Fondle(bullet);

                    this.firedBullets.Add(bullet);
                    if (this._lastDuck != null && this._lastDuck.profile.connection != null)
                        bullet.connection = this._lastDuck.profile.connection;

                    bullet._hSpeed = 0f;
                    bullet._vSpeed = 10f;
                }

                this.ammo = 0;
            }
        }

        public override void OnPressAction()
        {
            if (ammo > 1)
            {
                --this.ammo;
                _strikePos = this.position;
                _target.position = this.position;
                this._sprite.frame = 1;

                _pressSFX.Play();
                _quackSFX.Play();
            }
        }

        public override void Draw()
        {
            if (ammo == 1)
            {
                _target.Draw();
            }
            base.Draw();
        }

        private Vec2 _strikePos;
        private float _timer;
    }
}
