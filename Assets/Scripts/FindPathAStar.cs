using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PathMarker
{
    public MapLocation location;
    public float G;
    public float H;
    public float F;
    public GameObject marker;
    public PathMarker parent;

    public PathMarker(MapLocation l, float g, float h, float f, GameObject marker, PathMarker p)
    {
        location = l;
        G = g;
        H = h;
        F = f;
        this.marker = marker;
        parent = p;
    }

    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            return location.Equals(((PathMarker) obj).location);
        }
    }

    public override int GetHashCode()
    {
        return 0;
    }
}


public class FindPathAStar : MonoBehaviour
{
    public Maze maze;
    public Material closedMaterial;
    public Material openMaterial;

    List<PathMarker> open = new List<PathMarker>();   //Lista OPEN de PathMakers
    List<PathMarker> closed = new List<PathMarker>(); //Lista CLOSED de PathMakers

    public GameObject start;
    public GameObject end;
    public GameObject pathP;

    PathMarker goalNode;  //PathMaker que marca la posicion final (DESTINO)
    PathMarker startNode; //PathMaker que marca la posicion inicial (ORIGEN)

    PathMarker lastPos; //Nodo en el que estamos en cada momento, cuando estamos
                        //buscando el DESTINO y cuando estamos dibujando el CAMINO
    bool done = false;  //Booleano que se utiliza para decir si se ha alcanzado el objetivo


    //Eliminar todos los GameObjects que puedan haber en el mapa (maze)
    void RemoveAllMarkers()
    {

    }

    //Inicializamos las posiciones inicial y final y las listas de open y closed
    void BeginSearch()
    {
        //Inicializamos done

        //Eliminar posibles GameObjects en el mapa


        //Creamos una lista de localizaciones validas

        //Para cada posicion del mapa, chequeamos si es un muro (=1 si es muro),
        //si no lo es, la anyadimos a la lista de posiciones validas

        //"Barajamos" las posiciones validas


        //Inicializamos startNode como posicion de salida


        //Inicializamos goalNode como posicion de llegada


        //Borramos open/closed lists por si es un juego nuevo

        //Anyadimos el starNode a la lista open

        //Inicializamos lastPos a startNode

    }

    void Search(PathMarker thisNode)
    {
        //Chequeamos si el goalNode ha sido alcanzado ya


        //Vamos a buscar todos los vecinos
        //foreach (MapLocation dir in maze.directions)
        //{
            //Inicializamos la localizacion "neighbour"
            //Ojo, en este punto neighbour es solo una posicion en el mapa


            //Antes de procesar este vecino chequeamos que cumpla ciertas condiciones:
            //1.- No debe ser un muro

            //2.- El vecino no debe estar fuera del MAZE

            //3.- El vecino no debe estar en la closed list


            //Aqui ya sabemos que el vecino es correcto
                 //Calculamos G: Sumamos la G que teniamos a la distancia entre los nodos vecinos
                 //Calculamos H
                 //Calculamos F

            //Creamos un gameObject "pathBlock" para poner en este punto de nuestro camino


            //"pathBlock" tiene el texto G, F, H adjunto como subcomponentes, los inicializamos esto strings


            //Llamamos a la funcion UpdateMarker para:
            //   1.- testear si neighbour ya esta en la lista open
            //   2.- si lo esta, actualizar sus G H F y el parent (que sera thisNode)
            //Si neighbour no esta en la lista open, deberemos anyadirlo

        //}

        //Vamos a generar muchos vecinos en la open list y tenemos que decidir cual es que se selecciona
        //El criterio es el menor valor F de la open list, esa sera la eleccion
             //Anyadimos el pathMaker a la closed list

        //Quitamos el pathMaker de la open list


        //El nodo que sera el siguiente lastPos es el pm

    }


    //bool UpdateMarker(MapLocation pos, float g, float h, float f, PathMarker prt)
    //{
        //Para cada pathMaker en al open list
        //   Si pos esta en la lista:
        //      actualizamos G H F y parent
        //      devolvemos TRUE
        //Devolvemos FALSE
    //}

    //bool IsClosed(MapLocation marker)
    //{
        //Devolvemos TRUE si marker esta en la lista de closed
    //}


    // Start is called before the first frame update
    void Start()
    {
        
    }

    //GetPath devuelve el camino inverso, es decir, desde el destino al que hemos
    //llegado hasta el pathMaker inicial
    void GetPath()
    {
        //Borrar todos los pathMakers

        //Inicializamos begin a lastPos


        //Mientras startNode no sea begin y begin no sea NULL
        //    Instanciamos un nuevo pathMaker que senyalara el camino de vuelta
        //    begin pasara ahora a se�alar al su nodo padre

        //Instanciamos un ultimo pathMaker que senyalara la posicion inicial

    }

    // Update is called once per frame
    void Update()
    {
        //Si presionamos la letra "P" inicializamos el juego
        if (Keyboard.current[Key.P].wasPressedThisFrame) BeginSearch();
        //Si presionamos la letra "C" calculamos el siguiente movimiento del alg. A*
        if (Keyboard.current[Key.C].wasPressedThisFrame && !done) Search(lastPos);
        //Si presionamos la letra "M" dejamos solo los pathMakers que forman el path optimo
        if (Keyboard.current[Key.M].wasPressedThisFrame) GetPath();
    }
}