using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Level12 : LevelBase
{
    protected override void OnFinishCompletionVoiceLines()
    {
        base.TransitionToNextLevel();
    }
}
