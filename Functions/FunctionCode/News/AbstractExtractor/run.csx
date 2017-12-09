#r "Newtonsoft.Json"

using System;
using System.Net;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);
    string query = data.query;
    string text = data.text;

    log.Info(query);

            ManipulateQuery findWords = new ManipulateQuery();
            List<int> splitOnKeyWord = findWords.GetAllIndexes(query, @"OR|AND");
            List<string> words = new List<string>();

            if (splitOnKeyWord.Count != 0)
            {
                List<string> roughWords = findWords.SplitQuery(splitOnKeyWord, query);

                foreach (string word in roughWords)
                {
                    List<string> phrases = findWords.CleanWords(word);
                    foreach (string phrase in phrases)
                    {
                        words.Add(phrase);
                    }
                }
            }
            else
            {
                List<string> phrases = findWords.CleanWords(query);
                foreach (string phrase in phrases)
                {
                    words.Add(phrase);
                }
            }

            if(words.Count == 0)
            {
                throw new ArgumentException("No keywords found");
            }

            SearchText searchText = new SearchText();

            dynamic newAbstractMatches = searchText.AbstractSelector(words, text);




    return req.CreateResponse(HttpStatusCode.OK, new {
        Snippet = newAbstractMatches.extract,
        Matches = newAbstractMatches.items
    });

}

    public class ManipulateQuery
    {
        public List<int> GetAllIndexes(string query, string tomatch)
        {
            List<int> indexes = new List<int>();

            if (String.IsNullOrEmpty(query))
                throw new ArgumentException("The query is null or empty");

            char[] specialCharacters = { '(', ')', ' ', '"' };


            foreach (Match match in Regex.Matches(query, tomatch))
            {
                bool startsOr = false;
                bool endsOr = false;
                if (match.Index != 0)
                {
                    int startingIndex = query[match.Index - 1];
                    foreach (char special in specialCharacters)
                    {
                        if (startingIndex == special)
                        {
                            startsOr = true;
                            continue;
                        }
                    }
                }
                else
                {
                    startsOr = true;
                }

                if (query.Length > match.Index + match.Length)
                {
                    int endingIndex = query[match.Index + match.Length];
                    foreach (char special in specialCharacters)
                    {
                        if (endingIndex == special)
                        {
                            endsOr = true;
                            continue;
                        }
                    }
                }
                else
                {
                    endsOr = true;
                }

                if (startsOr && endsOr)
                {
                    indexes.Add(match.Index);
                }

            }

            return indexes;

        }

        public List<string> SplitQuery(List<int> indexes, string query)
        {
            List<string> words = new List<string>();

            int length;
            int currentIndex;
            int previousItem;
            int lengthOfPrevious;

            foreach (int i in indexes)
            {
                if (query[i] == 'O')
                {
                    length = 2;
                }
                else
                {
                    length = 3;
                }


                if (query.Length == length)
                {
                    break;
                }
                else if (indexes[0] == i)
                {
                    if (i != 0)
                    {
                        words.Add(query.Substring(0, i));
                    }

                    if (indexes.Count == 1)
                    {
                        words.Add(query.Substring(i + length, query.Length - (i + length)));
                        break;
                    }
                    continue;
                }
                else if (indexes.Last() == i)
                {
                    currentIndex = indexes.IndexOf(i);
                    previousItem = indexes[currentIndex - 1];

                    if (query[previousItem] == 'O')
                    {
                        lengthOfPrevious = 2;
                    }
                    else
                    {
                        lengthOfPrevious = 3;
                    }

                    words.Add(query.Substring(previousItem + lengthOfPrevious, i - previousItem - lengthOfPrevious));

                    if (query.Length > i + length)
                    {
                        words.Add(query.Substring(i + length, query.Length - (i + length)));
                    }
                    continue;
                }
                else
                {
                    //Works out words to the left
                    currentIndex = indexes.IndexOf(i);
                    previousItem = indexes[currentIndex - 1];

                    if (query[previousItem] == 'O')
                    {
                        lengthOfPrevious = 2;
                    }
                    else
                    {
                        lengthOfPrevious = 3;
                    }

                    words.Add(query.Substring(previousItem + lengthOfPrevious, i - previousItem - lengthOfPrevious));
                }
            }
            return words;
        }

        public List<string> CleanWords(string word)
        {
            List<string> words = new List<string>();

            StringBuilder sb = new StringBuilder();

            bool isOpenQuote = false;
            bool isExclusion = false;

            foreach (char c in word)
            {
                switch (c)
                {
                    case '-':
                        isExclusion = !isExclusion;
                        break;
                    case '(':
                        break;
                    case ')':
                        if (!isExclusion)
                        {
                            if (sb.Length > 0)
                            {
                                words.Add(sb.ToString());
                                sb.Clear();
                                break;
                            }
                            break;
                        }
                        break;
                    case '"':
                        isOpenQuote = !isOpenQuote;
                        if (!isExclusion)
                        {
                            if (!isOpenQuote)
                            {
                                if (sb.Length > 0)
                                {
                                    words.Add(sb.ToString());
                                    sb.Clear();
                                }
                                break;
                            }
                        }
                        break;
                    case ' ':
                        if (isOpenQuote)
                        {
                            if (!isExclusion)
                            {
                                sb.Append(c);
                                break;
                            }
                            break;
                        }
                        if (!isExclusion)
                        {
                            if (sb.Length > 0)
                            {
                                words.Add(sb.ToString());
                                sb.Clear();
                                break;
                            }
                            break;
                        }
                        else
                        {
                            isExclusion = !isExclusion;
                            break;
                        }
                    default:
                        if (!isExclusion)
                        {
                            sb.Append(c);
                            break;
                        }
                        break;
                }
            }
            if (sb.Length > 0)
            {
                words.Add(sb.ToString());
                sb.Clear();
            }
            return words;
        }

    }

public class SearchText
{
    public object AbstractSelector(List<string> keyWords, string text)
    {
        string snippet = null;
        bool snippetFlag = false;
        List<string> matchedWords = new List<string> { };

        foreach (string word in keyWords)
        {
            int mentionIndex;
            int i = 1;
            int wordCount = 0;
            int startIndex = 0;
            int endIndex = 0;
            int keyWordCount = keyWords.Count();

            Match mention = Regex.Match(text, word, RegexOptions.IgnoreCase);
             if (Regex.IsMatch(text, word, RegexOptions.IgnoreCase) && snippetFlag == false)
            {
                matchedWords.Add(word);
                mentionIndex = mention.Index;
            }
            else if (keyWords[keyWordCount - 1] == word  && snippetFlag == false)
            {
                if(Regex.IsMatch(text, word, RegexOptions.IgnoreCase))
                {
                    matchedWords.Add(word);
                }
                mentionIndex = 0;
            }
            else if (Regex.IsMatch(text, word, RegexOptions.IgnoreCase) && snippetFlag == true)
            {
                matchedWords.Add(word);
                continue;
            }
            else
            {
                continue;

            }


            if (mentionIndex == 0)
            {
                startIndex = 0;
            }
            else
            {
                while (wordCount < 25)
                {
                    if (text[mentionIndex - i] == '.' | text[mentionIndex - i] == '\\' | text[mentionIndex - i] == '>' | text[mentionIndex - i] == '?' | text[mentionIndex - i] == '!')
                    {
                        startIndex = mentionIndex - i + 1;
                        break;
                    }
                    else if (text[mentionIndex - i] == ' ')
                    {
                        wordCount = wordCount + 1;
                        startIndex = mentionIndex - i + 1;
                        i = i + 1;
                        continue;
                    }
                    else if (mentionIndex - i == 0)
                    {
                        startIndex = 0;
                        break;
                    }
                    else
                    {
                        i = i + 1;
                        continue;
                    }
                }
            }

            i = 1;

            if (wordCount < 25)
            {
                while (wordCount < 25)
                {
                    if (text[mentionIndex + i] == ' ')
                    {
                        wordCount = wordCount + 1;
                        endIndex = mentionIndex + i;
                        i = i + 1;
                    }
                    else if (mentionIndex + i == text.Length - 1)
                    {
                        endIndex = mentionIndex + i;
                        break;
                    }
                    else
                    {
                        i = i + 1;
                        continue;
                    }
                }
            }
            else
            {
                endIndex = mentionIndex + word.Length;
            }

            snippet = text.Substring(startIndex, endIndex - startIndex + 1);
            if (!(snippet.Last() == '.'))
            {
                snippet = snippet + "...";
            }

            snippetFlag = true;
        }

        GetObject response = new GetObject();
        response.extract = snippet;
        response.items = new List<Item>();

         foreach (string item in matchedWords)
         {
            response.items.Add(new Item { match = item });                
         }

        return response;
    }

    public class GetObject
    {
        public string extract;
        public List<Item> items { get; set; }
    }

     public class Item
    {
        public string match { get; set; }
    }


    }
