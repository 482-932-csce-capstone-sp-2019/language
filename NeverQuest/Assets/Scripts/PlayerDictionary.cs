using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDictionary : MonoBehaviour
{

    public Dictionary<string, List<Word>> playerDict = new Dictionary<string, List<Word>>();
    public static Dictionary<string, string> numToKorean = new Dictionary<string, string>();
    public static Dictionary<string, string> KoreanToNum = new Dictionary<string, string>();
    

    public class Word
    {
        public Word(string trans, List<Syllable> sybs)
        {
            translation = trans;
            syllables = sybs;

            foreach (Syllable sub in sybs)
            {
                foreach (string kchar in sub.characterNumsList)
                {
                    koreanText += numToKorean[kchar];
                }
            }
        }

        public string koreanText = "";
        public string translation;
        public List<Syllable> syllables;

        public float numsSeen = 0;
        public float numsMissed = 0;
    }

    public class Syllable
    {
        public Syllable(int n, List<string> yeet)
        {
            tempNum = n;
            characterNumsList = yeet;
            foreach (var item in yeet)
            {
                koreanString += numToKorean[item];
            }
        }

        public int tempNum;
        public List<string> characterNumsList;
        public string koreanString = "";

    }

    // Start is called before the first frame update
    public void Start()
    {
        numToKorean = new Dictionary<string, string>();
        if (true)
        {
            numToKorean.Add("1", "ㅣ");
            numToKorean.Add("2", "ㅡ");
            numToKorean.Add("3", "ㅏ");
            numToKorean.Add("4", "ㅑ");
            numToKorean.Add("5", "ㅓ");
            numToKorean.Add("6", "ㅕ");
            numToKorean.Add("7", "ㅗ");
            numToKorean.Add("8", "ㅛ");
            numToKorean.Add("9", "ㅜ");
            numToKorean.Add("10", "ㅠ");
            numToKorean.Add("11", "ㄱ");
            numToKorean.Add("12", "ㄴ");
            numToKorean.Add("13", "ㄷ");
            numToKorean.Add("14", "ㄹ");
            numToKorean.Add("15", "ㅁ");
            numToKorean.Add("16", "ㅂ");
            numToKorean.Add("17", "ㅅ");
            numToKorean.Add("18", "ㅇ");
            numToKorean.Add("19", "ㅈ");
            numToKorean.Add("20", "ㅊ");
            numToKorean.Add("21", "ㅋ");
            numToKorean.Add("22", "ㅌ");
            numToKorean.Add("23", "ㅍ");
            numToKorean.Add("24", "ㅎ");
            numToKorean.Add("X", "?");
        }

        foreach (KeyValuePair<string, string> item in numToKorean)
        {
            if (KoreanToNum.ContainsKey(item.Value))
            {
                KoreanToNum.Add(item.Value, item.Key);
            }
        }

        playerDict = new Dictionary<string, List<Word>>();
        playerDict["Verbs"] = new List<Word>();
        playerDict["Adjective"] = new List<Word>();
        playerDict["Demo"] = new List<Word>();
        playerDict["Food"] = new List<Word>();
        playerDict["Time"] = new List<Word>();
        playerDict["Nouns"] = new List<Word>();
        playerDict["Phrases"] = new List<Word>();
        playerDict["Other"] = new List<Word>();
        playerDict["Colors"] = new List<Word>();

        //*********************************************************
        // > PHRASES
        //*********************************************************
        playerDict["Phrases"].Add(new Word(
         "Hello",
         new List<Syllable>()
         {
                new Syllable(4, new List<string>(){ "18", "3", "12"  }),
                new Syllable(4, new List<string>(){ "12", "6","18"  })
         }));


        //*********************************************************
        // > COLORS
        //*********************************************************
        playerDict["Colors"].Add(new Word(
            "Yellow",
            new List<Syllable>()
            {
                new Syllable(1, new List<string>(){ "12", "7"  }),
                new Syllable(4, new List<string>(){ "14", "3","18"  })
            }));

        playerDict["Colors"].Add(new Word(
            "Red",
            new List<Syllable>()
            {
                new Syllable(2, new List<string>(){ "2", "8", "9"  }),
                new Syllable(2, new List<string>(){ "14", "2","12"  })
            }));
            
        playerDict["Colors"].Add(new Word(
          "Black",
          new List<Syllable>()
          {
                new Syllable(4, new List<string>(){ "11", "5", "15"  }),
                new Syllable(4, new List<string>(){ "18", "2","12"  })
          }));



        //*********************************************************
        // > ADJECTIVE
        //*********************************************************
        playerDict["Adjective"].Add(new Word(
            "Difficult",
            new List<Syllable>()
            {
                new Syllable(3, new List<string>(){ "18", "5"  }),
                new Syllable(3, new List<string>(){ "14", "6"  }),
                new Syllable(2, new List<string>(){ "18", "9","12"})
            }));



        //*********************************************************
        // > DEMO
        //*********************************************************
        playerDict["Demo"].Add(new Word(
          "Test lol",
          new List<Syllable>()
          {
                    new Syllable(1, new List<string>(){ "1", "2"  }),
                    new Syllable(5, new List<string>(){ "12", "11", "11", "12"  })
          }));



        //*********************************************************
        // > TIME
        //*********************************************************
        playerDict["Time"].Add(new Word(
             "Today",
             new List<Syllable>()
             {
                new Syllable(1, new List<string>(){ "18", "7"  }),
                new Syllable(4, new List<string>(){ "12", "2", "14"}),
             }));



        //*********************************************************
        // > FOOD
        //*********************************************************
        playerDict["Food"].Add(new Word(
             "Water",
             new List<Syllable>()
             {
                   new Syllable(2, new List<string>(){ "15", "9", "14"})
             }));

        playerDict["Food"].Add(new Word(
        "Taco",
        new List<Syllable>()
        {
                new Syllable(3, new List<string>(){ "22", "3"}),
                new Syllable(1, new List<string>(){ "21", "7" }),
        }));



        //*********************************************************
        // > NOUNS
        //*********************************************************
        playerDict["Nouns"].Add(new Word(
             "You",
             new List<Syllable>()
             {
                   new Syllable(4, new List<string>(){ "13", "3", "18"}), //temp 4
                   new Syllable(4, new List<string>(){ "17", "1", "12"}), //temp 4
             }));


        //*********************************************************
        // > VERBS
        //*********************************************************
        playerDict["Verbs"].Add(new Word(
             "Will",
             new List<Syllable>()
             {
                   new Syllable(1, new List<string>(){"18","2"}),
                   new Syllable(6, new List<string>(){"1","19","1"}),
             }));

        playerDict["Verbs"].Add(new Word(
        "Are",
        new List<Syllable>()
        {
                   new Syllable(3, new List<string>(){"18","3"}),
                   new Syllable(1, new List<string>(){"14","2"}),
        }));

        playerDict["Verbs"].Add(new Word(
           "Come",
           new List<Syllable>()
           {
                new Syllable(1, new List<string>(){ "18", "7"  }),
                new Syllable(3, new List<string>(){ "13", "3"  })
           }));


        //*********************************************************
        // > OTHER
        //*********************************************************
        playerDict["Other"].Add(new Word(
             "How",
             new List<Syllable>()
             {
                   new Syllable(4, new List<string>(){ "16", "3", "18"}), //temp 4
                   new Syllable(4, new List<string>(){ "16", "5", "16"}), //temp 4
             }));

        playerDict["Other"].Add(new Word(
             "But",
             new List<Syllable>()
             {
                   new Syllable(1, new List<string>(){"11","2"}),
                   new Syllable(3, new List<string>(){"14","5"}),
                   new Syllable(3, new List<string>(){"12","3"}),
             }));

        playerDict["Other"].Add(new Word(
             "No",
             new List<Syllable>()
             {
                   new Syllable(3, new List<string>(){"18","3"}),
                   new Syllable(3, new List<string>(){"12","1"}),
             }));

        playerDict["Other"].Add(new Word(
             "Yeah",
             new List<Syllable>()
             {
                   new Syllable(6, new List<string>(){"12","5", "1"}),
             }));

        playerDict["Other"].Add(new Word(
            "Goodbye",
            new List<Syllable>()
            {
                   new Syllable(4, new List<string>(){"18","3", "12"}),
                   new Syllable(4, new List<string>(){"12","6", "18"}),
            }));
    }

}