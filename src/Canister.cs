using System;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Environment")]
    public class Canister : Gun
    {
        public StateBinding gravMultiplierStateBinding = new StateBinding("gravMultiplier");
        public StateBinding _leakedStateBinding = new StateBinding("_leaked");
        public StateBinding _currentStateStateBinding = new StateBinding("_currentState");
        public StateBinding _WeightStateBinding = new StateBinding("weight");

        public float _leaked;
        public int _currentState;

        private Sprite _sprite;
        private Sprite _melting;
        public Canister(float xval, float yval) : base(xval, yval)
        {
            _sprite = new Sprite(GetPath("canister"));
            _melting = new Sprite(GetPath("canisterMelting"));
            graphic = _sprite;
            _center = new Vec2(8f, 8f);
            _barrelOffsetTL = new Vec2(16f, 0f);
            _collisionSize = new Vec2(13f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            _holdOffset = new Vec2(2f, 2f);

            ammo = 10;
            weight = 5.1f;
            thickness = 1f;
            depth = -0.5f;
            _hasTrigger = false;
            physicsMaterial = PhysicsMaterial.Wood;

            editorTooltip = "Contains some gasoline.";
            _leaked = 0f;
            _currentState = 0;
            flammable = 0.4f;
        }

        public override void Update()
        {
            base.Update();

            if (this.duck != null)
            {
                handAngle = offDir * (float)Math.PI / 6;
            }

            this.burnSpeed = 0.0015f;
            if (this._onFire && (double)this.burnt < 0.9)
            {
                graphic = _melting;
                ammo = 1;
                weight = 1.5f;

                collisionSize = new Vec2(11.5f, 12f);
                collisionOffset = new Vec2(0f, -6f);

                this.yscale = (float)(0.5 + (1.0 - (double)this.burnt) * 0.5);
                this.centery = (float)(8.0 - (double)this.burnt * 7.0);
            }

            if (_currentState == -1)
            {
                if (duck != null || velocity.length > 0.6f)
                {
                    _currentState = 0;
                    gravMultiplier = 1f;
                }
                else
                {
                    _vSpeed = 0f;
                    _hSpeed = 0f;
                    gravMultiplier = 0f;
                    grounded = true;
                }
            }
            if (_currentState == 1)
            {
                if (_holdOffset.x < 10f && duck != null && Level.CheckLine<Block>(duck.position, position, this) == null)
                {
                    handOffset.x += 1f;
                    _holdOffset.x += 1f;
                }
                else
                    _currentState = 2;
            }
            else if (_currentState == 2)
            {
                if (_holdOffset.x > 2f)
                {
                    handOffset.x -= 0.5f;
                    _holdOffset.x -= 0.5f;
                }
                else
                    _currentState = 0;
            }
        }


        public override void OnPressAction()
        {
            if (duck != null && _currentState == 0 && ammo > 1)
            {
                ammo--;
                weight -= 0.4f;
                _currentState = 1;
                SFX.Play(GetPath("glug"), 1f, Rando.Float(-0.3f, 0.3f));

                if (isServerForObject)
                {
                    FluidData fluid = Fluid.Gas;
                    fluid.amount = 0.03f;

                    for (int index = 0; index < 10; ++index)
                    {
                        var gas = new Fluid(this.barrelPosition.x, this.barrelPosition.y, new Vec2(0.0f, -1f), fluid);
                        Level.Add((Thing) gas);
                        this.Fondle(gas);
                        gas._hSpeed = barrelVector.x * Rando.Float(3f);
                        gas._vSpeed = barrelVector.y - 1f;
                    }
                }
            }
            else
            {
                _currentState = 1;
            }
        }

        public override void Thrown()
        {
            handOffset.x = 0f;
            _holdOffset.x = 2f;
            _currentState = 0;
            base.Thrown();
        }

        public override void CheckIfHoldObstructed()
        {
            if (owner == null)
                return;
            duck.holdObstructed = false;
        }
    }
}