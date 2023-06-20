using UnityEngine;
using System.Collections;

// /-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\
//
// 						    Type3D 1.0, Copyright © 2017, Ripcord Development
//										   DEMO_ScoreUpdater.cs
//										   info@ripcorddev.com
//
// \-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/

//ABOUT - This script randomly adds a random number to the specified Score3D object to demonstrate the AnimateOnUpdate setting

namespace Type3D
{
    public class DEMO_ScoreUpdater : MonoBehaviour
    {
        float timer;									//The number of seconds since the last score update occured
        float nextUpdate;								//The number of seconds before the next score update will happen

        public Score3D scoreContainer;					//The Score3D object that will be modified on the next update

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // UPDATE
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        void Update ()
        {
            if (timer < nextUpdate)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0.0f;
                nextUpdate = Random.Range(0.5f, 2.5f);

                int randomValue = Random.Range(1, 5000);
                scoreContainer.GetComponent<Score3D>().UpdateScore(randomValue);
            }
        }
    }
}
