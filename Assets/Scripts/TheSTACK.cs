using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TheSTACK : MonoBehaviour
{
    public GameObject endPanel;
    public Text scoreText;
    public Material stackMat;
    public Color32[] gamecolors = new Color32[4];
    public const float bound = 3.5f;
    public float Tiletransition =3.0f;
    public float Tilespeed = 1.0f;
    public int countboard = 0;
    public const float stack_moving_speed = 5.0f;
    public const float error_margin = 0.1f;
    public bool gameover = false;
    public int stackIndex;
    public float secondaryposition;
    public const float stack_bound_gain = 0.25f;
    public const int combo_start_gain = 3;
    public bool isMovingX = true;
    GameObject[] stack;
    public const float stack_speed = 5.0f;
    public int combo = 0;
    public Vector3 desiredposition;
    public Vector2 stackbounds = new Vector2(bound, bound);
    public Vector3 lasttileposition;

	// Use this for initialization

	private void Start ()
    {
        stack = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            stack[i] = transform.GetChild(i).gameObject;
            colorMesh(stack[i].GetComponent<MeshFilter>().mesh);
        }
          stackIndex = transform.childCount - 1;
    }

    //create rubble
    private void CreateRubble(Vector3 pos, Vector3 scale)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.AddComponent<Rigidbody>();
        go.GetComponent<MeshRenderer>().material = stackMat;
        colorMesh(go.GetComponent<MeshFilter>().mesh);
    }
   

    private void Update()
    {
        if (gameover)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if(placetile())
            {
                SpawnTile();
                countboard++;
               scoreText.text=countboard.ToString();
            }
            else
            {
                Endgame();
            }            
        }
        MoveTile();
     transform.position = Vector3.Lerp(transform.position, desiredposition, stack_speed * Time.deltaTime);
    }




    private void MoveTile()
    {

        Tiletransition= Tiletransition+ Time.deltaTime * Tilespeed;
        if (isMovingX)
        {
            stack[stackIndex].transform.localPosition = new Vector3(Mathf.Sin(Tiletransition) * bound, countboard, secondaryposition);
        }

        else 
            stack[stackIndex].transform.localPosition = new Vector3(secondaryposition, countboard, Mathf.Sin(Tiletransition) * bound);

    }







        private void SpawnTile()
        {
                lasttileposition = stack[stackIndex].transform.localPosition;        
            stackIndex--;
        if (stackIndex < 0)
            stackIndex = transform.childCount - 1;
        
        desiredposition = (Vector3.down) * countboard;
        stack[stackIndex].transform.localPosition = new Vector3(0, countboard, 0);
        stack[stackIndex].transform.localScale = (new Vector3(stackbounds.x, 1, stackbounds.y));
       colorMesh(stack[stackIndex].GetComponent<MeshFilter> ().mesh);
 
         }


    
    private bool placetile()
    {
        Transform t = stack[stackIndex].transform;
       if(isMovingX)
        {
            float deltaX = lasttileposition.x- t.position.x;
            if(Mathf.Abs(deltaX) > error_margin)
            {
                combo = 0;
                stackbounds.x -= Mathf.Abs(deltaX);
                if(stackbounds.x<=0)
                    return false;

                float middle = lasttileposition.x + t.localPosition.x/2;
                t.localScale=(new Vector3(stackbounds.x, 1, stackbounds.y));
                CreateRubble
                    (
                    new Vector3((t.position.x>0)
                        ?t.position.x + (t.localScale.x/2 )
                        :t.position.x - (t.localScale.x/2)
                        , t.position.y
                        , t.position.z),
                    new Vector3(Mathf.Abs(deltaX),1,t.localScale.z)
                        
                    );
                    t.localPosition = new Vector3(middle - (lasttileposition.x)/2, countboard, lasttileposition.z);
            }
            else
            {
                if(combo>combo_start_gain)
                {
                    stackbounds.x += stack_bound_gain;
                    if(stackbounds.x>bound)
                    {
                        stackbounds.x = bound;
                    }
                    float middle = lasttileposition.x + t.localPosition.x / 2;
                    t.localScale = (new Vector3(stackbounds.x, 1, stackbounds.y));                   
                    t.localPosition = new Vector3(middle - (lasttileposition.x) / 2, countboard, lasttileposition.z);
                }
                combo++;
                t.localPosition = new Vector3(lasttileposition.x, countboard, lasttileposition.z);
            }
        }
        else
        {
            float deltaZ = lasttileposition.z - t.position.z;
            if (Mathf.Abs(deltaZ) > error_margin)
            {
                combo = 0;
                stackbounds.y -= Mathf.Abs(deltaZ);
                if (stackbounds.y <= 0)
                {
                    return false;
                }
                float middle = lasttileposition.z + t.localPosition.z / 2;
                t.localScale = (new Vector3(stackbounds.x, 1, stackbounds.y));
                CreateRubble
                   (
                   new Vector3(t.position.x
                   , t.position.y
                   , (t.position.z > 0)
                       ? t.position.z + (t.localScale.z/2)
                       : t.position.z - (t.localScale.z/2)),
                   new Vector3(t.localScale.z, 1, Mathf.Abs(deltaZ))

                   );
                t.localPosition = new Vector3(lasttileposition.x, countboard, middle-(lasttileposition.z)/2);                
            }
            else
            {
                if (combo > combo_start_gain)
                {
                    stackbounds.y += stack_bound_gain;
                    if (stackbounds.y > bound)
                    {
                        stackbounds.y = bound;
                    }
                    float middle = lasttileposition.y + t.localPosition.y / 2;
                    t.localScale = (new Vector3(stackbounds.y, 1, stackbounds.y));
                    t.localPosition = new Vector3(middle - (lasttileposition.x) / 2, countboard, lasttileposition.z);
                }
                combo++;
                t.localPosition = new Vector3(lasttileposition.x, countboard, lasttileposition.z);
            }
        }
        secondaryposition = (isMovingX)
            ? t.localPosition.x
            : t.localPosition.z;  
        isMovingX = !isMovingX;

        return true;
    }


    private void colorMesh(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Color32[] colors = new Color32[vertices.Length];
        float f = Mathf.Sin(countboard * 0.25f);
        for(int i=0;i<vertices.Length;i++)
        {
            colors[i] = Lerp4(gamecolors[0], gamecolors[1], gamecolors[2], gamecolors[3],f);
        }
        mesh.colors32 = colors;
    }

    private Color32 Lerp4(Color32 a, Color32 b, Color32 c, Color32 d, float t)
    {
        if (t < 0.33f)
        {
            return Color.Lerp(a, b, t / 0.33f);
        }
        else if (t < 0.66f)
            return Color.Lerp(b, c, (t - 0.33f) / 0.33f);
        else
            return Color.Lerp(c, d, (t - 0.66f) / 0.66f);
    }


    private void Endgame()
    {
        if(PlayerPrefs.GetInt("Score")<countboard)
        {
            PlayerPrefs.SetInt("Score", countboard);
        }
        gameover = true;
        endPanel.SetActive(true);
      stack[stackIndex].AddComponent<Rigidbody>();
    }



    public void OnButtonClick(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
   

}
