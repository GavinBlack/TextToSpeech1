using IBM.Cloud.SDK.Core.Authentication.Iam;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Google.Cloud.TextToSpeech.V1;
using System.Speech.Synthesis;
using System.Threading;
using System.Web.ModelBinding;
using System.Globalization;
using System.Linq.Expressions;

namespace SE_1
{
    
    class Program
    {
        public static string getText()
        {
            //Gets text from the user
            string text = "";

            Console.Write("Enter the text you want it to speak: ");
            text = Console.ReadLine();

            return text;
        }
        public static void getFileNames(string[] fileNames)
        {
            //Gets the name of the mp3 and json file
            Console.WriteLine("What do you want the name of the mp3 file to be?");
            fileNames[0] = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("What do you want the name of the json file to be?");
            fileNames[1] = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Writing files...");
        }
        public static void setObjectProperties(Text t, string fullText, string[] fileNames)
        {
            //Sets all object properties
            t.text = fullText;
            t.date = Convert.ToString(DateTime.Now);
            t.mp3Name = fileNames[0] + ".mp3";
            t.jsonName = fileNames[1] + ".json";
        }
        public static void start()
        {
            //gets the path to write to
            //Creates new objects for program
            //Initializes variables
            string filePathJson = @"C:\Users\Black\Onedrive\Desktop\";  // <---- change to your desired path
            string filePathMp3 = @"C:\Users\Black\Onedrive\Desktop\";   // <---- change to your desired path
            string text = "";
            string fullText = "";
            string[] fileNames = new string[2];
            char choice = 'y';
            bool active = true;
            Text t = new Text();
            TextToSpeechClient client = TextToSpeechClient.Create();
            VoiceSelectionParams voiceSelection = new VoiceSelectionParams{LanguageCode = "en-US",
                                                                                                                   SsmlGender = SsmlVoiceGender.Male };
            AudioConfig audioConfig = new AudioConfig{ AudioEncoding = AudioEncoding.Mp3 };
            try
            {
                //stays in loop as long as the user wants
                while (active)
                {
                    //gets text, then concatenates it to the full string
                    text = getText();
                    fullText += text + " ";

                    //Asks if user is done entering text
                    Console.WriteLine("Do you want to add more text? y/n");
                    choice = Convert.ToChar(Console.ReadLine());
                    if (choice == 'n')
                    {
                        active = false;
                    }
                    Console.Clear();
                }

                // 0th index is the mp3Name, 1st index is the json name
                getFileNames(fileNames);
                filePathMp3 += fileNames[0] + ".mp3";
                filePathJson += fileNames[1] + ".json";

                StreamWriter sw = new StreamWriter(filePathJson);

                //Gets the input from the full string of text
                SynthesisInput input = new SynthesisInput { Text = fullText };
                setObjectProperties(t, fullText, fileNames);

                //creates the json file with text in it
                string json = JsonConvert.SerializeObject(t);
                
                //writes and closes the text file
                //Converts the text to speech
                SynthesizeSpeechResponse response = client.SynthesizeSpeech(input, voiceSelection, audioConfig);
                using (Stream output = File.Create(@filePathMp3))
                {
                    response.AudioContent.WriteTo(output);
                }
                sw.WriteLine(json);
                sw.Close();
                Console.Clear();
                Console.WriteLine(fileNames[0] + ".mp3 was created at " + filePathMp3);
                Console.WriteLine(fileNames[1] + ".json was created at " + filePathJson);
            }
            //catches all exceptions
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        static void Main(string[] args)
        {
            try
            {
                //checks to see if there are any arguments 
                if (args.Length > 0 && args[0] == "/?")
                {
                    Console.WriteLine("Welcome to the help menu!");
                    Console.WriteLine("You will be prompted to enter a string a text to be spoken");
                    Console.WriteLine("You will also be prompted for the names of the mp3 and json file");
                }
                start();
            }
            
            catch(InvalidOperationException)
            {
                Console.WriteLine("ERROR: Did you add the environment variable for your .json file?");
                Console.WriteLine("Please look at the readme file to fix this error"); 
            }
        }
    }
}
