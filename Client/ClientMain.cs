using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace core.Client
{
    public class ClientMain : BaseScript
    {
        public ClientMain()
        {
            Debug.WriteLine("ClientMain called");
        }

        [Command("username")]
        public void onCommandUsername()
        {
            var sharedPlayer = new Shared.Player();
            Debug.WriteLine($"Hi, {sharedPlayer.username}!");
        }
    }
}