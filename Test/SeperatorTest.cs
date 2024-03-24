using dotnetAnima.Core;
using System;
using System.Collections.Generic;

namespace dotnetAnima.Test
{
    class SeperatorTest
    {
        static void Main(string[] args)
        {
            TextSeperator textSeperator = new TextSeperator("en");

            List<string> separatedText = textSeperator.ReadAndSeparateText();

            Console.WriteLine("Separated Text Blocks:");
            foreach (string block in separatedText)
            {
                Console.WriteLine(block);
                Console.WriteLine("-------------------");
            }
        }
    }
}
