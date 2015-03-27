using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExpressionExtraction
{
    class BrownCorpusRead
    {
        String filePath;
        String entireDocument;
        Dictionary<String, List<String>> abbreviations;
        Dictionary<String, List<String>> contractions;
        List<String> document;

        public BrownCorpusRead(string path)
        {
            this.filePath = path;
            InitializeMembers();
            
        }

        private void InitializeMembers()
        {
            abbreviations = new Dictionary<String, List<String>>();
            contractions = new Dictionary<String, List<String>>();
            document = new List<string>();
            entireDocument = "";
        }

        public void Start()
        {
            GetAbbreviations();
            GetContractions();
            ReadDocument();
            ReplaceAbbreviations();
            ReplaceContractions();
            CleanDocument();
            WriteDocumentToFile();


        }

        private void WriteDocumentToFile()
        {
            System.IO.StreamWriter file =
              new System.IO.StreamWriter("out" + filePath);
            file.WriteLine(entireDocument);
            file.Flush();
            file.Close();
        }

        private void CleanDocument()
        {
           
            document = entireDocument.Split('‡').ToList();
            entireDocument = "";
            foreach (string line in document)
            {
                Regex r = new Regex(@"[\W]{1,2}\/[\W]{1,2}");
                string newline = r.Replace(line, " ");
               /* r = new Regex(@"[\W]{1,2}\/[\W]{1,2}");
                 newline = r.Replace(newline, " ");*/
                r = new Regex(@"\/[\w\S]+");
                newline = r.Replace(newline, " ");

                entireDocument = entireDocument + newline + "‡" + Environment.NewLine;
            }
        }

        private void ReplaceContractions()
        {
            foreach (KeyValuePair<String, List<String>> kp in contractions)
            {
                Regex r = new Regex(kp.Key);
                entireDocument = r.Replace(entireDocument, kp.Value[0]);     
   
            }

            
        }

        private void ReplaceAbbreviations()
        {
            List<String> convertedMatches = new List<string>();
            List<String> matches = new List<string>();
            
           
            foreach (string line in document)
            {
                Regex r = new Regex(@"([A-Za-z]+\.\s?)+");
                Match m = r.Match(line);
                int matchCount = 0;
               
                while (m.Success)
                {
                    Console.WriteLine("Match: " + m);
                  
                    String match = m.ToString().Trim();
                    if (abbreviations.ContainsKey(match))
                    {
                        matches.Add(match);
                        Regex reg = new Regex(@"\.");
                        match = reg.Replace(match, "\\.");

                        Console.WriteLine(match);
                        convertedMatches.Add(match);
                       
                    }

                    m = m.NextMatch();
                }
            }
            for (int i = 0; i < matches.Count; i++)
            {
                Regex r = new Regex(convertedMatches[i]);
                entireDocument = r.Replace(entireDocument, abbreviations[matches[i]][0]); 
            }
            
            
            

        }

        private void ReadDocument()
        {
                 string line;

            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader(filePath);
            while ((line = file.ReadLine()) != null)
            {
                AddToDocument(line);
            }
            file.Close();
            entireDocument.Remove(entireDocument.Length - 1); //remove last terminator

           
        }

        private void AddToDocument(string line)
        {
            if (line.Trim().Length == 0) return;
            else
            {
                document.Add(line.Trim());
                entireDocument = entireDocument + line + "‡";
            }
            

        }
        

        private void GetAbbreviations()
        {
            string line;

            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader("abbreviations.txt");
            while ((line = file.ReadLine()) != null)
            {
                AddToList(ref abbreviations, line);
            }
            file.Close();
            WriteToFile();

        }

        private void GetContractions()
        {
            string line;

            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader("contractions.txt");
            while ((line = file.ReadLine()) != null)
            {
                AddToList(ref contractions, line);
            }
            file.Close();
          

        }

        private void WriteToFile()
        {
            System.IO.StreamWriter file =
               new System.IO.StreamWriter("abb.txt");
            foreach (KeyValuePair<string, List<string>> abbreviation in abbreviations)
            {
                 file.Write(abbreviation.Key + " : " );
                foreach (String item in abbreviation.Value)
                {
                      file.Write(item + ',');
                }
                file.WriteLine();
            }
            file.Flush();
            file.Close();
        }


        /// <summary>
        /// Add a line to a specific collection
        /// </summary>
        /// <param name="collection">collection to be written to</param>
        /// <param name="line">line to be added</param>
        private void AddToList(ref Dictionary<String, List<String>> collection, string line)
        {
            //split into abbrev and meaning
            string[] terms = line.Split('|');
            //remove words between paranthesis
            Regex rgx = new Regex("\\([\\w\\s]+\\)");
            terms[1] = rgx.Replace(terms[1], "");
            //split terms
            string[] words = terms[1].Split('/');
            List<String> temp = new List<string>();
             for(int i = 0; i<words.Length;i++)
                temp.Add(words[i].Trim());
             if (collection.All(x => x.Key != terms[0]))
                 collection.Add(terms[0], temp);
             else
                 collection[terms[0]].AddRange(temp);
           
        }
    }
}
