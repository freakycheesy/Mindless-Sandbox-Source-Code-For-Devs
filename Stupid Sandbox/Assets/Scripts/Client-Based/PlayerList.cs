using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerList : MonoBehaviour
{
    public float highPoints;

    public TMP_Text Text_playerName;
    public TMP_Text Text_playerPoints;
    public TMP_Text Text_playerDeaths;

    void Update()
    {
        foreach(NetworkPlayer player in GetComponentsInChildren<NetworkPlayer>()) {
            if(player.points > highPoints) {
                highPoints = player.points;
            }

            if(player.points >= highPoints) {
                Text_playerName.text = player.photonView.Owner.NickName;
                Text_playerPoints.text = player.points.ToString();
                Text_playerDeaths.text = player.deathCounter.ToString();
            }
        }
    }
}
