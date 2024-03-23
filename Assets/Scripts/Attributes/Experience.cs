using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes {
    public class Experience : MonoBehaviour {
        [SerializeField] float experiencePoints = 0;

        public void GainExperience(float experiencePoints) {
            this.experiencePoints += experiencePoints;
        }
    }
}