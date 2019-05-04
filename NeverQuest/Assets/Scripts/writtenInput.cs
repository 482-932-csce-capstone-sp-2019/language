using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class writtenInput : MonoBehaviour
{
    public GameObject line;
    public Shader hehe;


    private Stack<Point> points;
    private List<Stroke> strokes;
    private List<Character> characters;
    List<GameObject> renderers;
    List<GameObject> curBoxes;
    bool isMouseUp;
    bool wait;
    bool insideBox;
    int numPoints;
    int tmpCount;
    float lastXCord = -1;
    int lastStrokeIndex = 0;
    Dictionary<String, int> singleStrokeMappings;


    private float writeBoxLeftBound;
    private float writeBoxUpperBound;
    private float writeBoxRightBound;
    private float writeBoxLowerBound;
    private GuiController guiController;
    private bool useMini = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void WriteBoxOpened(List<GameObject> inputBoxes, bool useMiniVersion = false)
    {
        useMini = useMiniVersion;
        points = new Stack<Point>();
        strokes = new List<Stroke>();
        renderers = new List<GameObject>();
        characters = new List<Character>();
        singleStrokeMappings = new Dictionary<String, int>();
        curBoxes = inputBoxes;
        //print("CUR BOXES COUNT: " + curBoxes.Count);
        singleStrokeMappings.Add("vertical line", 1);
        singleStrokeMappings.Add("horizontal line", 2);
        singleStrokeMappings.Add("g", 11);
        singleStrokeMappings.Add("n", 12);
        singleStrokeMappings.Add("circle", 18);

        guiController = GameObject.FindGameObjectWithTag("GUI").GetComponent<GuiController>();


        isMouseUp = false;
        wait = true;
        insideBox = false;
        numPoints = 0;
        tmpCount = 0;
        lastXCord = -1;
        lastStrokeIndex = 0;
        GameObject curObj = Instantiate(line);
        renderers.Add(curObj);
        LineRenderer drawLine = renderers[tmpCount].GetComponent<LineRenderer>();
        drawLine.sortingLayerName = "Foreground";
        drawLine.material = new Material(hehe);
        drawLine.widthMultiplier = 0.2f;
        drawLine.positionCount = 0;

        if (useMiniVersion)
        {
            RectTransform writeBox = gameObject.GetComponent<RectTransform>();
            float width = writeBox.rect.width;
            float height = writeBox.rect.height;
            Vector3[] v = new Vector3[4];
            writeBox.GetWorldCorners(v);

            writeBoxLeftBound = v[0].x + v[2].x * .025f;
            writeBoxRightBound = v[2].x - v[2].x * .025f;
            writeBoxLowerBound = v[0].y + v[2].y * .025f;
            writeBoxUpperBound = v[2].y - v[2].y * .025f;
        }
        else
        {
            //Get Write Box's height, width, and corners in world coordinates
            RectTransform writeBox = gameObject.GetComponent<RectTransform>();
            float width = writeBox.rect.width;
            float height = writeBox.rect.height;
            Vector3[] v = new Vector3[4];
            writeBox.GetWorldCorners(v);

            //Restrict written input to slightly inside of the write box's borders
            writeBoxLeftBound = v[0].x + v[2].x * .025f;
            writeBoxRightBound = v[2].x - v[2].x * .025f;
            writeBoxLowerBound = v[0].y + v[2].y * .025f;
            writeBoxUpperBound = v[2].y - v[2].y * .025f;
        }
        InvokeRepeating("collectPoints", 0, 0.0167f);
    }

    // Changed Update to invoke repeating
    public void collectPoints()
    {
        GameObject writeBox;
        if(useMini)
        {
            writeBox = guiController.WriteBoxMini;
        }
        else
        {
            writeBox = guiController.WriteBox;
        }

        //If stylus does collect points, else do not
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            wait = false;
            isMouseUp = false;
        }
        else
        {
            isMouseUp = true;
            insideBox = false;
        }
        //Wait if the stylus is not down
        if (!wait)
        {
            float convertedX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            float convertedY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
            foreach (var item in curBoxes)
            {
                Transform inputBox = item.transform;
                Vector3 center = inputBox.position;
                float height = inputBox.up.y;
                float width = inputBox.right.x;
                if ((convertedX < (center[0] + width)) && (convertedX > (center[0] - width))
                       && (convertedY < (center[1] + height)) && (convertedY > (center[1] - height)))
                {
                    insideBox = true;
                }
            }
            if (insideBox)
            {
                if (isMouseUp)
                {
                    if (Input.mousePosition.x > writeBoxLeftBound &&
                        Input.mousePosition.x < writeBoxRightBound &&
                        Input.mousePosition.y > writeBoxLowerBound &&
                        Input.mousePosition.y < writeBoxUpperBound &&
                        writeBox.activeSelf &&
                        points.Count > 1)
                    {
                        //Store stroke and create a new line render for the next line
                        strokes.Add(new Stroke(points.Distinct().ToList()));
                        renderers.Add(Instantiate(line));
                        LineRenderer drawLine = renderers[renderers.Count - 1].GetComponent<LineRenderer>();
                        drawLine.sortingLayerName = "Foreground";
                        drawLine.material = new Material(hehe);
                        drawLine.widthMultiplier = 0.2f;
                        drawLine.positionCount = 0;
                        tmpCount = 0;
                        numPoints += strokes.Count;
                        points.Clear();
                        lastXCord = convertedX;
                        wait = true;
                        insideBox = false;
                    }
                }
                else
                {
                    if (Input.mousePosition.x > writeBoxLeftBound &&
                        Input.mousePosition.x < writeBoxRightBound &&
                        Input.mousePosition.y > writeBoxLowerBound &&
                        Input.mousePosition.y < writeBoxUpperBound &&
                        writeBox.activeSelf)
                    {
                        Point curPoint = new Point(convertedX, convertedY);
                        try
                        {
                            Point topPoint = points.Peek();
                            //Do not add the collected point if it was already added right before
                            if (Point.checkEquality(curPoint, topPoint))
                                return;
                            points.Push(curPoint);
                            if (renderers[Math.Max(strokes.Count, 0)].GetComponent<LineRenderer>() != null)
                            {
                                LineRenderer curLine = renderers[Math.Max(strokes.Count, 0)].GetComponent<LineRenderer>();
                                curLine.sortingLayerName = "Foreground";
                                curLine.positionCount = tmpCount + 1;
                                curLine.SetPosition(tmpCount, new Vector3(convertedX, convertedY, 0));
                                ++tmpCount;
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            points.Push(curPoint);
                            if (renderers[Math.Max(strokes.Count, 0)] != null && renderers[Math.Max(strokes.Count, 0)].GetComponent<LineRenderer>() != null)
                            {
                                LineRenderer curLine = renderers[Math.Max(strokes.Count, 0)].GetComponent<LineRenderer>();
                                curLine.sortingLayerName = "Foreground";
                                curLine.positionCount = tmpCount + 1;
                                curLine.SetPosition(tmpCount, new Vector3(convertedX, convertedY, 0));
                                ++tmpCount;
                            }
                        }
                        insideBox = false;
                        return;
                    }
                }
            }
        }
    }

    public void Clear()
    {
        GameObject[] curStrokes = GameObject.FindGameObjectsWithTag("Stroke");
        foreach (GameObject stroke in curStrokes)
        {
            Destroy(stroke);
        }
        if (strokes != null)
            strokes.Clear();
        if (points != null)
            points.Clear();
        if (characters != null)
            characters.Clear();
        if (renderers != null)
        {
            renderers.Clear();
            GameObject curObj = Instantiate(line);
            renderers.Add(curObj);
            tmpCount = 0;
            LineRenderer drawLine = renderers[tmpCount].GetComponent<LineRenderer>(); 
            drawLine.sortingLayerName = "Foreground";
            drawLine.material = new Material(hehe);
            drawLine.widthMultiplier = 0.2f;
            drawLine.positionCount = 0;
            lastStrokeIndex = 0;
            lastXCord = -1;
        }
    }

    public float strokeLength(List<Point> p)
    {
        double dist = 0;
        for (int i = 0; i < p.Count - 1; i++)
        {
            dist += Math.Sqrt((p[i].x - p[i + 1].x) * (p[i].x - p[i + 1].x) + (p[i].y - p[i + 1].y) * (p[i].y - p[i + 1].y));
        }
        return (float)dist;
    }

    public bool closedFigureTest(List<Point> p)
    {
        double dist = Math.Sqrt((p[0].x - p[p.Count - 1].x) * (p[0].x - p[p.Count - 1].x) + (p[0].y - p[p.Count - 1].y) * (p[0].y - p[p.Count - 1].y));
        if ((dist / strokeLength(p)) - .2 < 1E-7)
            return true;
        return false;
    }

    public bool DLineTest(List<Point> points)
    {
        //least squares line
        double m, b;
        double s1, sx, sy, sxx, sxy;
        s1 = points.Count;
        sx = sy = sxx = sxy = 0;
        foreach (Point p in points)
        {
            sx += p.x;
            sy += p.y;
            sxx += p.x * p.x;
            sxy += p.x * p.y;
        }
        m = (sxy * s1 - sx * sy) / (sxx * s1 - sx * sx);
        b = (sxy * sx - sy * sxx) / (sx * sx - s1 * sxx);

        //error
        double total = 0, avgError;
        foreach (Point p in points)
        {
            double dy = p.y - (m * p.x + b);
            total += dy * dy;
        }
        avgError = Math.Sqrt(total) / points.Count / strokeLength(points);

        //check if line and if slope is around 1 or -1
        if (avgError - 0.03 < 1E-7 && Math.Abs(m) < 1.6 && Math.Abs(m) > 0.625)
            return true;
        return false;
    }

    public double VLineError(List<Point> points)
    {
        double sx = 0;
        foreach (Point p in points)
        {
            sx += p.x;
        }
        double avgx = sx / points.Count;

        //error
        double avgError, totalx;
        totalx = 0;
        foreach (Point p in points)
        {
            double dx = p.x - avgx;
            totalx += dx * dx;
        }
        avgError = Math.Sqrt(totalx) / points.Count / strokeLength(points);
        return avgError;
    }

    public double HLineError(List<Point> points)
    {
        double sy = 0;
        foreach (Point p in points)
        {
            sy += p.y;
        }
        double avgy = sy / points.Count;

        //error
        double avgError, totaly;
        totaly = 0;
        foreach (Point p in points)
        {
            double dy = p.y - avgy;
            totaly += dy * dy;
        }
        avgError = Math.Sqrt(totaly) / points.Count / strokeLength(points);
        return avgError;
    }

    public bool circleTest(List<Point> points)
    {
        // check if closed figure
        if (!closedFigureTest(points)) return false;

        double sx = 0, sy = 0;
        foreach (Point p in points)
        {
            sx += p.x;
            sy += p.y;
        }

        //ideal center
        double ix, iy;
        ix = sx / points.Count;
        iy = sy / points.Count;

        //ideal radius
        double irsq, distsq = 0;
        foreach (Point p in points)
        {
            distsq += ((ix - p.x) * (ix - p.x) + (iy - p.y) * (iy - p.y));
        }
        irsq = distsq / points.Count;

        //error
        double total = 0, avgError;
        foreach (Point p in points)
        {
            double diff = irsq - ((ix - p.x) * (ix - p.x) + (iy - p.y) * (iy - p.y));
            total += diff * diff;
        }
        avgError = Math.Sqrt(total / points.Count);

        if (avgError < 1.5)
            return true;
        return false;
    }

    public bool sharpTest(List<Point> points)
    {
        int leeway = (int)(points.Count / 20.0);

        //check if first half is horizontal and second half is diagonal
        if (HLineError(points.GetRange(leeway, (points.Count / 2) - (2 * leeway))) < 0.05 &&
            DLineTest(points.GetRange(points.Count / 2 + leeway, (points.Count / 2) - (2 * leeway))) &&
            points[leeway].x < points[points.Count / 2].x &&
            points[points.Count / 2].x > points[points.Count - 1].x &&
            points[points.Count / 2].y > points[points.Count - 1].y)
            return true;

        return false;
    }

    public bool gTest(List<Point> points)
    {
        int leeway = (int)(points.Count / 20.0);

        //check if first half is horizontal and second half is vertical
        if (HLineError(points.GetRange(leeway, (points.Count / 2) - (2 * leeway))) < 0.05 &&
            VLineError(points.GetRange(points.Count / 2 + leeway, (points.Count / 2) - (2 * leeway))) < 0.05 &&
            points[leeway].x < points[points.Count / 2].x &&
            points[points.Count / 2].y > points[points.Count - 1].y)
            return true;

        return false;
    }

    public bool nTest(List<Point> points)
    {
        int leeway = (int)(points.Count / 20.0);

        //check if first half is vertical and second half is horizontal
        if (VLineError(points.GetRange(leeway, (points.Count / 2) - (2 * leeway))) < 0.05 &&
            HLineError(points.GetRange(points.Count / 2 + leeway, (points.Count / 2) - (2 * leeway))) < 0.05 &&
            points[leeway].y > points[points.Count / 2].y &&
            points[points.Count / 2].x < points[points.Count - 1].x)
            return true;

        return false;
    }

    // Recognizer
    public List<string> SubmitInput(int templateNum, List<GameObject> inputBoxes)
    {
        this.CancelInvoke();
        //Get input from each box and seperate into characters
        foreach (var item in inputBoxes)
        {
            Transform inputBox = item.transform;
            Vector3 center = inputBox.position;
            float height = inputBox.up.y;
            float width = inputBox.right.x;
            List<Stroke> curStrokes = new List<Stroke>();
            foreach (Stroke stroke in strokes)
            {
                List<Point> curPoints = new List<Point>();

                foreach (Point p in stroke.points)
                {
                    if ((p.x < (center[0] + width)) && (p.x > (center[0] - width))
                        && (p.y < (center[1] + height)) && (p.y > (center[1] - height)))
                    {
                        curPoints.Add(p);
                    }
                }
                if (curPoints.Count != 0)
                {
                    curPoints.Reverse();
                    curStrokes.Add(new Stroke(curPoints));
                }
            }
            if (curStrokes.Count != 0)
            {
                characters.Add(new Character(curStrokes));
            }
        }

        List<string> blockNums = new List<string>();
        int count = 1;
        foreach (Character character in characters)
        {
            //print("CHAR " + count);
            foreach (Stroke stroke in character.strokes)
            {
                foreach (Point p in stroke.points)
                {
                    //print("x = " + p.x + " y = " + p.y);
                }
                // stroke testing
                double vError, hError;
                vError = VLineError(stroke.points);
                hError = HLineError(stroke.points);
                //print(vError + " " + hError);
                if (circleTest(stroke.points))
                {
                    stroke.category = "circle";
                }
                else if (sharpTest(stroke.points))
                {
                    stroke.category = "sharp";
                }
                else if (gTest(stroke.points))
                {
                    stroke.category = "g";
                }
                else if (nTest(stroke.points))
                {
                    stroke.category = "n";
                }
                else if (DLineTest(stroke.points))
                {
                    stroke.category = "diagonal line";
                }
                else if (vError < 0.025)
                {
                    stroke.category = "vertical line";
                    if (vError > hError)
                        stroke.category = "horizontal line";
                }
                else if (hError < 0.025)
                {
                    stroke.category = "horizontal line";
                }

                //print(count + ": " + stroke.category);
                count++;
            }
            //Interpret recognized strokes
            //print("Count: " + character.strokes.Count);
            if ((character.strokes.Count == 1))
            {
                //print("CATEGORY: " + character.strokes[0].category);
                if ((character.strokes[0].category == "raw") || (character.strokes[0].category == "sharp") ||
                    (character.strokes[0].category == "diagonal line"))
                    blockNums.Add("X");
                else
                    blockNums.Add(singleStrokeMappings[character.strokes[0].category].ToString());
            }
            else
            {
                multipleStrokeInterpreter(character, ref blockNums);
            }
            //Handle polylines where there is one stroke
        }
        //Used to test
        count = 1;
        foreach (string num in blockNums)
        {
            //print(count + ") " + num);
            count++;
        }
        this.Clear();
        return blockNums;
    }

    //Process a character with multiple strokes and add mapped korean character num to the return list
    void multipleStrokeInterpreter(Character curChar, ref List<string> nums)
    {
        //need to analyze the strokes, their position, and orientation to determine which korean character
        //to map to
        int numStrokes = curChar.strokes.Count;

        Stroke firstStroke = curChar.strokes[0];
        Stroke secondStroke = curChar.strokes[1];
        if (numStrokes == 2)
        {
            //Handles block nums 3, 5, 7, 9, 13, 17, 19, 21
            //Figure out stroke order and then look at relative positions to map to the correct character
            if (firstStroke.category == "vertical line"
                && secondStroke.category == "horizontal line")
            {
                //3, 7
                if ((Math.Abs(firstStroke.GetPoint(0.5f)[1] - secondStroke.GetStart().y) < 0.2)
                     && Math.Abs(firstStroke.GetPoint(0.5f)[0] - secondStroke.GetStart().x) < 0.2)
                {
                    nums.Add("3");
                }
                else if ((Math.Abs(secondStroke.GetPoint(0.5f)[0] - firstStroke.GetEnd().x) < 0.2)
                          && Math.Abs(secondStroke.GetPoint(0.5f)[1] - firstStroke.GetEnd().y) < 0.2)
                {
                    nums.Add("7");
                }
            }
            else if (firstStroke.category == "horizontal line"
                      && secondStroke.category == "vertical line")
            {
                //nums 5, 9
                if ((Math.Abs(firstStroke.GetEnd().y - secondStroke.GetPoint(0.5f)[1]) < 0.2)
                   && Math.Abs(firstStroke.GetEnd().x - secondStroke.GetPoint(0.5f)[0]) < 0.2)
                {
                    nums.Add("5");
                }
                else if ((Math.Abs(firstStroke.GetPoint(0.5f)[0] - secondStroke.GetStart().x) < 0.2)
                       && Math.Abs(firstStroke.GetPoint(0.5f)[1] - secondStroke.GetStart().y) < 0.2)
                {
                    nums.Add("9");
                }
            }
            else if (firstStroke.category == "horizontal line"
                     && secondStroke.category == "n")
            {
                if (Math.Abs(firstStroke.GetStart().y - secondStroke.GetStart().y) < 0.2
                   && Math.Abs(firstStroke.GetStart().x - secondStroke.GetStart().x) < 0.2)
                    nums.Add("13");
            }
            else if (firstStroke.category == "diagonal line" && secondStroke.category == "diagonal line")
            {
                if (Math.Abs(firstStroke.GetPoint(0.5f)[0] - secondStroke.GetStart().x) < 0.2 &&
                    Math.Abs(firstStroke.GetPoint(0.5f)[1] - secondStroke.GetStart().y) < 0.2)
                    nums.Add("17");
            }
            else if (firstStroke.category == "sharp" && secondStroke.category == "diagonal line")
            {
                // gets midpoint of firstStroke's diagonal
                if (Math.Abs((firstStroke.points[firstStroke.points.Count / 2].x + firstStroke.GetEnd().x) / 2.0f - secondStroke.GetStart().x) < 0.25 &&
                    Math.Abs((firstStroke.points[firstStroke.points.Count / 2].y + firstStroke.GetEnd().y) / 2.0f - secondStroke.GetStart().y) < 0.25)
                    nums.Add("19");
            }
            else if ((firstStroke.category == "sharp" || firstStroke.category == "g")
                    && secondStroke.category == "horizontal line")
            {
                if (Math.Abs((firstStroke.points[firstStroke.points.Count / 2].x + firstStroke.GetEnd().x) / 2.0f - secondStroke.GetEnd().x) < 0.25 &&
                    Math.Abs((firstStroke.points[firstStroke.points.Count / 2].y + firstStroke.GetEnd().y) / 2.0f - secondStroke.GetEnd().y) < 0.25)
                    nums.Add("21");
            }
            else nums.Add("X");
        }
        else if (numStrokes == 3)
        {
            //Handles block nums 4, 6, 8, 10, 14, 15, 20, 22, 24
            Stroke thirdStroke = curChar.strokes[2];

            if (firstStroke.category == "vertical line"
                && secondStroke.category == "horizontal line" && thirdStroke.category == "horizontal line")
            {
                if (Math.Abs(firstStroke.GetPoint(0.33f)[1] - secondStroke.GetStart().y) < 0.2 &&
                    Math.Abs(firstStroke.GetPoint(0.33f)[0] - secondStroke.GetStart().x) < 0.2 &&
                    Math.Abs(firstStroke.GetPoint(0.66f)[1] - thirdStroke.GetStart().y) < 0.2 &&
                    Math.Abs(firstStroke.GetPoint(0.66f)[0] - thirdStroke.GetStart().x) < 0.2)
                    nums.Add("4");
            }
            else if (firstStroke.category == "horizontal line" && secondStroke.category == "horizontal line"
                && thirdStroke.category == "vertical line")
            {
                if (Math.Abs(thirdStroke.GetPoint(0.33f)[1] - firstStroke.GetEnd().y) < 0.2 &&
                    Math.Abs(thirdStroke.GetPoint(0.33f)[0] - firstStroke.GetEnd().x) < 0.2 &&
                    Math.Abs(thirdStroke.GetPoint(0.66f)[1] - secondStroke.GetEnd().y) < 0.2 &&
                    Math.Abs(thirdStroke.GetPoint(0.66f)[0] - secondStroke.GetEnd().x) < 0.2)
                    nums.Add("6");
            }
            else if (firstStroke.category == "vertical line" && secondStroke.category == "vertical line"
                && thirdStroke.category == "horizontal line")
            {
                if (Math.Abs(thirdStroke.GetPoint(0.33f)[1] - firstStroke.GetEnd().y) < 0.2 &&
                    Math.Abs(thirdStroke.GetPoint(0.33f)[0] - firstStroke.GetEnd().x) < 0.2 &&
                    Math.Abs(thirdStroke.GetPoint(0.66f)[1] - secondStroke.GetEnd().y) < 0.2 &&
                    Math.Abs(thirdStroke.GetPoint(0.66f)[0] - secondStroke.GetEnd().x) < 0.2)
                    nums.Add("8");
            }
            else if (firstStroke.category == "horizontal line"
                && secondStroke.category == "vertical line" && thirdStroke.category == "vertical line")
            {
                if (Math.Abs(firstStroke.GetPoint(0.33f)[1] - secondStroke.GetStart().y) < 0.2 &&
                    Math.Abs(firstStroke.GetPoint(0.33f)[0] - secondStroke.GetStart().x) < 0.2 &&
                    Math.Abs(firstStroke.GetPoint(0.66f)[1] - thirdStroke.GetStart().y) < 0.2 &&
                    Math.Abs(firstStroke.GetPoint(0.66f)[0] - thirdStroke.GetStart().x) < 0.2)
                    nums.Add("10");
            }
            else if (firstStroke.category == "g" && secondStroke.category == "horizontal line"
                && thirdStroke.category == "n")
            {
                if (Math.Abs(firstStroke.GetEnd().y - secondStroke.GetEnd().y) < 0.2 &&
                    Math.Abs(firstStroke.GetEnd().x - secondStroke.GetEnd().x) < 0.2 &&
                    Math.Abs(secondStroke.GetStart().y - thirdStroke.GetStart().y) < 0.2 &&
                    Math.Abs(secondStroke.GetStart().x - thirdStroke.GetStart().x) < 0.2)
                    nums.Add("14");
            }
            else if (firstStroke.category == "vertical line" && secondStroke.category == "g"
                && thirdStroke.category == "horizontal line")
            {
                if (Math.Abs(firstStroke.GetStart().y - secondStroke.GetStart().y) < 0.2 &&
                    Math.Abs(firstStroke.GetStart().x - secondStroke.GetStart().x) < 0.2 &&
                    Math.Abs(secondStroke.GetEnd().y - thirdStroke.GetEnd().y) < 0.2 &&
                    Math.Abs(secondStroke.GetEnd().x - thirdStroke.GetEnd().x) < 0.2 &&
                    Math.Abs(firstStroke.GetEnd().y - thirdStroke.GetStart().y) < 0.2 &&
                    Math.Abs(firstStroke.GetEnd().x - thirdStroke.GetStart().x) < 0.2)
                    nums.Add("15");
            }
            else if (firstStroke.category == "horizontal line"
                && secondStroke.category == "sharp" && thirdStroke.category == "diagonal line")
            {
                // like '19' but checks for small horizontal line
                if (Math.Abs((secondStroke.points[secondStroke.points.Count / 2].x + secondStroke.GetEnd().x) / 2.0f - thirdStroke.GetStart().x) < 0.25 &&
                    Math.Abs((secondStroke.points[secondStroke.points.Count / 2].y + secondStroke.GetEnd().y) / 2.0f - thirdStroke.GetStart().y) < 0.25 &&
                    firstStroke.GetStart().x > secondStroke.GetStart().x && firstStroke.GetEnd().x < thirdStroke.GetEnd().x &&
                    firstStroke.GetStart().y > secondStroke.GetStart().y && firstStroke.GetEnd().y > secondStroke.GetStart().y)
                    nums.Add("20");
            }
            else if (firstStroke.category == "horizontal line" && secondStroke.category == "horizontal line"
                && thirdStroke.category == "n")
            {
                if (Math.Abs(firstStroke.GetStart().y - thirdStroke.GetStart().y) < 0.2 &&
                    Math.Abs(firstStroke.GetStart().x - thirdStroke.GetStart().x) < 0.2 &&
                    Math.Abs(secondStroke.GetStart().y - ((thirdStroke.GetStart().y) + thirdStroke.GetEnd().y) / 2.0f) < 0.2 &&
                    Math.Abs(secondStroke.GetStart().x - thirdStroke.GetStart().x) < 0.2)
                    nums.Add("22");
            }
            else if (firstStroke.category == "horizontal line"
                && secondStroke.category == "horizontal line" && thirdStroke.category == "circle")
            {
                if (firstStroke.GetStart().x > secondStroke.GetStart().x && firstStroke.GetEnd().x < secondStroke.GetEnd().x &&
                    firstStroke.GetStart().y > secondStroke.GetStart().y && firstStroke.GetEnd().y > secondStroke.GetStart().y &&
                    secondStroke.GetStart().x < thirdStroke.points[(int)(thirdStroke.points.Count * 0.25)].x && secondStroke.GetEnd().x > thirdStroke.points[(int)(thirdStroke.points.Count * 0.75)].x &&
                    secondStroke.GetStart().y > thirdStroke.GetStart().y)
                    nums.Add("24");
            }
            else nums.Add("X");
        }
        else if (numStrokes == 4)
        {
            //Handles block nums 16 and 23
            Stroke thirdStroke = curChar.strokes[2];
            Stroke fourthStroke = curChar.strokes[3];

            if (firstStroke.category == "vertical line" && secondStroke.category == "vertical line"
                && thirdStroke.category == "horizontal line" && fourthStroke.category == "horizontal line")
            {
                if (Math.Abs(firstStroke.GetPoint(0.5f)[1] - thirdStroke.GetStart().y) < 0.2 &&
                    Math.Abs(firstStroke.GetPoint(0.5f)[0] - thirdStroke.GetStart().x) < 0.2 &&
                    Math.Abs(firstStroke.GetEnd().y - fourthStroke.GetStart().y) < 0.2 &&
                    Math.Abs(firstStroke.GetEnd().x - fourthStroke.GetStart().x) < 0.2 &&
                    Math.Abs(secondStroke.GetPoint(0.5f)[1] - thirdStroke.GetEnd().y) < 0.2 &&
                    Math.Abs(secondStroke.GetPoint(0.5f)[0] - thirdStroke.GetEnd().x) < 0.2 &&
                    Math.Abs(secondStroke.GetEnd().y - fourthStroke.GetEnd().y) < 0.2 &&
                    Math.Abs(secondStroke.GetEnd().x - fourthStroke.GetEnd().x) < 0.2)
                    nums.Add("16");
            }
            else if (firstStroke.category == "horizontal line" && secondStroke.category == "vertical line"
                && thirdStroke.category == "vertical line" && fourthStroke.category == "horizontal line")
            {
                if (Math.Abs(firstStroke.GetPoint(0.33f)[1] - secondStroke.GetStart().y) < 0.2 &&
                    Math.Abs(firstStroke.GetPoint(0.33f)[0] - secondStroke.GetStart().x) < 0.2 &&
                    Math.Abs(firstStroke.GetPoint(0.66f)[1] - thirdStroke.GetStart().y) < 0.2 &&
                    Math.Abs(firstStroke.GetPoint(0.66f)[0] - thirdStroke.GetStart().x) < 0.2 &&
                    Math.Abs(fourthStroke.GetPoint(0.33f)[1] - secondStroke.GetEnd().y) < 0.2 &&
                    Math.Abs(fourthStroke.GetPoint(0.33f)[0] - secondStroke.GetEnd().x) < 0.2 &&
                    Math.Abs(fourthStroke.GetPoint(0.66f)[1] - thirdStroke.GetEnd().y) < 0.2 &&
                    Math.Abs(fourthStroke.GetPoint(0.66f)[0] - thirdStroke.GetEnd().x) < 0.2)
                    nums.Add("23");
            }
            else nums.Add("X");
        }
        else nums.Add("X");
    }
}

public class Point
{
    public float x;
    public float y;

    public Point()
    {
        x = -1;
        y = -1;
    }
    public Point(float xv, float yv)
    {
        x = xv;
        y = yv;
    }
    static public bool checkEquality(Point p1, Point p2)
    {
        if (p1.x == p2.x)
            if (p1.y == p2.y)
                return true;
        return false;
    }
}

public class Stroke
{
    public List<Point> points;
    public String category;

    public Stroke()
    {
        points = new List<Point>();
        category = "raw";
    }

    public Stroke(List<Point> curPoints)
    {
        points = new List<Point>(curPoints.ToList<Point>());
        points.Reverse();
        category = "raw";
    }

    public List<float> GetPoint(float f)
    {
        List<float> tupleCouple = new List<float>();
        tupleCouple.Add(this.GetEnd().x * f + this.GetStart().x * (1 - f));
        tupleCouple.Add(this.GetEnd().y * f + this.GetStart().y * (1 - f));
        return tupleCouple;
    }

    public Point GetStart()
    {
        return points[0];
    }

    public Point GetEnd()
    {
        return points[points.Count - 1];
    }
}

public class Character
{
    public List<Stroke> strokes;
    public Character()
    {
        strokes = new List<Stroke>();
    }
    public Character(List<Stroke> curStrokes)
    {
        strokes = new List<Stroke>(curStrokes);
    }
}