using System;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        public class PetrenkoString //A class for Petrenko string, containing text of the string and Petrenko-Goltzman index.
        {
            private double comindex;

            public PetrenkoString(string text, double index)
            {
                Text = text;
                PetrenkoIndex = index;
            }

            public PetrenkoString(string text)
            {
                Text = text;
                PetrenkoIndex = 0;
            }

            public string Text { get; set; }
            public double PetrenkoIndex { get; set; }
            public double Length { get; set; }
        }

        public class EnglishPetrenkoString : PetrenkoString
        {
            
            public EnglishPetrenkoString(string text, double index, double comindex) : base(text, index)
            {
                Text = text;
                CommentaryIndex = comindex;
                PetrenkoIndex = index;
            }
            public EnglishPetrenkoString(string text) : base(text)
            {
                Text = text;
                CommentaryIndex = 0;
                PetrenkoIndex = 0;
            }
            public double CommentaryIndex;
            public double CommentaryLength;

        }

        public class Pair
        {
            public Pair(PetrenkoString RusString, List<EnglishPetrenkoString> EngString)
            {
                RussianString = RusString;
                EnglishString = new List<EnglishPetrenkoString>(EngString);
            }

            public void Display()
            {
                Console.WriteLine(RussianString.Text + " ({0})", RussianString.PetrenkoIndex);
                foreach(var eng in EnglishString)
                {
                    Console.WriteLine();
                    Console.WriteLine("------------->" + eng.Text + " ({0}, {1})", eng.PetrenkoIndex, eng.CommentaryIndex);
                    Console.WriteLine();
                }
            }
            public PetrenkoString RussianString;
            public List<EnglishPetrenkoString> EnglishString;
        }


        public static void GetStringsFromFile(string RuPath, string EnPath, List<PetrenkoString> RuStrings, List<EnglishPetrenkoString> EnStrings)
        {
            StreamReader streamReaderRu = new StreamReader(RuPath);
            StreamReader streamReaderEn = new StreamReader(EnPath);

            try
            {
                string line;
                while ((line = streamReaderRu.ReadLine()) != null)
                {
                    if(line != "") {
                        RuStrings.Add(new PetrenkoString(line));
                    }
                    
                }
                line = null;
                while ((line = streamReaderEn.ReadLine()) != null)
                {
                    if (line!="")
                    {
                        EnStrings.Add(new EnglishPetrenkoString(line));
                    }
                    
                }
                    
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }


        //Petrenko index calulation function for russian petrenko strings.
        static public void CalculatePetrenkoIndex(List<PetrenkoString> Strings)
        {
            foreach (var str in Strings)
            {
                double index = 0.5;
                str.Length = 0;
                foreach (var chr in str.Text)
                {
                    if (Char.IsLetter(chr))
                    {
                        str.PetrenkoIndex += index;
                        index++;
                        str.Length++;
                    }

                }
                str.PetrenkoIndex *= str.Length;
            }
        }
        
        //A function for calculating the Petrenko index for english strings.
        static public void CalculatePetrenkoIndex(List<EnglishPetrenkoString> Strings)
        {
            foreach (var str in Strings)
            {
                double index = 0.5;
                int length = 0;
                int CommentaryIndex = 0;
                string[] Substrings = str.Text.Split('|'); //We are dividing the string into two substrings - the text itself and commentary.
                foreach (var chr in Substrings[0])
                {
                    if (Char.IsLetter(chr))
                    {
                        str.PetrenkoIndex += index;
                        index++;
                        length++;
                    } //It's all the same as the original Petrenko Index calculation.
                }
                index = 0.5;
                str.CommentaryLength = 0;
                int i = 1;
                Substrings[1] = Substrings[1].TrimStart(); //In order to find all empty commentaries and not lose the length of them, we need to only trim the start of the sentence.
                string[] words = Substrings[1].Split(new char[] { ' ', '-' });
                if (Substrings[1]=="") //In case if there are no words in the commentary, this line will be ignored.
                {
                    continue;
                }
                foreach (var word in words)
                {
                    if (i > 5) break; //If there are more than 5 words in commentary, the cycle will break.
                    foreach(var chr in word)
                    {
                        if (Char.IsLetter(chr))
                        {
                            str.CommentaryIndex += index;
                            index++;
                            str.CommentaryLength++;
                        }
                    }
                    i++;
                    
                }
                str.PetrenkoIndex *= length;
                str.CommentaryIndex *= str.CommentaryLength;
            }
        }

       
        static public List<Pair> CompareStrings(List<PetrenkoString> Russian, List<EnglishPetrenkoString> English)
        {
            List<Pair> Pairs = new List<Pair>();
            List<EnglishPetrenkoString> EnglishStrings = new List<EnglishPetrenkoString>(); //Temporary list for containing the matching english lines.
            foreach (var RusStr in Russian)
            {
                EnglishStrings.Clear(); //Each iteration we purge it.
                foreach (var EngStr in English)
                {
                    if(RusStr.PetrenkoIndex == EngStr.PetrenkoIndex + EngStr.CommentaryIndex) //If russian string's Petrenko Index is equal to sum of english string's index and commentary index, we are adding the english string to the temporary list.
                    {
                        EnglishStrings.Add(EngStr);
                    }
                }
                if (EnglishStrings.Count != 0)
                {
                    Pairs.Add(new Pair(RusStr, EnglishStrings)); //We are making a new Pair object, which will include the russian string and all english strings with matching parameters to our task.
                }
                
            }
            return Pairs;
        }
        static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            List<PetrenkoString> RussianStrings = new List<PetrenkoString>();
            List<EnglishPetrenkoString> EnglishStrings = new List<EnglishPetrenkoString>();


            //GetStringFromFile function by default receives the main program arguments, but you can change that by uncommenting the following lines
            //string RuPath = Console.ReadLine();
            //string EnPath = Console.ReadLine();

            GetStringsFromFile(args[0], args[1], RussianStrings, EnglishStrings);

            CalculatePetrenkoIndex(RussianStrings);
            CalculatePetrenkoIndex(EnglishStrings);


            List<Pair> Pairs = CompareStrings(RussianStrings, EnglishStrings);


            if(Pairs.Count == 0)
            {
                Console.WriteLine();
                Console.WriteLine("Pairs were not found.");
                Console.WriteLine();
            } else
            {
                foreach (var pair in Pairs)
                {
                    pair.Display();
                }
            }
            sw.Stop();
            Console.WriteLine("Execution time (in ms): " + sw.ElapsedMilliseconds);
            
            
        }
    }
}