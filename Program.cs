using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Configuration;
using System.Text.RegularExpressions;

namespace WordCount
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathSource = Path.Combine(Directory.GetCurrentDirectory(), "wc_input");
            string pathDestination = Directory.GetCurrentDirectory();                      

            if(! Directory.Exists(pathSource))
            {
                Console.WriteLine("wc_input folder does not exist." + pathSource);
            }           

            else
            {               
                try
                {
                    #region File lookup
                    DirectoryInfo DirInfo = new DirectoryInfo(pathSource);

                    if (DirInfo.GetFiles("*").Length == 0)
                    {
                        Console.WriteLine("No file exists.");
                    }
                    #endregion

                    #region Reading from files and hashtable initialization

                    Hashtable WordCountsHash = new Hashtable();

                    List<int> lWordCountsPerLine = new List<int>();

                    int MaxKeyLength = 0;
                    
                    foreach (string fileName in Directory.GetFiles(pathSource).OrderBy(x => new FileInfo(x).FullName))
                    {
                        using (StreamReader sr = new StreamReader(fileName))
                        {
                            String line;
                            
                            while ((line = sr.ReadLine()) != null)
                            {                                
                                string[] Words = line.TrimEnd(new char[] { ',', ' ', '.' }).Split(new Char[] { ' ', ':', '\t' });

                                if (Words.Length == 1 && Words[0] == string.Empty)
                                    lWordCountsPerLine.Add(0);

                                else 
                                    lWordCountsPerLine.Add(Words.Length);

                                foreach (string Word in Words)
                                {

                                    Regex pattern = new Regex("[-\'’;,.\"?!()]");

                                    string TrimmedWord = pattern.Replace(Word.TrimEnd(' '), "").ToLower();
                                    
                                    if (TrimmedWord != string.Empty)
                                    {
                                        if (!WordCountsHash.ContainsKey(TrimmedWord))
                                        {
                                            WordCountsHash.Add(TrimmedWord, 1);

                                            if (TrimmedWord.Length >= MaxKeyLength)
                                                MaxKeyLength = TrimmedWord.Length;
                                        }
                                        else
                                        {
                                            int oldVal = (int)WordCountsHash[TrimmedWord];
                                            WordCountsHash[TrimmedWord] = oldVal + 1;
                                        }
                                    }
                                }                           
                            }
                        }                   
                    }

                    #endregion

                    #region sorting Hashtable key

                    ArrayList KeysArraylist = new ArrayList(WordCountsHash.Keys);
                    KeysArraylist.Sort();

                    #endregion

                    #region find median

                    List<int> lWordCountsPerLineSorted = new List<int>();
                    List<double> RunningMedian = new List<double>();

                    int length = 0;
                                        
                    foreach (var item in lWordCountsPerLine)
                    {
                        lWordCountsPerLineSorted.Add(item);
                        lWordCountsPerLineSorted.Sort();
                        
                        length = lWordCountsPerLineSorted.Count ;
                        if(length % 2 == 0)
                            RunningMedian.Add((lWordCountsPerLineSorted[length / 2 - 1] + lWordCountsPerLineSorted[length / 2]) / 2.0);
                        else
                            RunningMedian.Add(double.Parse(lWordCountsPerLineSorted[length / 2].ToString()));

                    }

                    #endregion

                    #region writing the result to the output file for WordCount

                    string sResult = string.Empty;

                    string sOutputDirectory = "";

                    sOutputDirectory = Path.Combine(pathDestination, "wc_output");

                    if (!Directory.Exists(sOutputDirectory))
                    {
                        System.IO.Directory.CreateDirectory(sOutputDirectory);
                    }

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(Path.Combine(sOutputDirectory, "wc_result.txt")))
                    {
                        foreach (Object obj in KeysArraylist)
                    {
                        sResult = String.Format("{0}\t{1}", obj.ToString().PadRight(MaxKeyLength), WordCountsHash[obj].ToString());
                            file.WriteLine(sResult);
                            sResult = string.Empty;
                        }                        
                    }      
              
                    #endregion

                    #region writing the result to the output file for Running Median                    

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(Path.Combine(sOutputDirectory, "med_result.txt")))
                    {
                        foreach (double obj in RunningMedian)
                        {
                            file.WriteLine(obj.ToString("N1"));                  
                        }
                    }

                    #endregion
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.ToString());
                }            
            }
        }
    }
}
