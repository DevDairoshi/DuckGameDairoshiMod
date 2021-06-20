using System;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Misc")]
    public class HairDryer : Gun
    {
        public StateBinding _powerStateBinding = new StateBinding("_power");
        public StateBinding _onStateBinding = new StateBinding("_on");

        public float _burnWait;
        public bool burntOut;

        public HairDryer(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("dryer"));

            center = new Vec2(10.5f, 8.5f);
            collisionSize = new Vec2(21f, 17f);
            collisionOffset = new Vec2(-10.5f, -8.5f);
            _barrelOffsetTL = new Vec2(29f, 4f);
            _holdOffset = new Vec2(0f, 0f);

            flammable = 0.8f;
            wideBarrel = true;
            ammo = 100;
            _type = "gun";
            _kickForce = 0f;
            _ammoType = (AmmoType)new ATDefault();
            _blow = new Sound(GetPath("blow"), 1f, 0f, 0f, true);
            editorTooltip = "Old hair dryer, legends say that it blows to hard.";

            physicsMaterial = PhysicsMaterial.Plastic;
        }

        protected override void PlayFireSound()
        {
            SFX.Play(this._fireSound, 1f, _power, 0f, true);
        }

        public override void Update()
        {
            if (!this.burntOut && this.burnt >= 1f)
            {
                this.graphic = new Sprite(GetPath("dryerBurnt"));
                Vec2 smokePos = this.Offset(new Vec2(10f, 0f));
                Level.Add(SmallSmoke.New(smokePos.x, smokePos.y));
                this._onFire = false;
                this.flammable = 0f;
                this.burntOut = true;
                _on = false;
            }

            base.Update();

            if (duck == null)
            {
                _on = false;
            }

            _blow.Pitch = _power;

            if (_on)
            {
                _temp += 0.02f;

                if (_temp > 10f)
                {
                    Burn(new Vec2(barrelOffset.x - 10, barrelOffset.y), this);
                }

                if (_power < 1f)
                {
                    _power += 0.02f;
                }
            }
            else
            {
                _temp -= 0.014f;

                if (_power > 0f)
                {
                    _power -= 0.04f;
                }
                else
                {
                    _blow.Stop();
                }
            }

            if (_power > 0.2f)
            {
                Vec2 vec = Offset(barrelOffset);

                if (onFire)
                {
                    if (Rando.Int(0, 100) == 5)
                    {
                        SmallFire fire = SmallFire.New(vec.x, vec.y + +Rando.Float(-3f, 3f),
                            barrelVector.x * _power * 8f + Rando.Float(-3f, 3f) * _power,
                            barrelVector.y * _power * 8f + Rando.Float(-3f, 3f) * _power
                        );
                        Level.Add(fire);
                    }
                    else
                    {
                        AddAir(vec);
                    }
                }
                else
                {
                    AddAir(vec);
                }
            }
        }

        public void AddAir(Vec2 vec)
        {
            Air air = new Air(vec.x, vec.y + Rando.Float(-3f, 3f));
            Level.Add(air);
            this.Fondle(air);

            air.hSpeed = barrelVector.x * _power * 8f + Rando.Float(-3f, 3f) * _power;
            air.vSpeed = barrelVector.y * _power * 8f + Rando.Float(-3f, 3f) * _power;
        }

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            base.onFire = true;
            return true;
        }

        public override void UpdateFirePosition(SmallFire f)
        {
            f.position = this.Offset(new Vec2(barrelOffset.x - 10, barrelOffset.y + 2));
        }

        public override void UpdateOnFire()
        {
            if (this.onFire)
            {
                _burnWait = _burnWait - 0.01f;
                if (this._burnWait < 0f)
                {
                    Level.Add(SmallFire.New(22f, 0f, 0f, 0f, false, this, false, this, false));
                    this._burnWait = 1f;
                }
                if (this.burnt < 1f)
                {
                    burnt = burnt + 0.001f;
                }
            }
        }

        public override void OnPressAction()
        {
            if (!burntOut && !onFire)
            {
                if (!_on && _power <= 0f && duck != null)
                {
                    SFX.Play(_clickSound);
                    _blow.Play();
                    _on = true;
                    _power = 0.01f;
                    return;
                }

                if (_on && _power >= 1f)
                {
                    SFX.Play(_clickSound);

                    _on = false;
                    _power = 1.3f;
                    return;
                }
            }
            else
            {
                SFX.Play(_clickSound);
            }
        }

        public override void Fire()
        {
            // empty
        }

        private bool _on;
        private float _power;
        private Sound _blow;
        private float _temp;
    }

    public class Air : PhysicsObject
    {
        public Air(float x, float y) : base(x, y)
        {
            graphic = new Sprite(GetPath("air"));

            this.center = new Vec2(5f, 5f);
            this.collisionOffset = new Vec2(5f, -5f);
            this.collisionSize = new Vec2(1f, 10f);

            _lifeTime = Rando.Float(0.15f, 0.25f);
            gravMultiplier = -0.5f;
            airFrictionMult = 0.1f;
            _skipPlatforms = true;
            _skipAutoPlatforms = true;
        }

        public override void Update()
        {
            base.Update();

            _lifeTime -= 0.01f;
            if (_lifeTime < 0f)
            {
                Remove();
            }
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom @from)
        {
            if (with.GetType() == typeof(Air) || with == this._owner)
                return;

            this.Fondle(with);
            with.ApplyForce(new Vec2(_hSpeed * 0.4f, -1f));
            Remove();
        }

        public void Remove()
        {
            _destroyed = true;
            position = new Vec2(999999, 999999);
            Level.Remove(this);
        }

        private float _lifeTime;
    }
}
