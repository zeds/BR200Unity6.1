using UnityEngine;
using System;
using System.Threading.Tasks;
using Unity.Services.Vivox;

namespace TPSBR
{
    public class VoiceChannelManager : MonoBehaviour
    {
        private string currentChannelName;

        public async Task JoinVoiceChannel(string channelName)
        {
            if (!VivoxService.Instance.IsLoggedIn)
            {
                Debug.LogWarning("Not logged into Vivox");
                return;
            }

            try
            {
                // チャンネルに参加
                await VivoxService.Instance.JoinGroupChannelAsync(channelName, ChatCapability.AudioOnly);

                currentChannelName = channelName;
                Debug.Log($"Joined voice channel: {channelName}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to join voice channel: {e.Message}");
            }
        }

        public async Task LeaveVoiceChannel()
        {
            if (!string.IsNullOrEmpty(currentChannelName))
            {
                try
                {
                    await VivoxService.Instance.LeaveChannelAsync(currentChannelName);
                    currentChannelName = null;
                    Debug.Log("Left voice channel");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to leave voice channel: {e.Message}");
                }
            }
        }
    }
}
