using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class StringInterpreter : MonoBehaviour
{
    // Start is called before the first frame update
    public string translate(string inString)
    {
        string outString = inString;
        Regex rx = new Regex(@"[\\%]");
        MatchCollection matches = rx.Matches(inString);
        if (matches.Count != 0) {

        }
        return outString;

    }
}
