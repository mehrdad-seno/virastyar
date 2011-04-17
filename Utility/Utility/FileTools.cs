// Author: Omid Kashefi, Mehrdad Senobari 
// Created on: 2010-March-08
// Last Modified: Omid Kashefi, Mehrdad Senobari at 2010-March-08
//


using System;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace SCICT.Utility.IO
{
    ///<summary>
    /// Generic tools for filing
    ///</summary>
    public class FileTools
    {
        /// <summary>
        /// Find the position (byte index) of the given word in the specified stream.
        /// </summary>
        /// <param name="fstream"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static long GetWordStartPositionInFile(FileStream fstream, string word)
        {
            #region Variables

            byte[] BOM = { 0xEF, 0xBB, 0xBF };
            const int BufferSize = 0x10000;

            byte[] byteBuff = new byte[BufferSize];
            byte[] zeroBuff = new byte[BufferSize];
            char[] charBuff = new char[BufferSize];

            fstream.Seek(0, SeekOrigin.Begin);

            long startPoisiton = 0;

            int readedCount;

            bool wholeStrFound = false;

            Encoding UTF8 = Encoding.UTF8;

            int buffWriteIndex = 0;
            bool eof = false;

            #endregion

            #region Check for BOM - If file is created manually
            if (fstream.Read(byteBuff, 0, 3) == 3)
            {
                bool isEqual = true;
                for (int i = 0; i < BOM.Length; i++)
                {
                    if (BOM[i] != byteBuff[i])
                    {
                        isEqual = false;
                        break;
                    }
                }
                if (isEqual)
                    buffWriteIndex = 0;
                else
                    buffWriteIndex = 3;
            }
            #endregion

            do
            {
                readedCount = fstream.Read(byteBuff, buffWriteIndex, BufferSize - buffWriteIndex);
                eof = (readedCount != BufferSize - buffWriteIndex);

                int lastOperationalIndex = Array.LastIndexOf<byte>(byteBuff, 0x0D);
                if (lastOperationalIndex + 1 != BufferSize && byteBuff[lastOperationalIndex + 1] == 0x0A)
                    ++lastOperationalIndex;

                int charCount = UTF8.GetChars(byteBuff, 0, lastOperationalIndex + 1, charBuff, 0);

                for (int i = 0; i < charCount; i++)
                {
                    #region Check first character
                    if (charBuff[i] == word[0])
                    {
                        if ((i - 1) > 0 && charBuff[i - 1] != 0x0A && charBuff[i - 1] != 0x0D)
                            continue;

                        #region Searsch to find the match
                        wholeStrFound = true;
                        for (var j = 1; j < word.Length; j++)
                        {
                            Debug.Assert(i + j < charBuff.Length && charBuff[i + j] != 0);

                            if (word[j] == charBuff[i + j])
                            {
                                continue;
                            }
                            else
                            {
                                wholeStrFound = false;
                                break;
                            }
                            //else // TODO: Remove this after completion
                            //{
                            //    wholeStrFound = true;
                            //}
                        }

                        #endregion

                        if (wholeStrFound && char.IsWhiteSpace(charBuff[i + word.Length]))
                        {
                            // Calculate Position
                            startPoisiton = (fstream.Position - (readedCount + buffWriteIndex)) + UTF8.GetByteCount(charBuff, 0, i);
                            return startPoisiton;
                        }
                    }
                    #endregion
                }

                buffWriteIndex = BufferSize - (lastOperationalIndex + 1);

                #region Copy Buffers
                Buffer.BlockCopy(byteBuff, lastOperationalIndex + 1, byteBuff, 0, buffWriteIndex);
                Buffer.BlockCopy(zeroBuff, 0, byteBuff, buffWriteIndex, BufferSize - buffWriteIndex);
                Buffer.BlockCopy(zeroBuff, 0, charBuff, 0, BufferSize);
                #endregion

            } while (!eof);

            return -1;
        }

        ///<summary>
        /// Remove a line from file
        ///</summary>
        ///<param name="fstream">Opened file stream</param>
        ///<param name="position">position of line</param>
        public static void RemoveLineFromPosition(FileStream fstream, long position)
        {
            int BufferSize = 0x400;

            byte[] byteBuff = new byte[BufferSize];
            char[] charBuff = new char[BufferSize];

            // 1. Go to next line
            fstream.Seek(position, SeekOrigin.Begin);
            fstream.Read(byteBuff, 0, BufferSize);

            int firstEndOfLine = Array.IndexOf<byte>(byteBuff, 0x0D, 0);

            byte[] wholeOfFile = new byte[fstream.Length - (position + firstEndOfLine + 2)];
            fstream.Seek(position + firstEndOfLine + 2, SeekOrigin.Begin);
            int readedCount = fstream.Read(wholeOfFile, 0, wholeOfFile.Length);
            fstream.Seek(position, SeekOrigin.Begin);
            fstream.Write(wholeOfFile, 0, wholeOfFile.Length);

            fstream.SetLength(position + wholeOfFile.Length);
        }
    }

    ///<summary>
    /// Load dictionary file
    ///</summary>
    public class DictionaryLoader
    {
        protected string m_filename;
        protected StreamReader m_streamReader;
        protected StreamWriter m_streamWriter;
        protected bool m_endOfStream = false;

        ///<summary>
        /// End of Stream
        ///</summary>
        public bool EndOfStream
        {
            get
            {
                return m_endOfStream;
            }
        }

        ///<summary>
        /// Load file
        ///</summary>
        ///<param name="fileName">File name</param>
        ///<returns>True if suucessfully loade, otherwise False</returns>
        public bool LoadFile(string fileName)
        {
            m_filename = fileName;
            if (m_filename.Length == 0)
            {
                return false;
            }

            if (!File.Exists(m_filename))
            {
                return false;
            }

            try
            {
                m_streamReader = new StreamReader(m_filename);
            }
            catch (Exception)
            {
                return false;
            }

            m_endOfStream = false;
            return true;
        }

        ///<summary>
        /// Get next line
        ///</summary>
        ///<param name="line">Line contents</param>
        ///<returns>True if not EOF, False on EOF</returns>
        public bool NextLine(out string line)
        {
            line = "";

            if (m_streamReader == null)
            {
                return false;
            }

            try
            {
                this.m_endOfStream = m_streamReader.EndOfStream;
                if (!m_streamReader.EndOfStream)
                {
                    line = m_streamReader.ReadLine();
                    return true;
                }
                else
                {
                    m_streamReader.Close();
                    m_streamReader = null;
                    return false;
                }
            }
            catch(Exception)
            {
                m_streamReader.Close();
                m_streamReader = null;

                return false;
            }
        }

        /// <summary>
        /// Close Stream Reader
        /// </summary>
        public void CloseFile()
        {
            if (m_streamReader != null)
            {
                m_streamReader.Close();
            }
        }

        ///<summary>
        /// Add a term to dictionary
        ///</summary>
        ///<param name="line">word</param>
        ///<returns>True if word successfully added, otherwise False</returns>
        public bool AddLine(string line)
        {
            if (m_filename.Length == 0)
            {
                return false;
            }

            if (!File.Exists(m_filename))
            {
                return false;
            }

            try
            {
                m_streamWriter = new StreamWriter(m_filename, true);
                m_streamWriter.WriteLine(line);
                m_streamWriter.Close();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        ///<summary>
        /// Add a term to dictionary
        ///</summary>
        ///<param name="line">word</param>
        ///<param name="fileName">File name</param>
        ///<returns>True if word successfully added, otherwise False</returns>
        public bool AddLine(string line, string fileName)
        {
            if (fileName.Length == 0)
            {
                return false;
            }

            if (!File.Exists(fileName))
            {
                return false;
            }

            try
            {
                using (m_streamWriter = new StreamWriter(fileName, true))
                {
                    m_streamWriter.WriteLine(line);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }

    ///<summary>
    /// Load words from dictionary file
    ///</summary>
    public class DictionaryWordLoader : DictionaryLoader
    {
        ///<summary>
        /// Next dictionary term
        ///</summary>
        ///<param name="word">Extracted word</param>
        ///<returns>True if word successfully extracted, False if EOF</returns>
        public virtual bool NextTerm(out string word)
        {
            word = "";

            while (true)
            {
                string line;
                if (!NextLine(out line))
                {
                    return false;
                }

                string[] terms = line.Split('\t');

                if (terms.Length >= 1)
                {
                    if (terms[0].Length > 0)
                    {
                        word = terms[0];

                        return true;
                    }
                }
            }
        }

        ///<summary>
        /// Add a term to dictionary
        ///</summary>
        ///<param name="word">word</param>
        ///<returns>True if word successfully added, otherwise False</returns>
        public virtual bool AddTerm(string word)
        {
            if (!AddLine(word))
            {
                return false;
            }

            return true;
        }

        ///<summary>
        /// Add a term to dictionary
        ///</summary>
        ///<param name="word">word</param>
        ///<param name="fileName">File name</param>
        ///<returns>True if word successfully added, otherwise False</returns>
        public virtual bool AddTerm(string word, string fileName)
        {
            if (!AddLine(word, fileName))
            {
                return false;
            }

            return true;
        }
    }

    ///<summary>
    /// Load words and usage frequency from dictionary file
    ///</summary>
    public class DictionaryWordFreqLoader : DictionaryLoader
    {
        ///<summary>
        /// Pars line's content
        ///</summary>
        ///<param name="word">Extracted word</param>
        ///<param name="freq">Extracted word's usage frequency</param>
        ///<returns>True if word successfully extracted, otherwise False</returns>
        public bool NextTerm(out string word, out int freq)
        {
            word = "";
            freq = 0;

            string line;
            if (!NextLine(out line))
            {
                return false;
            }

            string[] terms = line.Split('\t');

            if (terms.Length >= 2)
            {
                if (terms[0].Length > 0 && terms[1].Length > 0)
                {
                    word = terms[0];

                    if (int.TryParse(terms[1], out freq))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        ///<summary>
        /// Add a term to dictionary
        ///</summary>
        ///<param name="word">word</param>
        ///<param name="freq">word's usage frequency</param>
        ///<returns>True if word successfully added, otherwise False</returns>
        public bool AddTerm(string word, int freq)
        {
            if (!AddLine(word + "\t" + freq.ToString()))
            {
                return false;
            }

            return true;
        }

        ///<summary>
        /// Add a term to dictionary
        ///</summary>
        ///<param name="word">word</param>
        ///<param name="freq">word's usage frequency</param>
        ///<param name="fileName">File name</param>
        ///<returns>True if word successfully added, otherwise False</returns>
        public bool AddTerm(string word, int freq, string fileName)
        {
            if (!AddLine(word + "\t" + freq.ToString(), fileName))
            {
                return false;
            }

            return true;
        }
    }

    ///<summary>
    /// Load words, usage frequency and POS tag from dictionary file
    ///</summary>
    public class DictionaryWordFreqPOSLoader : DictionaryLoader
    {
        ///<summary>
        /// Pars line's content
        ///</summary>
        ///<param name="word">Extracted word</param>
        ///<param name="freq">Extracted word's usage frequency</param>
        ///<param name="pos">Extracted word's POS tag</param>
        ///<returns>True if word successfully extracted, otherwise False</returns>
        public bool NextTerm(out string word, out int freq, out string pos)
        {
            word = "";
            freq = 0;
            pos = "";

            string line;
            if (!NextLine(out line))
            {
                return false;
            }
            
            string[] terms = line.Split('\t');

            if (terms.Length >= 3)
            {
                if (terms[0].Length > 0 && terms[1].Length > 0 && terms[2].Length > 0)
                {
                    word = terms[0];
                    pos = terms[2];

                    if (int.TryParse(terms[1], out freq))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        ///<summary>
        /// Add a term to dictionary
        ///</summary>
        ///<param name="word">word</param>
        ///<param name="freq">word's usage frequency</param>
        ///<param name="pos">word's POS tag</param>
        ///<returns>True if word successfully added, otherwise False</returns>
        public bool AddTerm(string word, int freq, string pos)
        {
            if (!AddLine(word + "\t" + freq.ToString() + "\t" + pos))
            {
                return false;
            }

            return true;
        }
        
        ///<summary>
        /// Add a term to dictionary
        ///</summary>
        ///<param name="word">word</param>
        ///<param name="freq">word's usage frequency</param>
        ///<param name="pos">word's POS tag</param>
        ///<param name="fileName">File name</param>
        ///<returns>True if word successfully added, otherwise False</returns>
        public bool AddTerm(string word, int freq, string pos, string fileName)
        {
            if (!AddLine(word + "\t" + freq.ToString() + "\t" + pos, fileName))
            {
                return false;
            }

            return true;
        }

    }

}
