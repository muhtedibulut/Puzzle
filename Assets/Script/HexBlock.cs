using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexBlock : MonoBehaviour
{
    enum Rotate { ClockWise, CounterClockWise, };

    Rotate direction;
    Vector2 firstTounch;
    Vector2 finalTounch;

    public static int score = 0;
    private float speed = 1.0f;
    private static int rotateCount = 0;
    private static bool click = false;
    private static bool rotate = false;
    private static bool block = false;
    float minSwipeDistX;
    float minSwipeDistY;


    public static int row = 8;
    public static int col = 9;
    public static Vector3[,] pos = new Vector3[row, col];
    public static bool[,] posBool = new bool[row, col];
    public static Vector2[] clickObjPos = new Vector2[3];
    public static Vector3 clickCenter;
    public static bool isBlock = false;

    private List<Color> colors;
    private System.Random random;



    
    void Start()
    {
        colorAdd();
    }

    
    private void colorAdd()
    {
        random = new System.Random();
        colors = new List<Color>();
        colors.Add(Color.magenta);
        colors.Add(Color.red);
        colors.Add(Color.yellow);
        colors.Add(Color.blue);
        colors.Add(Color.green);
    }

    void Update()
    {
        if (rotate)
        {
            animationRotate();
        }

        animationMove(checkPos());
    }

    //Animasyonu sağlayan method
    private void animationMove(GameObject game)
    {
        if (game != null)
        {
            float step = speed * Time.deltaTime;
            game.transform.position = Vector3.MoveTowards(game.transform.position, pos[(int)getPos(game).x, (int)getPos(game).y], step);
            if (Vector3.Distance(game.transform.position, pos[(int)getPos(game).x, (int)getPos(game).y]) == 0)//< 0.0001f )
            {
                posBool[(int)getPos(game).x, (int)getPos(game).y] = true;
            }
        }
        else if (!isBlock)
        {
            posAssign();
        }
    }

    //Tüm objelerin yerine konumlandırıldığını sağlayan method
    private void posAssign()
    {
        for (int i = row - 1; i >= 0; i--)
        {
            for (int j = col - 1; j >= 0; j--)
            {
                if (GameObject.Find(i + "+" + j) != null)
                {
                    GameObject.Find(i + "+" + j).transform.position = pos[i, j];
                }
            }
        }
        isBlock = true;
    }

    //Yok olan yerinin tespit etmeye yarayan method
    private GameObject checkPos()
    {
        for (int i = row - 1; i >= 0; i--)
        {
            for (int j = col - 1; j >= 0; j--)
            {
                if (GameObject.Find(i + "+" + j) != null)
                {
                    if (!posBool[i, j])
                        return GameObject.Find(i + "+" + j);
                }
            }
        }
        return null;
    }

    //Ekrana tıklanıldığında aşamaları başlatacak olan method 
    private void clickOperation()
    {
        distanceCalculate(finalTounch);
        clickPointCalculate();
        clickPointObj();
    }

    //Seçilen objeleri belirtmek için kullanılan dairenin konulması
    private void clickPointObj()
    {
        GameObject hex = (GameObject)Instantiate(Resources.Load("click"), clickCenter, Quaternion.identity);
        hex.name = "click";
        click = true;
    }

    //Seçili üç objenin orta noktasını hesaplayan method
    private Vector3 clickPointCalculate()
    {
        clickCenter = new Vector3(0, 0, 0);
        for (int i = 0; i < clickObjPos.Length; i++)
        {
            clickCenter += pos[(int)clickObjPos[i].x, (int)clickObjPos[i].y];
        }
        clickCenter /= clickObjPos.Length;
        return clickCenter;
    }

    //Tıklanıldığı yere en yakın üç objeyi bulan method
    private void distanceCalculate(Vector2 finalTounch)
    {
        List<GameObject> game = new List<GameObject>();
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = finalTounch;

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (GameObject.Find(i + "+" + j) != null)
                {
                    game.Add(GameObject.Find(i + "+" + j));
                }
            }
        }

        for (int k = 0; k < 3; k++)
        {
            foreach (GameObject go in game)
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
            clickObjPos[k].x = int.Parse(closest.name.Split('+')[0]);
            clickObjPos[k].y = int.Parse(closest.name.Split('+')[1]);
            for (int i = 0; i < game.Count; i++)
            {
                if (game[i].name.Equals(closest.name))
                {
                    game.RemoveAt(i);
                }
            }
            closest = null;
            distance = Mathf.Infinity;
            position = finalTounch;
        }
    }

    //Dönderme işlemini sağlayan method 
    private void animationRotate()
    {
        float step = speed * Time.deltaTime / 25;
        int g1X = (int)clickObjPos[0].x, g1Y = (int)clickObjPos[0].y;
        int g2X = (int)clickObjPos[1].x, g2Y = (int)clickObjPos[1].y;
        int g3X = (int)clickObjPos[2].x, g3Y = (int)clickObjPos[2].y;
        GameObject g1 = GameObject.Find(g1X + "+" + g1Y);
        GameObject g2 = GameObject.Find(g2X + "+" + g2Y);
        GameObject g3 = GameObject.Find(g3X + "+" + g3Y);

        if (direction == Rotate.ClockWise)
        {
            g1.transform.position = Vector3.MoveTowards(g1.transform.position, pos[g2X, g2Y], step);
            g2.transform.position = Vector3.MoveTowards(g2.transform.position, pos[g3X, g3Y], step);
            g3.transform.position = Vector3.MoveTowards(g3.transform.position, pos[g1X, g1Y], step);
            if (Vector3.Distance(g1.transform.position, pos[g2X, g2Y]) < 0.001f && Vector3.Distance(g2.transform.position, pos[g3X, g3Y]) < 0.001f && Vector3.Distance(g3.transform.position, pos[g1X, g1Y]) < 0.001f)
            {
                g1.name = g2X + "+" + g2Y;
                g2.name = g3X + "+" + g3Y;
                g3.name = g1X + "+" + g1Y;

                rotateStage();
            }
        }
        else
        {
            g1.transform.position = Vector3.MoveTowards(g1.transform.position, pos[g3X, g3Y], step);
            g2.transform.position = Vector3.MoveTowards(g2.transform.position, pos[g1X, g1Y], step);
            g3.transform.position = Vector3.MoveTowards(g3.transform.position, pos[g2X, g2Y], step);

            if (Vector3.Distance(g1.transform.position, pos[g3X, g3Y]) < 0.001f && Vector3.Distance(g2.transform.position, pos[g1X, g1Y]) < 0.001f && Vector3.Distance(g3.transform.position, pos[g2X, g2Y]) < 0.001f)
            {
                g1.name = g3X + "+" + g3Y;
                g2.name = g1X + "+" + g1Y;
                g3.name = g2X + "+" + g2Y;

                rotateStage();
            }
        }
    }

    //Dönderme aşamasını başlatan method
    private void rotateStage()
    {
        rotateCount++;
        blockStage();
        if (checkRotate())
        {
            stopRatate();
            blockStage();
        }
    }

    //Bloğun kontrol edildiği aşamayı başalatan method
    private void blockStage()
    {
        checkBlock();
        if (block)
        {
            blockBomb();
            colDown();
            createObj(empytCount());
            block = false;
        }
    }

    //Yeni bloğun üretildiği method
    private void createObj(int count)
    {
        scoreUptade();
        propertyObj(checkEmpty());
        isBlock = false;
    }

    //Scoru hesapla
    private void scoreUptade()
    {
        score += (checkEmpty().Count * 5);
    }

    //Yeni üretilecek olan bloğun özelliklerinin verildiği method (renk, konum)
    private void propertyObj(List<Vector2> loc)
    {
        for (int i = 0; i < loc.Count; i++)
        {
            GameObject hex = (GameObject)Instantiate(Resources.Load("Hex"), new Vector3(pos[(int)loc[i].x, (int)loc[i].y].x, 3, 0), Quaternion.identity);//,pos[(int)loc[i].x,(int)loc[i].y],Quaternion.identity);
            hex.GetComponent<Transform>().eulerAngles = new Vector3(0, 0, 90);
            hex.name = (int)loc[i].x + "+" + (int)loc[i].y;
            hex.GetComponent<Renderer>().material.color = colors[UnityEngine.Random.Range(0, colors.Count)];
        }
    }


    private void createBomb(Vector2 vector2)
    {
        GameObject bombObj = (GameObject)Instantiate(Resources.Load("Bomb"), new Vector3(pos[(int)vector2.x, (int)vector2.y].x, 3, 0), Quaternion.identity);//,pos[(int)loc[i].x,(int)loc[i].y],Quaternion.identity);
        bombObj.GetComponent<Transform>().eulerAngles = new Vector3(0, 0, 90);
        bombObj.name = (int)vector2.x + "+" + (int)vector2.y;
        bombObj.GetComponent<Renderer>().material.color = colors[UnityEngine.Random.Range(0, colors.Count)];

    }

    //Boş olan blokların sayısının sayıldığı method
    private int empytCount()
    {
        int count = 0;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (!posBool[i, j])
                {
                    count++;
                }
            }
        }
        return count;
    }

    //Üçlü blokları kontrol edecek olan method
    private void checkBlock()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (j % 2 != 0)//Çukur
                {
                    if (i != row - 1)//En son satır değilse
                    {
                        //Sol tarafındakini kontrol et
                        checkStage(new[] { new Vector2(i, (j - 1)), new Vector2(i, j), new Vector2((i + 1), (j - 1)) });
                        //Sağ tarafındakini kontrol et
                        checkStage(new[] { new Vector2(i, (j + 1)), new Vector2(i, j), new Vector2((i + 1), (j + 1)) });
                    }
                    else//En son satır ise
                    {
                        //Sol tarafındakini kontrol et
                        checkStage(new[] { new Vector2(i, (j - 1)), new Vector2(i, j), new Vector2((i - 1), (j)) });
                        //Yukarı tarafındakini kontrol et
                        checkStage(new[] { new Vector2((i - 1), (j)), new Vector2(i, j), new Vector2((i), (j + 1)) });
                    }
                }
                else//Tepe
                {
                    if (i != 0 && i != row - 1 && j != 0 && j != col - 1)//İlk ve son satır değil ise (Çukur kontrol ediyor)
                    {
                        //Sağ tarafındakini kontrol et
                        checkStage(new[] { new Vector2(i, (j - 1)), new Vector2(i, j), new Vector2((i - 1), (j - 1)) });
                        //Sol tarafındakini kontrol et
                        checkStage(new[] { new Vector2(i, (j + 1)), new Vector2(i, j), new Vector2((i - 1), (j + 1)) });
                    }
                    if (j == 0)//İlk sutun ise sağ tarafı kontrol et
                    {
                        if (i != 0)
                        {
                            checkStage(new[] { new Vector2((i - 1), (j + 1)), new Vector2(i, j), new Vector2((i), (j + 1)) });
                        }
                        else//Köşedeyse
                        {
                            checkStage(new[] { new Vector2((i + 1), (j)), new Vector2(i, j), new Vector2((i), (j + 1)) });
                        }
                    }
                    else if (j == col - 1)//Son sutun ise sol tarafı kontrol et
                    {
                        if (i != 0)
                        {
                            checkStage(new[] { new Vector2((i - 1), (j - 1)), new Vector2(i, j), new Vector2((i), (j - 1)) });
                        }
                        else//Köşedeyse
                        {
                            checkStage(new[] { new Vector2(i, (j - 1)), new Vector2(i, j), new Vector2((i + 1), (j)) });
                        }
                    }
                }
            }
        }
    }

    //Üçlü blokları kontrol aşamasının başladığı aşama
    private void checkStage(Vector2[] vector2)
    {
        Color[] color = colorEdit(vector2);//Üçlü bloğu düzenle
        if (checkColor(color))// Üçlü bloğun hepsi aynı renkte mi
        {
            changeBool(vector2);
            block = true;
        }
    }

    //Belirlenen objeleri patlat
    private void blockBomb()
    {
        int count = 0;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (!posBool[i, j])
                {
                    count++;
                    Destroy(GameObject.Find(i + "+" + j));
                }
            }
        }
    }

    //Üçlü bloğun düzenlendiği  method
    private Color[] colorEdit(Vector2[] vector2)
    {
        Color[] color = new Color[vector2.Length];
        for (int i = 0; i < vector2.Length; i++)
        {
            if (GameObject.Find((int)vector2[i].x + "+" + (int)vector2[i].y) != null)
            {
                color[i] = GameObject.Find((int)vector2[i].x + "+" + (int)vector2[i].y).GetComponent<Renderer>().material.color;
            }
        }
        return color;

    }

    //Patlayan blokların konumlarının olduğu yerleri "false" yap
    private void changeBool(Vector2[] vector2)
    {
        for (int i = 0; i < vector2.Length; i++)
        {
            posBool[(int)vector2[i].x, (int)vector2[i].y] = false;
        }
    }

    //Üçlü bloğun hepsinin aynı olup olmadığını kontrol eden method
    private bool checkColor(Color[] color)
    {
        Color clr = color[0];
        for (int i = 1; i < color.Length; i++)
        {
            if (!(clr == color[i])) return false;
        }
        stopRatate();
        return true;
    }

    //Üçlü bloğun dönüşünü kontrol eden method
    private bool checkRotate()
    {
        if (rotateCount >= 3)
        {
            return true;
        }
        return false;
    }

    //Dönüşü durdurmaya yarayan method
    private void stopRatate()
    {
        rotateCount = 0;
        click = false;
        rotate = false;
        Destroy(GameObject.Find("click"));
    }

    //Patlayan blokların konu bilgisini tutan method
    private List<Vector2> checkEmpty()
    {
        List<Vector2> pos = new List<Vector2>();
        for (int i = row - 1; i >= 0; i--)
        {
            for (int j = col - 1; j >= 0; j--)
            {
                if (!posBool[i, j])
                {
                    pos.Add(new Vector2(i, j));
                }
            }
        }
        return pos;
    }

    private void showMatrix()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                print(i + "X" + j + " => " + pos[i, j]);
            }
        }
    }

    //bloklar patladıktan sonra kalan blokların aşağı düşmesini sağlayan method
    private void colDown()
    {
        int emptyCount;

        for (int j = col - 1; j >= 0; j--)
        {
            emptyCount = 0;
            for (int i = row - 1; i >= 0; i--)
            {
                if (posBool[i, j])
                {
                    GameObject.Find(i + "+" + j).transform.position = pos[i + emptyCount, j];
                    GameObject.Find(i + "+" + j).name = (i + emptyCount) + "+" + j;
                    posBool[i, j] = false;
                    posBool[i + emptyCount, j] = true;

                }
                else
                {
                    emptyCount++;
                }
            }
        }
    }

    //Gönderilen bloğun konum bilgisini gönderen method
    private Vector2 getPos(GameObject game)
    {
        string name = game.name;
        string[] pos = name.Split('+');

        return new Vector2(int.Parse(pos[0]), int.Parse(pos[1]));
    }

    //Ekrana tıklanıldığında çalışacak olan method
    private void OnMouseDown()
    {
        firstTounch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    //Ekrandan tıklanma bitince çalışacak olan method
    private void OnMouseUp()
    {
        finalTounch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        directionCalculate();
    }

    //Saat Yönünde mi yoksa Saat Yönünün Tersinde mi 
    private void directionCalculate()
    {
        if (rotate)
        {
            return;
        }


        float swing = Mathf.Atan2(finalTounch.y - firstTounch.y, firstTounch.x - firstTounch.x);


        float swipeDistVertical = (new Vector3(0, finalTounch.y, 0) - new Vector3(0, firstTounch.y, 0)).magnitude;
        float swipeDistHorizontal = (new Vector3(finalTounch.x, 0, 0) - new Vector3(firstTounch.x, 0, 0)).magnitude;


        if (swipeDistVertical > minSwipeDistY && swipeDistHorizontal > minSwipeDistX)
        {
            //(swipeValueY > 0) Yukarı
            //(swipeValueY < 0) Aşağı
            //(swipeValueX > 0) Sağ
            //(swipeValueX < 0) Sol
            if (click)
            {
                float swipeValueY = Mathf.Sign(finalTounch.y - firstTounch.y);
                float swipeValueX = Mathf.Sign(finalTounch.x - firstTounch.x);

                if (swipeValueX > 0 && swipeValueY > 0 || swipeValueX < 0 && swipeValueY < 0)
                {
                    direction = Rotate.ClockWise;
                }
                else if (swipeValueX < 0 && swipeValueY > 0 || swipeValueX > 0 && swipeValueY < 0)
                {
                    direction = Rotate.CounterClockWise;
                }
                rotate = true;
            }
            else
            {
                clickOperation();
            }
        }
        else
        {
            if (click)
            {
                Destroy(GameObject.Find("click"));
            }
            clickOperation();
        }
    }
}
