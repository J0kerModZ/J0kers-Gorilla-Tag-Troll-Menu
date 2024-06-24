using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace J0kersTrollMenu.Patchers
{
    public class LogEvents : MonoBehaviour
    {
        // Lars + Colossus code THX <3
        // From ColossalCheatMenuV2: https://github.com/ColossusYTTV/ColossalCheatMenuV2/releases/ <3

        private static bool Prefix(byte eventCode, object eventContent, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
        {
            Debug.Log(string.Format("Event code: {0}, Event Context: {1}, RaiseEvent options: {2}, Send Options: {3}", new object[]
            {
                eventCode,
                eventContent,
                raiseEventOptions,
                sendOptions
            }));
            return false;
        }
    }
}
