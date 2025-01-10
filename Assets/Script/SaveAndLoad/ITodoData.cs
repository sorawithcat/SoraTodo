using System.Drawing;

public interface ITodoData
{
    string GetTitle();
    Color GetTitleColor();
    Color GetStartColor();
    Color GetEndColor();
    ClearFX GetClearFX();
    TimingType GetTimingType();
    AlarmType GetAlarmType();
    float GetCountdownTime();

}