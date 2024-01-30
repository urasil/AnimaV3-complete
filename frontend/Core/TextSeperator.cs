using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace dotnetAnima.Core
{
    public class TextSeperator
    {
        public List<string> SeperatedText { get; private set; }

        public TextSeperator()
        {
            SeperatedText = new List<string>();
        }

        // Yes this can still method can still be broken however that is not the point. Any good text should not break this function.
        public List<string> ReadAndSeparateText()
        {
            SeperatedText.Clear();

            string text = File.ReadAllText("../../VoiceBankingText.txt");
            string[] sentences = Regex.Split(text, @"(?<=[.!?])");

            int totalWords = CountWords(text);
            int targetWordsPerBlock = totalWords < 3000 ? totalWords/10 : 300;

            int currentBlockWordCount = 0;
            string currentBlock = "";

            foreach (string sentence in sentences)
            {
                currentBlock += sentence.Trim() + " ";

                currentBlockWordCount += CountWords(sentence);

                if (currentBlockWordCount >= targetWordsPerBlock)
                {
                    SeperatedText.Add(currentBlock);

                    currentBlock = "";
                    currentBlockWordCount = 0;
                }
            }

            // Add any remaining text as the last block
            if (!string.IsNullOrEmpty(currentBlock))
            {
                SeperatedText.Add(currentBlock);
            }

            // Ensure there are exactly 10 blocks
            while (SeperatedText.Count > 10)
            {
                if (SeperatedText.Count == 11)
                {
                    SeperatedText.RemoveAt(SeperatedText.Count - 1);
                    SeperatedText.RemoveAt(SeperatedText.Count - 2);
                    SeperatedText.Add("The developers clearly weren't careful enough with the text so it had to cut off a few sentences early. Do not worry, this doesn't take away from the quality of the solution. Thank you for banking your voice using Anima v3. You are about to have a voice that is immortalized. Enjoy!");
                }
                else
                {
                    SeperatedText.RemoveAt(SeperatedText.Count - 1);
                }
            }
            
            while(SeperatedText.Count <10)
            {
                if(SeperatedText.Count == 7)
                {
                    SeperatedText.Add("Wow, you are done quite early, please keep hitting the buttton located at the bottom of the window after you are done reading this text.");
                    SeperatedText.Add("We weren't expecting you to be so good at this, hence our preparation was not enough. Congratulations!");
                    SeperatedText.Add("Thank you for using Anima v3. No disease can take away you are, we just wanted to make sure your voice stayed intact too. Enjoy!");
                }
                else
                {
                    if (SeperatedText.Count == 8)
                    {
                        SeperatedText.Add("We weren't expecting you to be so good at this, hence our preparation was not enough. Congratulations!");
                        SeperatedText.Add("Thank you for using Anima v3. No disease can take away you are, we just wanted to make sure your voice stayed intact too. Enjoy!");
                    }
                    else
                    {
                        if (SeperatedText.Count == 9)
                        {
                            SeperatedText.Add("Thank you for using Anima v3. No disease can take away you are, we just wanted to make sure your voice stayed intact too. Enjoy!");
                        }
                        else
                        {
                            SeperatedText.Add("WARNING: THE INPUT TEXT IS NOT GOOD ENOUGH, PLEASE CONSIDER CHANGING IT FOR A BETTER VOICEBANKING EXPERIENCE!");
                        }
                    }
                }
            }

            return SeperatedText;
        }


        private int CountWords(string s)
        {
            return s.Split(new char[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }
    }
}
