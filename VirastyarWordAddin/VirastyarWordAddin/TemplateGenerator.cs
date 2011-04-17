using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Core;
using VirastyarWordAddin.Log;
using VirastyarWordAddin.Properties;
using System.Diagnostics;

namespace VirastyarWordAddin
{
    public abstract class TemplateGenerator
    {
        public abstract Template Generate(ThisAddIn thisAddin, string templatePath);

        public static TemplateGenerator GetGenerator(OfficeVersion version)
        {
            if (version == OfficeVersion.Office2003)
                return new Template2003Generator();
            else
                return new Template2007Generator();
        }
    }

    class Template2003Generator : TemplateGenerator
    {
        #region Buttons

        //private CommandBarButton m_btnAddinSettings;
        //private CommandBarButton m_btnCheckSpell;
        //private CommandBarButton m_btnPreCheckSpell;
        //private CommandBarButton m_btnPinglishConvert;
        //private CommandBarButton m_btnPinglishConvertAll;
        //private CommandBarButton m_btnCheckDates;
        //private CommandBarButton m_btnCheckNumbers;
        //private CommandBarButton m_btnCheckPunctuation;
        //private CommandBarButton m_btnCheckAllPunctuation;
        //private CommandBarButton m_btnRefineAllCharacters;
        //private CommandBarButton m_btnAbout;

        //private CommandBarButton m_btnHelp;
        //private CommandBarButton m_btnTip;

        //private CommandBarButton m_btnSuggestSynonym;
        //private CommandBarButton m_btnSuggestRhyme;

        #endregion

        public override Template Generate(ThisAddIn thisAddin, string templatePath)
        {
            Template template = SettingsHelper.LoadTemplate(thisAddin.Application, templatePath);

            CreateMenuAndToolbars(thisAddin, thisAddin.Application, template);

            template.Save();

            return template;
        }

        /// <summary>
        /// Creates the menu and toolbars.
        /// </summary>
        private static void CreateMenuAndToolbars(ThisAddIn thisAddin, Application wordApp, Template template)
        {
            thisAddin.PushOldTemplateAndSetCustom(template);

            try
            {
                #region Create Toolbar and Buttons

                #region Text Toolbar

                /*CommandBar oldTextToolbar = WordUIHelper.FindOldToolbar(wordApp, Constants.TextToolbarName);
                CommandBar textToolbar = WordUIHelper.AddWordToolbar(wordApp, Constants.TextToolbarName);
                WordUIHelper.CopyToolbarSettings(textToolbar, oldTextToolbar);
                WordUIHelper.DeleteOldToolbar(oldTextToolbar);*/

                #region Synonym

                /*m_btnSuggestSynonym = WordUIHelper.MakeANewButton(textToolbar, "پیشنهاد لفات هم‌معنی", 0251, true,
                    new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler
                    (btnSuggestSynonym_Click));

                if (assignShortcuts && Hotkey.TryParse(m_appSettings.Config_Shortcut_SynonymSuggestion, out tmpHotkey))
                {
                    if (!m_hotkeyEngine.RegisterHotkey(tmpHotkey,
                        new EventHandler(SynonymSuggestionHotkey_Pressed)))
                    {
                        m_appSettings.Config_Shortcut_SynonymSuggestion = "";
                    }
                }

                //Also: 0227
                m_btnSuggestRhyme = WordUIHelper.MakeANewButton(textToolbar, "پیشنهاد لفات هم‌قافیه", 0278, true,
                    new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler
                    (btnSuggestRhyme_Click));

                if (assignShortcuts && Hotkey.TryParse(m_appSettings.Config_Shortcut_RhymeSuggestion, out tmpHotkey))
                {
                    if (!m_hotkeyEngine.RegisterHotkey(tmpHotkey,
                        new EventHandler(RhymeSuggestionHotkey_Pressed)))
                    {
                        m_appSettings.Config_Shortcut_RhymeSuggestion = "";
                    }
                }*/

                #endregion

                #region AutoTextInsert

                /*btnInsertAutoText = WordUIHelper.MakeANewButton(textToolbar, "درج خودکار متن", 335, true,
                new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler
                (btnInsertAutoText_Click));*/

                #endregion

                #endregion

                #region Conversion Toolbar

                CommandBar oldConversionToolbar = WordUIHelper.FindOldToolbar(wordApp, Constants.ConversionToolbarName);
                CommandBar conversionToolbar = WordUIHelper.AddWordToolbar(wordApp, Constants.ConversionToolbarName);
                WordUIHelper.CopyToolbarSettings(conversionToolbar, oldConversionToolbar);
                WordUIHelper.DeleteOldToolbar(oldConversionToolbar);

                #region Pinglish
                // Also: 
                CommandBarButton m_btnPinglishConvert = WordUIHelper.MakeANewButton(conversionToolbar, "تبدیل پینگلیش", Resources.IconPinglish, true,
                    "VirastyarPinglishConvert_Action");

                CommandBarButton m_btnPinglishConvertAll = WordUIHelper.MakeANewButton(conversionToolbar, "تبدیل یکباره: پینگلیش", Resources.IconPinglishAll, false,
                    "VirastyarPinglishConvertAll_Action");

                #endregion

                #region Date

                // Aslo: 1992, 1095
                CommandBarButton m_btnCheckDates = WordUIHelper.MakeANewButton(conversionToolbar, "تبدیل تاریخ", Resources.IconDate, true,
                    "VirastyarCheckDates_Action");


                #endregion

                #region Number

                // Also: 0070 -- 0079
                CommandBarButton m_btnCheckNumbers = WordUIHelper.MakeANewButton(conversionToolbar, "تبدیل اعداد", Resources.IconNumberConvertor, false,
                    "VirastyarCheckNumbers_Action");

                #endregion

                #endregion

                #region Refinement Toolbar

                CommandBar oldRefinementToolbar = WordUIHelper.FindOldToolbar(wordApp, Constants.RefinementToolbarName);
                CommandBar refinementToolbar = WordUIHelper.AddWordToolbar(wordApp, Constants.RefinementToolbarName);
                WordUIHelper.CopyToolbarSettings(refinementToolbar, oldRefinementToolbar);
                WordUIHelper.DeleteOldToolbar(oldRefinementToolbar);

                #region Spell

                // Create a button to style selected text.
                // Also: 0161
                CommandBarButton m_btnCheckSpell = WordUIHelper.MakeANewButton(refinementToolbar, "غلط‌یابی", Resources.IconSpell, false,
                    "VirastyarCheckSpell_Action");

                #endregion

                #region PreSpell
                CommandBarButton m_btnPreCheckSpell = WordUIHelper.MakeANewButton(refinementToolbar, "پیش‌پردازش املایی متن", Resources.IconPrespell, false,
                    "VirastyarPreCheckSpell_Action");
                #endregion

                #region Punctuation

                // Also: 0163
                CommandBarButton m_btnCheckPunctuation = WordUIHelper.MakeANewButton(refinementToolbar, "تصحیح نشانه‌گذاری", Resources.IconPunc, true,
                    "VirastyarCheckPunctuation_Action");

                // Also: 0386
                CommandBarButton m_btnCheckAllPunctuation = WordUIHelper.MakeANewButton(refinementToolbar, "تصحیح یکباره نشانه‌گذاری", Resources.IconPuncAll, false,
                    "VirastyarCheckAllPunctuation_Action");

                #endregion

                #region Refine All Chars

                CommandBarButton m_btnRefineAllCharacters = WordUIHelper.MakeANewButton(refinementToolbar, "اصلاح تمامی نویسه‌های متن", Resources.IconCharRefiner, true,
                    "VirastyarRefineAllCharacters_Action");

                #endregion

                #region AutoComplete

                #endregion

                #endregion

                #region Settings Toolbar

                CommandBar oldSettingsToolBar = WordUIHelper.FindOldToolbar(wordApp, Constants.SettingsToolbarName);
                CommandBar settingsToolBar = WordUIHelper.AddWordToolbar(wordApp, Constants.SettingsToolbarName);
                WordUIHelper.CopyToolbarSettings(settingsToolBar, oldSettingsToolBar);
                WordUIHelper.DeleteOldToolbar(oldSettingsToolBar);

                CommandBarButton m_btnAddinSettings = WordUIHelper.MakeANewButton(settingsToolBar, "تنظیمات افزونه", Resources.IconSettings, false,
                    "VirastyarAddinSettings_Action");

                CommandBarButton m_btnTip = WordUIHelper.MakeANewButton(settingsToolBar, "نکته روز", Resources.IconTip, true,
                    "VirastyarTip_Action");

                CommandBarButton m_btnHelp = WordUIHelper.MakeANewButton(settingsToolBar, "راهنما", Resources.IconHelp, false,
                    "VirastyarHelp_Action");

                CommandBarButton m_btnAbout = WordUIHelper.MakeANewButton(settingsToolBar, "درباره", Resources.IconVirastyar, false,
                    "VirastyarAbout_Action");

                #endregion

                #region Update Toolbar

                CommandBar oldUpdateToolbar = WordUIHelper.FindOldToolbar(wordApp, Constants.UpdateToolbarName);
                CommandBar updateToolbar = WordUIHelper.AddWordToolbar(wordApp, Constants.UpdateToolbarName);
                WordUIHelper.CopyToolbarSettings(updateToolbar, oldUpdateToolbar);
                WordUIHelper.DeleteOldToolbar(oldUpdateToolbar);

                CommandBarButton m_btnUpdate = WordUIHelper.MakeANewButton(updateToolbar, "به‌روز رسانی",
                    Resources.IconUpdate, false, "VirastyarUpdate_Action");

                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.DebugException("", ex);
            }

            thisAddin.PopOldTemplate(template);
        }
    }

    public class Template2007Generator : TemplateGenerator
    {
        public override Template Generate(ThisAddIn thisAddin, string templatePath)
        {
            throw new NotImplementedException();
        }
    }
}
