using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter.Game
{
    public class PlayerCamera : MonoBehaviour
    {
        public Transform player;
        public Vector3 difference;

        void FollowToTarget()
        {
            if (player == null)
                return;
            transform.position = player.position + difference;
        }

        private void Update()
        {
            FollowToTarget();
        }
    }
}
