using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using EntityStates;
using UnityEngine;

namespace Skillstates.Squall
{
    internal class SquallMainState : BaseState
    {
        private Animator modelAnimator;

        private bool skill1InputReceived;
        private bool skill2InputReceived;
        private bool skill3InputReceived;
        private bool skill4InputReceived;
        private bool sprintInputReceived;

        public override void OnEnter()
        {
            base.OnEnter();
            modelAnimator = base.GetModelAnimator();
            PlayAnimation("Body", "Hover");
            modelAnimator.SetFloat("Fly.rate", 1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            PerformInputs();
        }

        public override void Update()
        {
            base.Update();
            if (Time.deltaTime <= 0) return;
            UpdateAnimParams();
        }

        private void PerformInputs()
        {
            if (base.isAuthority)
            {
                if(base.inputBank)
                {
                    if(base.rigidbodyMotor)
                    {
                        base.rigidbodyMotor.moveVector = base.inputBank.moveVector * base.characterBody.moveSpeed;
                    }
                    if (base.rigidbodyDirection)
                    {
                        if (base.inputBank.moveVector != Vector3.zero)
                            base.rigidbodyDirection.aimDirection = base.inputBank.moveVector;
                        else
                            base.rigidbodyDirection.aimDirection.y = 0f;
                    }

                    skill1InputReceived = base.inputBank.skill1.down;
                    skill2InputReceived = base.inputBank.skill2.down;
                    skill3InputReceived = base.inputBank.skill3.down;
                    skill4InputReceived = base.inputBank.skill4.down;
                    sprintInputReceived |= base.inputBank.sprint.down;

                    if (base.inputBank.moveVector.magnitude <= 0.5f) sprintInputReceived = false;
                    base.characterBody.isSprinting = sprintInputReceived;
                    if (sprintInputReceived) modelAnimator.SetFloat("Fly.rate", 1.5f);
                    sprintInputReceived = false;
                }

                if(base.skillLocator)
                {
                    if (skill1InputReceived && base.skillLocator.primary) base.skillLocator.primary.ExecuteIfReady();
                    if (skill2InputReceived && base.skillLocator.secondary) base.skillLocator.secondary.ExecuteIfReady();
                    if (skill3InputReceived && base.skillLocator.utility) base.skillLocator.utility.ExecuteIfReady();
                    if (skill4InputReceived && base.skillLocator.special) base.skillLocator.special.ExecuteIfReady();
                }
            }
        }

        private void UpdateAnimParams()
        {
            if (!modelAnimator) return;

            Vector3 moveVector = base.inputBank.moveVector;
            bool value = moveVector != Vector3.zero && base.characterBody.moveSpeed > Mathf.Epsilon;

            modelAnimator.SetBool("isMoving", value);
        }
    }
}
