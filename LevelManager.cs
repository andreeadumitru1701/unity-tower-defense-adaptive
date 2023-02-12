using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Aceasta clasa se ocupa de generarea nivelului
public class LevelManager : Singleton<LevelManager>
{
    //Un vector de tilePrefabs folosite pentr a crea placile in joc
    [SerializeField]
    private GameObject[] tilePrefabs;

    //Referinta la scriptul de miscare a camerei
    [SerializeField]
    private CameraMovement cameraMovement;

    //Transformarea hartii, necesara pentru adaugarea de noi placi
    [SerializeField]
    private Transform map;

    //Puncte de generare a portalurilor
    private Point blueSpawn, redSpawn;

    //Prefabricat pentru generarea portalului albastru
    [SerializeField]
    private GameObject bluePortalPrefab;

    //Prefabricat pentru generarea portalului rosu
    [SerializeField]
    private GameObject redPortalPrefab;

    public Portal BluePortal { get; set; }

    private Point mapSize;

    private Stack<Node> path;

    public Stack<Node> Path
    {
        get
        {
            if (path==null)
            {
                GeneratePath();
            }

            return new Stack<Node>(new Stack<Node>(path));
        }
    }


    //Dictionar care contine toate placile din cadrul jocului
    public Dictionary<Point,TileScript> Tiles { get; set; }

    //Proprietate pentru returnarea dimensiunii unei placi
    public float TileSize
    {
        get 
        { 
            return tilePrefabs[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x; 
        }
    }

    public Point BlueSpawn
    {
        get
        {
            return blueSpawn;
        }
    }

    public Point RedSpawn
    {
        get
        {
            return redSpawn;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateLevel()
    {
        Tiles = new Dictionary<Point, TileScript>();

        //Instantiere temporara a hartii placilor
        string[] mapData = ReadLevelText();

        mapSize = new Point(mapData[0].ToCharArray().Length, mapData.Length);

        //Calculeaza dimensiunea x a hartii
        int mapX = mapData[0].ToCharArray().Length;

        //Calculeaza dimensiunea y a hartii
        int mapY = mapData.Length;

        Vector3 maxTile = Vector3.zero;

        //Calculeaza punctul de start global, coltul stanga sus al ecranului de joc
        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height));

        for (int y = 0; y < mapY; y++) //Pozitiile y
        {
            char[] newTiles = mapData[y].ToCharArray(); //Preia toate placile necesare pentru plasarea pe harta

            for (int x = 0; x < mapX; x++) //Pozitiile y
            {
                //Plaseaza placile in nivelul de joc
                PlaceTile(newTiles[x].ToString(), x,y,worldStart);
            }
        }

        maxTile = Tiles[new Point(mapX - 1, mapY - 1)].transform.position;

        //Seteaza limitele camerei la pozitia maxima a placilor
        cameraMovement.SetLimits(new Vector3(maxTile.x+TileSize,maxTile.y-TileSize));

        SpawnPortals();
    }

    /// <summary>
    /// Plaseaza o placa in nivelul de joc
    /// </summary>
    /// <param name="tileType">Tipul placii de plasat</param>
    /// <param name="x">Pozitia x a placii</param>
    /// <param name="y">Pozitia y a placii</param>
    /// <param name="worldStart">Pozitia de inceput a hartii de joc</param>
    private void PlaceTile(string tileType, int x, int y, Vector3 worldStart)
    {
        //Converteste tipul placii la int pentru a fi folosit ca indexator la crearea unei noi placi
        int tileIndex = int.Parse(tileType);

        //Creeaza o placa noua si o referinta la aceasta in variabila newTile
        TileScript newTile = Instantiate(tilePrefabs[tileIndex]).GetComponent<TileScript>();

        //Utilizeaza noua variabila de placa pentru schimbarea pozitiei acesteia
        newTile.Setup(new Point(x, y), new Vector3(worldStart.x + (TileSize * x), worldStart.y - (TileSize * y), 0),map);
    }

    /// <summary>
    /// Citeste documentul text pentru crearea nivelului de joc
    /// </summary>
    /// <returns>Un vector de stringuri cu indicatori la placle de plasat</returns>
    private string[] ReadLevelText()
    {
        //Incarca documentul text din folderul Resources
        TextAsset bindData = Resources.Load("Level") as TextAsset;

        //Preia stringul
        string data = bindData.text.Replace(Environment.NewLine, string.Empty);

        //Separa stringul in vectori
        return data.Split('-');
    }

    //Genereaza portalele in joc
    private void SpawnPortals()
    {
        //Genereaza portalul albastru
        blueSpawn = new Point(0, 0);
        GameObject tmp = (GameObject)Instantiate(bluePortalPrefab, Tiles[blueSpawn].GetComponent<TileScript>().WorldPosition, Quaternion.identity);
        BluePortal = tmp.GetComponent<Portal>();
        BluePortal.name = "BluePortal";

        //Genereaza portalul rosu
        redSpawn = new Point(11, 6);
        Instantiate(redPortalPrefab, Tiles[redSpawn].GetComponent<TileScript>().WorldPosition, Quaternion.identity);
    }

    public bool InBounds(Point position)
    {
        return position.X >= 0 && position.Y >= 0 && position.X < mapSize.X && position.Y < mapSize.Y;
    }

    public void GeneratePath()
    {
        path = AStar.GetPath(blueSpawn, redSpawn);
    }

}
