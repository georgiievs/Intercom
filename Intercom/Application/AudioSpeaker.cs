using System.Media;

namespace Intercom.Application
{
    public class AudioSpeaker
    {
        //private readonly SoundPlayer openDoorSoundPlayer = new("Resources\\Sounds\\intercom-open-door.wav");
        private readonly SoundPlayer callingSoundPlayer = new("Resources\\Sounds\\intercom-outdoor.wav");


        public void PlayOpenDoorSound()
        {

        }

        public void StopPlayOpenDoorSound()
        {

        }

        public void PlayCallApartmentSound()
        {
            callingSoundPlayer.PlayLooping();
        }

        public void StopPlayCallApartmentSound()
        {
            callingSoundPlayer.Stop();
        }
    }
}
