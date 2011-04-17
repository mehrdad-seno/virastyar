using System;
using System.Diagnostics;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using stdole;
using VirastyarWordAddin.Log;
using VirastyarWordAddin.Properties;
using System.Drawing;

namespace VirastyarWordAddin
{
    /// <summary>
    /// Helper class to deal with Word-UI
    /// </summary>
    public static class WordUIHelper
    {
        //public static bool AutoMaintainCustomTemplate = false;

        /// <summary>
        /// Makes the A new button.
        /// </summary>
        /// <param name="commandBar">The command bar.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="faceID">The face ID.</param>
        /// <param name="beginGroup">if set to <c>true</c> [begin group].</param>
        /// <param name="clickHandler">The click handler.</param>
        /// <returns></returns>
        public static CommandBarButton MakeANewButton(
            Microsoft.Office.Core.CommandBar commandBar, string caption,
            Bitmap facePic, bool beginGroup, _CommandBarButtonEvents_ClickEventHandler clickHandler)
        {
            //Globals.ThisAddIn.PushOldTemplateAndSetCustom();
            object missing = System.Reflection.Missing.Value;
            CommandBarButton newButton = null;
            try
            {

                newButton = MakeANewButton(commandBar, caption, beginGroup);
                if (newButton != null)
                {
                    newButton.Click += clickHandler;
                    newButton.Picture = (IPictureDisp)AxHost2.GettIPictureDispFromPicture(facePic);
                    newButton.Mask = GetMask(facePic);
                }
            }
            catch (Exception ex)
            {
                LogHelper.DebugException("", ex);
            }

            return newButton;
        }

        public static CommandBarButton MakeANewButton(CommandBar commandBar, string caption, bool beginGroup)
        {
            object missing = System.Reflection.Missing.Value;
            CommandBarButton newButton = null;
            try
            {
                newButton = (CommandBarButton)commandBar.Controls.Add(
                    Microsoft.Office.Core.MsoControlType.msoControlButton,
                    missing, missing, missing, missing);

                newButton.Caption = caption;
                newButton.Tag = caption;
                if (beginGroup)
                    newButton.BeginGroup = true;
            }
            catch(Exception ex)
            {
                LogHelper.DebugException("", ex);
            }
            return newButton;
        }

        public static CommandBarButton MakeANewButton(
            Microsoft.Office.Core.CommandBar commandBar, string caption,
            Bitmap facePic, bool beginGroup, string onAction)
        {
            CommandBarButton newButton = null;
            try
            {
                newButton = MakeANewButton(commandBar, caption, beginGroup);
                if (newButton != null)
                {
                    newButton.OnAction = onAction;
                    newButton.Picture = (IPictureDisp)AxHost2.GettIPictureDispFromPicture(facePic);
                    newButton.Mask = GetMask(facePic);
                }
            }
            catch (Exception ex)
            {
                LogHelper.DebugException("", ex);
            }

            return newButton;
        }

        private static IPictureDisp GetMask(Bitmap facePic)
        {
            Bitmap bmp = new Bitmap(facePic);

            Color back = Color.FromArgb(255, 255, 255, 255);

            for(int x = 0; x < bmp.Size.Width; x++)
                for(int y = 0; y < bmp.Size.Height; y++)
                {
                    if (facePic.GetPixel(x, y).A > 0)
                        bmp.SetPixel(x, y, Color.Black);
                    else
                        bmp.SetPixel(x, y, Color.White);
                }

            return (IPictureDisp)AxHost2.GettIPictureDispFromPicture(bmp);
        }

        /// <summary>
        /// Makes the A new button.
        /// </summary>
        /// <param name="commandBar">The command bar.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="faceID">The face ID.</param>
        /// <param name="beginGroup">if set to <c>true</c> [begin group].</param>
        /// <param name="clickHandler">The click handler.</param>
        /// <returns></returns>
        public static CommandBarButton MakeANewButton(
            Microsoft.Office.Core.CommandBar commandBar, string caption,
            int faceID, bool beginGroup, _CommandBarButtonEvents_ClickEventHandler clickHandler)
        {
            CommandBarButton newButton = null;
            try
            {

                newButton = MakeANewButton(commandBar, caption, beginGroup);
                if (newButton != null)
                {
                    newButton.FaceId = faceID;
                    newButton.Click += clickHandler;
                }
            }
            catch (Exception ex)
            {
                LogHelper.DebugException("", ex);
            }
            
            return newButton;
        }

        /// <summary>
        /// Creates a new toolbar with the given name and adds it to the given MS-Word application instance.
        /// </summary>
        /// <param name="msWordApp">The word instance which the toolbar will be added to.</param>
        /// <param name="toolbarName">Name of the toolbar.</param>
        /// <returns>Instance of the newly created Toolbar</returns>
        public static CommandBar AddWordToolbar(Application msWordApp, string toolbarName)
        {
            //Globals.ThisAddIn.PushOldTemplateAndSetCustom();
            Microsoft.Office.Core.CommandBar toolBar;
            try
            {
                // Create a command bar for the add-in
                object missing = System.Reflection.Missing.Value;
                object menuBar = false;
                toolBar = msWordApp.CommandBars.Add(toolbarName,
                                                    Microsoft.Office.Core.MsoBarPosition.msoBarTop,
                                                    menuBar, missing);
                toolBar.RowIndex = 4;
                toolBar.Visible = true;
            }
            catch(Exception ex)
            {
                LogHelper.DebugException("Unable to create a toolbar", ex);
                return null;
            }

            //Globals.ThisAddIn.PopOldTemplate();
            return toolBar;
        }

        /// <summary>
        /// Copies the toolbar settings.
        /// </summary>
        /// <param name="toolBarDest">Destination toolbar .</param>
        /// <param name="toolBarSrc">Source toolbar .</param>
        public static void CopyToolbarSettings(CommandBar toolBarDest, CommandBar toolBarSrc)
        {
            // TODO: Do we need copy any other property ?!
            if (toolBarSrc == null || toolBarDest == null)
                return;

            toolBarDest.RowIndex = toolBarSrc.RowIndex;
            toolBarDest.Position = toolBarSrc.Position;

            toolBarDest.Left = toolBarSrc.Left;
            toolBarDest.Top  = toolBarSrc.Top;
        }

        /// <summary>
        /// Deletes the old toolbar.
        /// </summary>
        public static void DeleteOldToolbar(CommandBar toolbar)
        {
            //Globals.ThisAddIn.PushOldTemplateAndSetCustom();

            if (toolbar == null)
                return;

            foreach (CommandBarButton cmdButton in toolbar.Controls)
            {
                try
                {
                    cmdButton.Delete(false);
                }
                catch (Exception ex)
                {
                    LogHelper.DebugException("", ex);
                }
            }

            try
            {
                toolbar.Delete();
            }
            catch (Exception ex)
            {
                LogHelper.DebugException("", ex);
            }
            //Globals.ThisAddIn.PopOldTemplate();
        }

        /// <summary>
        /// Deletes the old toolbars.
        /// </summary>
        /// <param name="wordApp"></param>
        /// <param name="toolbarPrefix">Prefix of the toolbars name.</param>
        public static void DeleteOldToolbars(Application wordApp, string toolbarPrefix)
        {
            foreach (CommandBar cmdBar in wordApp.CommandBars)
            {
                if (cmdBar.Name.StartsWith(toolbarPrefix))
                {
                    DeleteOldToolbar(cmdBar);
                }
            }
        }

        /// <summary>
        /// Finds the old toolbar.
        /// </summary>
        /// <param name="wordApp"></param>
        /// <param name="toolbarName">Name of the toolbar.</param>
        /// <returns></returns>
        public static CommandBar FindOldToolbar(Application wordApp, string toolbarName)
        {
            foreach (CommandBar cmdBar in wordApp.CommandBars)
            {
                if (cmdBar.Name == toolbarName)
                {
                    return cmdBar;
                }
            }
            return null;
        }

        private static void TestAllControls(CommandBar commandBar, string prefix)
        {
            // TODO: Remove this method from next release

            object missing = System.Reflection.Missing.Value;
            try
            {
                CommandBarPopup newButton;
                newButton = (CommandBarPopup)commandBar.Controls.Add(
                    Microsoft.Office.Core.MsoControlType.msoControlPopup,
                    missing, missing, missing, missing);

                newButton.Caption = "test";
                newButton.Controls.Add(MsoControlType.msoControlEdit, missing, missing, missing, missing);
            }
            catch (Exception ex)
            {
                // Add code here to handle the exception.
                // return null;
                LogHelper.DebugException("", ex);
            }


            try
            {
                //CommandBarButt newButton;
                /*newButton = (CommandBarPopup)*/
                commandBar.Controls.Add(
                    Microsoft.Office.Core.MsoControlType.msoControlDropdown,
                    missing, missing, missing, missing);

                //newButton.Caption = "test";
                //newButton.Controls.Add(MsoControlType.msoControlEdit, missing, missing, missing, missing);
            }
            catch
            {
                // Add code here to handle the exception.
                // return null;
            }

            try
            {
                _CommandBarActiveX newButton;
                newButton = (_CommandBarActiveX)commandBar.Controls.Add(
                    Microsoft.Office.Core.MsoControlType.msoControlActiveX,
                    missing, missing, missing, missing);

                var lbl = new System.Windows.Forms.Label();
                lbl.Text = "yani mishe ?!";
            }
            catch
            {
                // Add code here to handle the exception.
                //return null;
            }

        }
    }
}
