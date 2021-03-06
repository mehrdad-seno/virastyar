// Author: Omid Kashefi, Mitra Nasri 
// Created on: 2010-March-08
// Last Modified: Omid Kashefi, Mitra Nasri at 2010-March-08
//

using System.Collections.Generic;

namespace SCICT.NLP.Utility.StringDistance
{
    ///<summary>
    /// Define Language of Current Keyboard Layout
    ///</summary>
    public enum KeyboardLanguages
    {
        ///<summary>
        /// Used as Persian Standard Keyboad Layout 
        ///</summary>
        Persian = 0,
        ///<summary>
        /// Used as English Standard Keyboad Layout
        ///</summary>
        English = Persian + 1
    }


    ///<summary>
    /// Keyboard Layout Data Structure
    ///</summary>
    public class KeyboardConfig
    {
        ///<summary>
        /// In a keyboard layout with 3 rows, it indicates the first row.
        ///</summary>
        public string FirstRowCharacters;
        ///<summary>
        /// In a keyboard layout with 3 rows, it indicates the second row.
        ///</summary>
        public string SecondRowCharacters ;
        ///<summary>
        /// In a keyboard layout with 3 rows, it indicates the third row.
        ///</summary>
        public string ThirdRowCharacters;

        ///<summary>
        /// Keyboard Language
        ///</summary>
        public KeyboardLanguages KeyboardLanguage;
        ///<summary>
        /// Other charachters that does not fit in 3 rowed structure layout like charachters which must typed with Shift 
        ///</summary>
        public List<KeyboardKey> OtherCharacters;

        /// <summary>
        /// If the language has not been specified, Persian will use as default. 
        /// </summary>
        public KeyboardConfig()
        {
            LoadPersianDefault();
        }
        ///<summary>
        /// Class Constructor
        ///</summary>
        ///<param name="language">Keyboard Labguage</param>
        public KeyboardConfig(KeyboardLanguages language)
        {
            this.KeyboardLanguage = language;

            if (language == KeyboardLanguages.Persian)
                LoadPersianDefault();

            if (language == KeyboardLanguages.English)
                LoadEnglishDefault();

        }

        /// <summary>
        /// this function loads Persian default keyboard layout into this config object.
        /// </summary>
        public void LoadPersianDefault()
        {
            this.FirstRowCharacters = "ضصثقفغعهخحجچپ";
            this.SecondRowCharacters = "شسیبلاتنمکگ";
            this.ThirdRowCharacters = "ظطزرذدئو";
            
            this.OtherCharacters = new List<KeyboardKey>();

            //this.OtherCharacters.Add(new KeyboardKey(0f, 3f, 'پ'));         // <'> key ('P' in Farsi) - left top
            this.OtherCharacters.Add(new KeyboardKey(4f, 0f, 'ژ', true));   // <c> key ('Zh' in Farsi) - left bottom
            this.OtherCharacters.Add(new KeyboardKey(6f, 2f, 'آ', true));    // <h> key ('AA' in Farsi) - right top
            this.OtherCharacters.Add(new KeyboardKey(8f, 0f, 'ء', true));   // <\> key ('hamze' in Farsi) - middle bottom
            this.OtherCharacters.Add(new KeyboardKey(7f, 0f, 'أ', true));  // <\> key ('hamze' in Farsi) - middle bottom
            this.OtherCharacters.Add(new KeyboardKey(6f, 0f, 'إ', true));   // <\> key ('hamze' in Farsi) - middle bottom
            this.OtherCharacters.Add(new KeyboardKey(6f, 0f, 'ؤ', true));   // <\> key ('hamze' in Farsi) - middle bottom

        }

        /// <summary>
        /// this function loads English default keyboard layout into this config object.
        /// </summary>
        public void LoadEnglishDefault()
        {
            this.FirstRowCharacters = "qwertyuiop";
            this.SecondRowCharacters = "asdfghjkl";
            this.ThirdRowCharacters = "zxcvbnm";
            this.OtherCharacters = new List<KeyboardKey>();
        }

    }
      
}
