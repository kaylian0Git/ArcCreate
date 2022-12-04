using ArcCreate.Gameplay.Judgement;
using UnityEngine;

namespace ArcCreate.Gameplay.Data
{
    public class Tap : Note, INote<TapBehaviour>
    {
        private TapBehaviour instance;
        private bool judgementRequestSent = false;

        public int Lane { get; set; }

        public bool IsAssignedInstance => instance != null;

        public void AssignInstance(TapBehaviour instance)
        {
            this.instance = instance;
            instance.SetData(this);
            ReloadSkin();
        }

        public TapBehaviour RevokeInstance()
        {
            var result = instance;
            instance = null;
            return result;
        }

        public override ArcEvent Clone()
        {
            return new Tap()
            {
                Timing = Timing,
                TimingGroup = TimingGroup,
                Lane = Lane,
            };
        }

        public override void Assign(ArcEvent newValues)
        {
            base.Assign(newValues);
            Tap e = newValues as Tap;
            Lane = e.Lane;
        }

        public void ResetJudge()
        {
            judgementRequestSent = false;
        }

        public void Rebuild()
        {
        }

        public void ReloadSkin()
        {
            if (instance != null)
            {
                instance.SetSprite(Services.Skin.GetTapSkin(this));
            }
        }

        public int CompareTo(INote<TapBehaviour> other)
        {
            return CompareTo(other as Tap);
        }

        public void UpdateInstance(int timing, double floorPosition, GroupProperties groupProperties)
        {
            if (instance == null)
            {
                return;
            }

            float z = ZPos(floorPosition);
            Vector3 pos = (groupProperties.FallDirection * z) + new Vector3(ArcFormula.LaneToWorldX(Lane), 0, 0);
            Quaternion rot = groupProperties.RotationIndividual;
            Vector3 scl = groupProperties.ScaleIndividual;
            scl.y = ArcFormula.CalculateTapSizeScalar(z) * scl.y;
            instance.SetTransform(pos, rot, scl);

            float alpha = ArcFormula.CalculateFadeOutAlpha(z);
            Color color = groupProperties.Color;
            color.a *= alpha;
            instance.SetColor(color);
        }

        public void ProcessJudgement(JudgementResult result)
        {
            Services.Particle.PlayTapParticle(new Vector3(ArcFormula.LaneToWorldX(Lane), 0), result);
            Services.Particle.PlayTextParticle(new Vector3(ArcFormula.LaneToWorldX(Lane), 0), result);
            Services.Score.ProcessJudgement(result);
            if (instance != null)
            {
                instance.gameObject.SetActive(false);
            }
        }

        public void UpdateJudgement(int timing, GroupProperties groupProperties)
        {
            if (!judgementRequestSent)
            {
                RequestJudgement();
                judgementRequestSent = true;
            }
        }

        private void RequestJudgement()
        {
            Services.Judgement.Request(
                new TapJudgementRequest()
                {
                    ExpireAtTiming = Timing + Values.LostJudgeWindow,
                    AutoAt = Timing,
                    Lane = Lane,
                    Receiver = this,
                });
        }
    }
}