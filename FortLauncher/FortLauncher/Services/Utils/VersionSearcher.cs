using FortLauncher.Services.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace FortLauncher.Services.Utils
{
    public class VersionSearcher
    {
        private static List<int> Search(byte[] src, byte[] pattern)
        {
            List<int> indices = new List<int>();

            int srcLength = src.Length;
            int patternLength = pattern.Length;
            int maxSearchIndex = srcLength - patternLength;

            for (int i = 0; i <= maxSearchIndex; i++)
            {
                if (src[i] != pattern[0])
                    continue;

                bool found = true;
                for (int j = 1; j < patternLength; j++)
                {
                    if (src[i + j] != pattern[j])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    indices.Add(i);
                }
            }

            return indices;
        }

        public async static Task<string> GetBuildVersion(string exePath)
        {
            try
            {
                string result = "";
                int numThreads = Environment.ProcessorCount;
                List<int> AllMatchingPos = new List<int>();
                try
                {
                    using (FileStream fileStream = new FileStream(exePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        long fileSize = fileStream.Length;
                        long chunkSize = fileSize / numThreads;

                        Task[] tasks = new Task[numThreads];

                        for (int i = 0; i < numThreads; i++)
                        {
                            int threadIndex = i;
                            long startPosition = i * chunkSize;
                            long endPosition = i == numThreads - 1 ? fileSize : startPosition + chunkSize;

                            tasks[i] = Task.Run(() =>
                            {
                                using (BinaryReader binaryReader = new BinaryReader(new FileStream(exePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                                {
                                    binaryReader.BaseStream.Position = startPosition;
                                    byte[] chunkData = binaryReader.ReadBytes((int)(endPosition - startPosition));
                                   
                             
                                    List<int> matchingPositionsInChunk = Search(chunkData, Encoding.Unicode.GetBytes("++Fortnite+Release-"));

                                    lock (AllMatchingPos)
                                    {
                                        AllMatchingPos.AddRange(matchingPositionsInChunk.Select(pos => pos + (int)startPosition));
                                    }
                                }
                            });
                        }

                        Task.WaitAll(tasks);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }



                if (AllMatchingPos.Count != 0)
                {
                    foreach (int num in AllMatchingPos)
                    {
                        using (BinaryReader chunkBinaryReader = new BinaryReader(new FileStream(exePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                        {
                            chunkBinaryReader.BaseStream.Position = num;

                            byte[] buffer = new byte[100];
                            int bytesRead = chunkBinaryReader.Read(buffer, 0, buffer.Length);
                            string chunkText2 = Encoding.Unicode.GetString(buffer, 0, bytesRead);
                            MessageBox.Show(chunkText2);
                            if (bytesRead >= 12)
                            {
                                string chunkText = Encoding.Unicode.GetString(buffer, 0, bytesRead);

                                Match match = Regex.Match(chunkText, @"\+\+Fortnite\+Release-(\d+(\.\d+){0,2}|Live|Next|Cert)-CL-\d+", RegexOptions.IgnoreCase);

                                if (match.Success)
                                {
                                    result = match.Value;
                                    break;
                                }
                            }
                        }
                    }
                }

                Loggers.Log("VersionSearcher->" + result);

                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "ERROR";
            }

        }
    }
}
