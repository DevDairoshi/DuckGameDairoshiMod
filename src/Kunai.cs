using System;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Throwable")]
    public class Kunai : Gun
    {
        public StateBinding _groundedStateBinding = new StateBinding("_grounded");
        public StateBinding gravMultiplierStateBinding = new StateBinding("gravMultiplier");
        public StateBinding _leakedStateBinding = new StateBinding("_leaked");
        public StateBinding _currentStateStateBinding = new StateBinding("_currentState");
        public StateBinding _savedDuckStateBinding = new StateBinding("_savedDuck");

        public float _leaked;
        public int _currentState;
        public Duck _savedDuck;
        private Sprite sprite;

        public Kunai(float xval, float yval) : base(xval, yval)
        {
            sprite = new Sprite(GetPath("kunai"));
            graphic = sprite;
            _center = new Vec2(10f, 3.5f);
            _collisionSize = new Vec2(20f, 7f);
            _collisionOffset = new Vec2(-10f, -3.5f);
            _holdOffset = new Vec2(2f, 2f);

            ammo = 1;
            weight = 0.5f;
            thickness = 1f;
            depth = -0.5f;
            _hasTrigger = false;
            physicsMaterial = PhysicsMaterial.Metal;
            throwSpeedMultiplier = 2.5f;

            _skipAutoPlatforms = true;
            _skipPlatforms = true;

            editorTooltip = "Narutooooo!";
            _leaked = 0f;
            _currentState = 0;
        }

        public override void Update()
        {
            if (duck != null)
                _savedDuck = duck;

            base.Update();

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
            else if (_currentState == 2)
            {
                if (_holdOffset.x > 2f)
                {
                    handOffset.x -= 0.5f;
                    _holdOffset.x -= 1f;
                }
                else
                    _currentState = 0;
            }

            if (this.duck == null)
            {
                this.angleDegrees = offDir > 0 ? -Maths.PointDirection(Vec2.Zero, new Vec2(this._hSpeed, this._vSpeed)) : -Maths.PointDirection(Vec2.Zero, new Vec2(this._hSpeed, this._vSpeed)) + 180f;
            }
        }

        public override void Thrown()
        {
            handOffset.x = 0f;
            _holdOffset.x = 2f;
            _currentState = 0;
            base.Thrown();
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            base.OnSolidImpact(with, from);
            if (_currentState != -1
                && !(with is Gun)
                && !(with is Duck)
                && !(with is Door)
                && !(with is VerticalDoor)
                && with is Block
                && from == (offDir > 0 ? ImpactedFrom.Right : ImpactedFrom.Left))
            {
                _hSpeed = 0f;
                _vSpeed = 0f;
                gravMultiplier = 0f;
                grounded = true;
                _currentState = -1;
                if (with is Block)
                {
                    SFX.Play("ting", 1f, Rando.Float(-0.2f, 0.2f));
                }
            }
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom from)
        {
            base.OnImpact(with, from);
            if (_currentState != -1
                && impactPowerH >= 0.05f
                && (with is Duck || with is RagdollPart)
                && with != _savedDuck
                && ((offDir > 0 && from == ImpactedFrom.Right)
                    || (offDir < 0 && from == ImpactedFrom.Left)))
            {
                if (with is Duck)
                    ((Duck)with).Kill(new DTImpale(with));
                else if (with is RagdollPart)
                    ((RagdollPart)with)._doll._duck.Kill(new DTImpale(with));
            }
        }

        public override void CheckIfHoldObstructed()
        {
            if (owner == null)
                return;
            duck.holdObstructed = false;
        }

        public override void OnPressAction()
        {

        }
    }
}
