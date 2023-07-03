using HomaGames.HomaBelly;

namespace HomaGames.HomaBelly.GameDoctor
{
    public class GameDoctorPopupQueryModel : EditorAnalyticsModelBase
    {
        public GameDoctorPopupQueryModel(bool tried) : base("popup_showed")
        {
            EventCategory = GameDoctorPluginConstants.ANALYTICS_CATEGORY;
            EventValues.Add("tried", tried);
        }
    }
}
