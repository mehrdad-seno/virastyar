namespace VirastyarWordAddin.Configurations
{
    public interface IAddinConfigurationDialog
    {
        //event ShortcutChangedEventHandler ShortcutChanged;
        event RefineAllSettingsChangedEventHandler RefineAllSettingsChanged;
        event SpellCheckSettingsChangedEventHandler SpellCheckSettingsChanged;
    }
}